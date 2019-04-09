using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PM.Editor
{
	using UnityEditor;
	using UnityEngine;

	[CustomEditor(typeof(AssemblyInfo), editorForChildClasses: true)]
	public class CustomInspector_AssemblyInfo : Editor
	{
		System.Version newVersion;
		AssemblyInfo assemblyInfo;

		void OnEnable()
		{
			assemblyInfo = (AssemblyInfo)target;
			newVersion = assemblyInfo.GetAssemblyName().Version;
		}

		public override void OnInspectorGUI()
		{
			const int FIELD_COUNT = 3;

			bool compiling = EditorApplication.isCompiling ||
			                 EditorApplication.isUpdating;

			AssemblyName assemblyName = assemblyInfo.GetAssemblyName();

			string versionString;

			bool inAssetsFolder = InAssetsFolder(assemblyInfo);
			bool enabled = inAssetsFolder;

			if (!VersionsEquals(assemblyName.Version, newVersion))
			{
				versionString = assemblyName.Version.ToString(FIELD_COUNT) + " → " + newVersion.ToString(FIELD_COUNT) + " (awaiting asset refresh)";
				enabled = false;
			}
			else if (compiling)
			{
				versionString = assemblyName.Version.ToString(FIELD_COUNT) + " (awaiting compilation)";
				enabled = false;
			}
			else
			{
				versionString = assemblyName.Version.ToString(FIELD_COUNT);
			}

			EditorGUILayout.Space();
			EditorGUILayout.LabelField(assemblyName.Name, EditorStyles.boldLabel);
			
			GUI.enabled = enabled;

			string input = EditorGUILayout.TextField("Assembly Version", versionString);

			if (System.Version.TryParse(input, out System.Version version) &&
			    !VersionsEquals(version, assemblyName.Version))
			{
				OverwriteAssemblyVersion(assemblyInfo, version);
				newVersion = version;

				if (assemblyInfo.updatesProjectVersion)
				{
					PlayerSettings.bundleVersion = newVersion.ToString(FIELD_COUNT);
				}
			}

			GUI.enabled = true;

			if (!inAssetsFolder)
			{
				return;
			}

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(AssemblyInfo.updatesProjectVersion)));
			serializedObject.ApplyModifiedProperties();
			GUI.enabled = false;
			EditorGUILayout.TextField("Project Version", Application.version);
			GUI.enabled = true;

			EditorGUILayout.Space();
			EditorGUILayout.HelpBox("Changing above version will recompile source.", MessageType.Warning);
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
			const StringComparison COMPARISON = StringComparison.OrdinalIgnoreCase;
#else
			const StringComparison COMPARISON = StringComparison.Ordinal;
#endif

			return normalizedFile.StartsWith(normalizedDirectory, COMPARISON);
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
			string path = AssetDatabase.GetAssetPath(monoScript);

			string script = File.ReadAllText(path);

			script = Regex.Replace(script, @"(\[assembly: Assembly(?:File|Informational)?Version\("")([\d.]+)(""\)\])",
				m => $"{m.Groups[1]}{newVersion}{m.Groups[3]}");

			File.WriteAllText(path, script);
		}
	}
}