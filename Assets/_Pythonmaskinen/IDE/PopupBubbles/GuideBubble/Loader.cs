using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace PM.Guide {

	public static class GuideLoader{

		public const string guideResourceName = "guide-master";
		public static readonly string[] linebreaks = new string[] { "\n\r", "\r\n", "\n", "\r" };
		public static List<LevelGuide> allGuides = new List<LevelGuide>();

		public static void BuildAll() {
			TextAsset masterAsset = Resources.Load<TextAsset>(guideResourceName);

			if (masterAsset == null)
				throw new Exception("The file \"" + guideResourceName + "\" could not be found.");

			string[] textRows = masterAsset.text.Trim().Split(linebreaks, StringSplitOptions.RemoveEmptyEntries);
			string guideFileName = "";

			int guidesBuilt = 0;

			for (int i = 0; i < textRows.Length; i++) {

				if (guidesBuilt >= PMWrapper.numOfLevels)
					break;

				// Ignore comments
				if (textRows[i].StartsWith("//") || textRows[i].StartsWith("#"))
					continue;

				guideFileName = textRows[i].Trim();

				LevelGuide guide = BuildFromPath (guideFileName);
				if (guide != null) {
					guide.level = guidesBuilt;
					allGuides.Add (guide);
				}

				guidesBuilt++;
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
			List<string> rows = new List<string>(fileText.Split(linebreaks, StringSplitOptions.RemoveEmptyEntries));
			LevelGuide levelGuide = new LevelGuide ();

			//Target target;
			string target = "";
			int lineNumber = 0;
			string guideMessage = "";

			for (int i = 0; i < rows.Count; i++) {

				// Comments
				if (rows[i].StartsWith("//") || rows[i].StartsWith("#")) continue;

				// get index of colon and split into target and guidemessage
				int colonIndex = rows[i].IndexOf(":");
				target = rows [i].Substring (0, colonIndex).Trim().ToLower();
				guideMessage = rows[i].Substring(colonIndex+1).Trim();

				// Check if target is a number
				Match match = Regex.Match (target, @"^[0-9]+$");
			
				if (match.Success) {
					int.TryParse (target, out lineNumber);
					levelGuide.guides.Add (new Guide (target, guideMessage, lineNumber));
				} else {
					levelGuide.guides.Add(new Guide(target, guideMessage));
				}
			}
			// Return no levelGuide if it has no guides
			if (levelGuide.guides.Count == 0)
				return null;
			
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