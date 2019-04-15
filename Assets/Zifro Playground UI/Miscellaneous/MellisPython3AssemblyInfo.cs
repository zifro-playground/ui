using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PM
{
	public class MellisPython3AssemblyInfo : AssemblyInfo
	{
		protected override Assembly GetAssembly()
		{
			return typeof(Mellis.Lang.Python3.PyCompiler).Assembly;
		}
	}
}