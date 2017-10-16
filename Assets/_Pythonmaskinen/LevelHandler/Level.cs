using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PM;

namespace PM.Level {

	public class Level {

		private static readonly string[] linebreaks = new string[] { "\n\r", "\r\n", "\n", "\r" };
		private const string resourceName = "LevelSettings/levelsettings_{0}";

		public LevelAnswere answere;
		public LevelSetting levelSetting;
		public CaseHandler caseHandler;

		public void BuildLevelSettings(int levelNumber){
			TextAsset asset = Resources.Load<TextAsset> (string.Format(resourceName, levelNumber));

			if (asset == null) {
				levelSetting = new LevelSetting ();
				caseHandler = new CaseHandler (1);
				return;
			}

			string[] textRows = asset.text.Split (linebreaks, StringSplitOptions.RemoveEmptyEntries);

			string preCode = "";
			string startCode = "";
			int rowLimit = 100;
			string[] smartButtons = new string[0];
			int numberOfCases = 1;

			for (int i = 0; i < textRows.Length; i++) {
				// Ignore comments
				if (textRows [i].StartsWith ("//"))
					continue;

				string[] splittedRow = textRows[i].Split(':');

				switch (splittedRow[0].ToLower()) {

				case "precode":
					preCode = JoinSplittedText (splittedRow);
					break;

				case "startcode":
					startCode = JoinSplittedText (splittedRow);
					break;

				case "rowlimit":
					bool couldParse = int.TryParse (splittedRow [1], out rowLimit);
					if (!couldParse)
						throw new Exception ("The row with rowLimit must have an integer after :");
					break;

				case "smartbuttons":
					smartButtons = splittedRow [1].Trim().Replace(" ", "").Split (new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
					break;
				case "casecount":
					couldParse = int.TryParse (splittedRow [1].Trim (), out numberOfCases);
					if (!couldParse)
						throw new Exception ("The casecount could not be parsed. Must be an integer after :");
					break;
				default:
					Debug.Log ("Row number " + i + " could not be parsed");
					break;
				}
			}
			levelSetting = new LevelSetting (preCode, startCode, rowLimit, smartButtons, numberOfCases);
			// this should perhaps be moved somewhere else
			caseHandler = new CaseHandler (numberOfCases);
		}

		// Can be used if text got splitted by : when not intended
		private string JoinSplittedText (string[] text){
			string result = "";
			for (int i = 1; i < text.Length; i++) {
				result += text [i];
				if (i != text.Length - 1)
					result += ":";
			}
			Debug.Log (result);
			// Quick solution to encoding problem with text file
			result = result.Replace ("\\n", "\n");
			result = result.Replace ("\\t", "\t");

			return result.Trim();
		}
	}
}