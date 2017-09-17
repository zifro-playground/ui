using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace PM.Guide {

	public static class GuideLoader{

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
				if (err is GuideLoaderException)
					Debug.LogException(err);
				return null;
			}
		}


		private static LevelGuide BuildFromString(string filename, string fileText) {
			List<string> splitText = new List<string>(fileText.Split(linebreaks, StringSplitOptions.RemoveEmptyEntries));
			LevelGuide levelGuide = new LevelGuide ();

			Target target;
			int lineNumber;
			string guideMessage;
			string[] row;

			for (int i = 0; i < splitText.Count; i++) {

				// Comments
				if (splitText[i].StartsWith("//") || splitText[i].StartsWith("#")) continue;

				row = splitText [i].Trim().Split(':');
				
				// Empty rows
				if (row.Length == 0) continue;

				Match match = Regex.Match (row [0], @"^[0-9]+$");


				if (match.Success) {
					target = Target.lineNumber;
					int.TryParse (row [0], out lineNumber);
				} else {
					throw new GuideLoaderException ("The first word on each row must be a integer.");
				}

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

				levelGuide.guides.Add( new Guide (target, guideMessage, lineNumber));
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