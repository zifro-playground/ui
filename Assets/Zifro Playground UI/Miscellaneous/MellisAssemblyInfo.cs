using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PM
{
	public class MellisAssemblyInfo : AssemblyInfo
	{
		protected override Assembly GetAssembly()
		{
			return typeof(Mellis.Core.Interfaces.IScriptType).Assembly;
		}
	}
}