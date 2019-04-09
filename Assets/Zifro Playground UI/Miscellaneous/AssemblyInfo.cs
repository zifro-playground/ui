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

		public AssemblyName GetAssemblyName()
		{
			return typeof(AssemblyInfo).Assembly.GetName();
		}
	}
}