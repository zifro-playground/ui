using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace PM.Guide {

	public static class Loader{

		public const string resourceName = "Guides/levelguide_{0}";
		public static readonly string[] linebreaks = new string[] { "\n\r", "\r\n", "\n", "\r" };
		public static List<LevelGuide> allGuides = new List<LevelGuide>();

		public static void BuildAll() {
			for (int i=0; i<PMWrapper.numOfLevels; i++) {
				LevelGuide guide = BuildFromPath (string.Format (resourceName, i));
				if (guide != null) {
					guide.level = i;
					allGuides.Add (guide);
				}
			}
		}

		public static LevelGuide BuildFromPath(string path) {
			TextAsset asset = Resources.Load<TextAsset>(path);

			if (asset == null)
				return null;
			try {
				return BuildFromString("Assets/Resources/Guides" + asset.name, asset.text);
			} catch (Exception err) {
				Debug.Log (err);
				return null;
			}
		}


		private static LevelGuide BuildFromString(string filename, string fileText) {
			List<string> splitText = new List<string>(fileText.Split(linebreaks, StringSplitOptions.RemoveEmptyEntries));
			LevelGuide levelGuide = new LevelGuide ();

			//Target target;
			string target = "";
			int lineNumber = 0;
			string guideMessage = "";
			string[] row;

			for (int i = 0; i < splitText.Count; i++) {

				// Comments
				if (splitText[i].StartsWith("//") || splitText[i].StartsWith("#")) continue;

				row = splitText [i].Trim().Split(new char[1] {':'}, StringSplitOptions.RemoveEmptyEntries);

				// Empty rows
				if (row.Length == 0) continue;


				// Checks if message uses : in text and rejoin strings if true
				if (row.Length > 2) {
					List<string> tempList = new List<string> ();
					for (int j = 1; j < row.Length; j++) {	
						if (j == 1)
							row [j] = row [j].TrimStart ();
						tempList.Add (row [j]);
					}
					guideMessage = string.Join ("", tempList.ToArray());
				} else {
					guideMessage = row [1].Trim ();
				}


				target = row [0].Trim().ToLower();

				// Check if target is a number
				Match match = Regex.Match (target, @"^[0-9]+$");
			
				if (match.Success) {
					int.TryParse (target, out lineNumber);
					levelGuide.guides.Add (new Guide (target, guideMessage, lineNumber));
				} else {
					levelGuide.guides.Add(new Guide(target, guideMessage));
				}


			}
			return levelGuide;
		}

		public static LevelGuide GetCurrentLevelGuide(){
			foreach (LevelGuide guide in allGuides) {
				if (guide.level == PMWrapper.currentLevel)
					return guide;
			}
			return null;
		}
	}
}