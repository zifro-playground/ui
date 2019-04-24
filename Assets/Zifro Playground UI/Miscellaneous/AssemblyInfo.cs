using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
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

		public MonoScript[] alsoEditsAssemblyInfos;
	}
}
