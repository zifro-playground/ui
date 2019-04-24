using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor.AnimatedValues;

namespace PM.Editor
{
	using UnityEditor;
	using UnityEngine;

	[CustomEditor(typeof(AssemblyInfo), editorForChildClasses: true)]
	public class CustomInspector_AssemblyInfo : Editor
	{
		System.Version newVersion;
		AssemblyInfo assemblyInfo;
		AnimBool animBoolExtraAssembliesProperty;
		AnimBool animBoolEditProjectSettings;

		void OnEnable()
		{
			assemblyInfo = (AssemblyInfo)target;
			newVersion = assemblyInfo.GetAssemblyName().Version;
			animBoolExtraAssembliesProperty = new AnimBool(assemblyInfo.canEditMultipleScriptFiles);
			animBoolExtraAssembliesProperty.valueChanged.AddListener(Repaint);
			animBoolEditProjectSettings = new AnimBool(assemblyInfo.updatesProjectVersion);
			animBoolEditProjectSettings.valueChanged.AddListener(Repaint);
		}

		public override void OnInspectorGUI()
		{
			const int fieldCount = 3;

			bool compiling = EditorApplication.isCompiling ||
			                 EditorApplication.isUpdating;

			AssemblyName assemblyName = assemblyInfo.GetAssemblyName();

			string versionString;

			bool enabled = assemblyInfo.canBeEdited&& InAssetsFolder(assemblyInfo);

			if (!VersionsEquals(assemblyName.Version, newVersion))
			{
				versionString = assemblyName.Version.ToString(fieldCount) + " → " + newVersion.ToString(fieldCount) +
				                " (awaiting asset refresh)";
				enabled = false;
			}
			else if (compiling)
			{
				versionString = assemblyName.Version.ToString(fieldCount) + " (awaiting compilation)";
				enabled = false;
			}
			else
			{
				versionString = assemblyName.Version.ToString(fieldCount);
			}

			EditorGUILayout.Space();
			EditorGUILayout.LabelField(assemblyName.Name, EditorStyles.boldLabel);

			GUI.enabled = enabled;

			string input = EditorGUILayout.TextField("Assembly Version", versionString);

			if (System.Version.TryParse(input, out System.Version version) &&
			    !VersionsEquals(version, assemblyName.Version))
			{
				OverwriteAssemblyVersion(assemblyInfo, version);
				if (assemblyInfo.canEditMultipleScriptFiles &&
				    assemblyInfo.alsoEditsAssemblyInfos != null)
				{
					foreach (MonoScript otherAssemblyInfo in assemblyInfo.alsoEditsAssemblyInfos)
					{
						OverwriteAssemblyVersion(otherAssemblyInfo, version);
					}
				}

				newVersion = version;

				if (assemblyInfo.updatesProjectVersion)
				{
					PlayerSettings.bundleVersion = newVersion.ToString(fieldCount);
				}
			}

			GUI.enabled = true;

			if (!(assemblyInfo.canBeEdited&& InAssetsFolder(assemblyInfo)))
			{
				return;
			}

			EditorGUILayout.Space();
			EditorGUILayout.HelpBox("Changing above version will recompile source.", MessageType.Warning);

			EditorGUILayout.Space();

			SerializedProperty updatesProjectVersion =
				serializedObject.FindProperty(nameof(AssemblyInfo.updatesProjectVersion));
			EditorGUILayout.PropertyField(updatesProjectVersion, true);

			animBoolEditProjectSettings.target = updatesProjectVersion.boolValue;
			if (EditorGUILayout.BeginFadeGroup(animBoolEditProjectSettings.faded))
			{
				GUI.enabled = false;
				EditorGUILayout.TextField("Project Version", Application.version);
				GUI.enabled = true;
			}
			EditorGUILayout.EndFadeGroup();

			EditorGUILayout.Space();

			SerializedProperty editsOtherScripts =
				serializedObject.FindProperty(nameof(AssemblyInfo.canEditMultipleScriptFiles));
			EditorGUILayout.PropertyField(editsOtherScripts, true);

			animBoolExtraAssembliesProperty.target = editsOtherScripts.boolValue;
			if (EditorGUILayout.BeginFadeGroup(animBoolExtraAssembliesProperty.faded))
			{
				SerializedProperty otherScripts =
					serializedObject.FindProperty(nameof(AssemblyInfo.alsoEditsAssemblyInfos));
				EditorGUILayout.PropertyField(otherScripts, true);
			}
			EditorGUILayout.EndFadeGroup();

			serializedObject.ApplyModifiedProperties();
		}

		static bool VersionsEquals(System.Version a, System.Version b, int fieldCount = 3)
		{
			return a.ToString(fieldCount) == b.ToString(fieldCount);
		}

		static bool InAssetsFolder(MonoBehaviour script)
		{
			var monoScript = MonoScript.FromMonoBehaviour(script);
			string localPath = AssetDatabase.GetAssetPath(monoScript);
			string directory = Directory.GetCurrentDirectory();
			string absPath = Path.Combine(directory, localPath);

			string assetsPath = Application.dataPath;

			return IsFileBelowDirectory(absPath, assetsPath);
		}

		static bool IsFileBelowDirectory(string file, string directory)
		{
			string normalizedFile = NormalizePath(file);
			string normalizedDirectory = NormalizePath(directory) + Path.DirectorySeparatorChar;

#if UNITY_EDITOR_WIN
			const StringComparison comparison = StringComparison.OrdinalIgnoreCase;
#else
			const StringComparison comparison = StringComparison.Ordinal;
#endif

			return normalizedFile.StartsWith(normalizedDirectory, comparison);
		}

		static string NormalizePath(string path)
		{
			try
			{
				return Path.GetFullPath(new Uri(path, UriKind.Absolute).LocalPath)
					.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			}
			catch
			{
				Debug.LogError($"Invalid URI: '{path}'");
				return string.Empty;
			}
		}

		static void OverwriteAssemblyVersion(MonoBehaviour assemblyInfo, System.Version newVersion)
		{
			var monoScript = MonoScript.FromMonoBehaviour(assemblyInfo);
			OverwriteAssemblyVersion(monoScript, newVersion);
		}

		static void OverwriteAssemblyVersion(MonoScript monoScript, System.Version newVersion)
		{
			string path = AssetDatabase.GetAssetPath(monoScript);

			string script = File.ReadAllText(path);

			script = Regex.Replace(script, @"(\[assembly: Assembly(?:File|Informational)?Version\("")([\d.]+)(""\)\])",
				m => $"{m.Groups[1]}{newVersion}{m.Groups[3]}");

			File.WriteAllText(path, script);
		}
	}
}
