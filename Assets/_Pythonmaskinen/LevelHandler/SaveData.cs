using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PM
{
	public class SaveData : MonoBehaviour
	{
		private static Dictionary<int, string> codes = new Dictionary<int, string>();

		public void ClearPreAndMainCode()
		{
			if (!this.enabled) return;

			if (codes.ContainsKey(PMWrapper.currentLevel))
				PMWrapper.mainCode = codes[PMWrapper.currentLevel];
			else
				PMWrapper.mainCode = string.Empty;

			PMWrapper.preCode = string.Empty;
		}

		public static void SaveMainCode()
		{
			codes[PMWrapper.currentLevel] = PMWrapper.mainCode;
		}
	}

}