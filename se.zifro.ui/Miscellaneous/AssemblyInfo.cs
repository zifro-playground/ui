using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace PM
{
	public abstract class AssemblyInfo : MonoBehaviour
	{
		public bool updatesProjectVersion;
		public bool canEditMultipleScriptFiles;

		public AssemblyName GetAssemblyName()
		{
			return GetAssembly().GetName();
		}

		protected virtual Assembly GetAssembly()
		{
			return GetType().Assembly;
		}

		public virtual bool canBeEdited => false;

#if UNITY_EDITOR
		public MonoScript[] alsoEditsAssemblyInfos;
#endif
	}
}
