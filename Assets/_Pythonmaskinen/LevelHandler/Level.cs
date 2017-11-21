using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PM;

namespace PM.Level {

	public class Level {

		private static readonly string[] linebreaks = new string[] { "\n\r", "\r\n", "\n", "\r" };

		public int levelNumber;
		public LevelAnswer levelAnswer;
		public LevelSetting levelSetting;
		public CaseHandler caseHandler;

		public bool hasShownTaskDescription = false;

		public void BuildLevelSettings(int levelNumber, string settingsFileName){
			this.levelNumber = levelNumber;

			TextAsset asset = Resources.Load<TextAsset> (settingsFileName);

			if (asset == null) {
				if (!settingsFileName.StartsWith("-"))
					Debug.Log("The levelsettings file \"" + settingsFileName + "\" could not be found. Ignore if you want no settings for level " + levelNumber);
				levelSetting = new LevelSetting ();
				caseHandler = new CaseHandler (1);
				return;
			}

			string[] textRows = asset.text.Split (linebreaks, StringSplitOptions.RemoveEmptyEntries);

			string taskDescription = "";
			string preCode = "";
			string startCode = "";
			int rowLimit = 100;
			int numberOfCases = 1;
			float gameSpeed = -1;
			string[] smartButtons = new string[0];
			List<Compiler.Function> functions= new List<Compiler.Function>();

			for (int i = 0; i < textRows.Length; i++) {
				// Ignore comments
				if (textRows [i].StartsWith ("//"))
					continue;

				string[] splittedRow = textRows[i].Split(':');

				switch (splittedRow[0].ToLower()) {

					case "taskdescription":
						taskDescription = JoinSplittedText (splittedRow);
						break;

					case "precode":
						preCode = JoinSplittedText (splittedRow);
						break;

					case "startcode":
						startCode = JoinSplittedText (splittedRow);
						break;

					case "rowlimit":
						bool couldParse = int.TryParse(splittedRow[1], out rowLimit);
						if (!couldParse)
							throw new Exception("The row with rowLimit must have an integer after :");
						break;

					case "casecount":
						couldParse = int.TryParse (splittedRow [1].Trim (), out numberOfCases);
						if (!couldParse)
							throw new Exception ("The casecount could not be parsed. There must be an integer after :");
						break;

					case "gamespeed":
						couldParse = float.TryParse(splittedRow[1], out gameSpeed);
						if (!couldParse)
							throw new Exception("The row with gameSpeed must have a float after :");
						break;

					case "smartbuttons":
						smartButtons = splittedRow [1].Trim().Replace(" ", "").Split (new char[] {','}, StringSplitOptions.RemoveEmptyEntries);

						// Automatically add AnswerFunction if there is a smart button with name "svara()"
						foreach (string buttonName in smartButtons)
						{
							if (buttonName == "svara()")
								functions.Add(new AnswerFunction());
						}
						break;
		
					case "functions":
						string[] functionsNames = splittedRow [1].Trim ().Replace (" ", "").Split (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

						// Use reflection to get an instance of compiler function class from string
						foreach (string functionName in functionsNames) {
							Type type = Type.GetType (functionName);

							if (type == null)
								throw new Exception ("Function name: \"" + functionName + "\" could not be found.");

							Compiler.Function function = (Compiler.Function)Activator.CreateInstance (type);
							functions.Add (function);
						}
						break;
					default:
						Debug.Log ("Row number " + i + " could not be parsed");
						break;
				}
			}
			levelSetting = new LevelSetting (taskDescription, preCode, startCode, rowLimit, numberOfCases, gameSpeed, smartButtons, functions.ToArray());
			// this should perhaps be moved somewhere else
			//caseHandler = new CaseHandler (numberOfCases);
		}

		// Can be used if text got splitted by : when not intended
		private string JoinSplittedText (string[] text){
			string result = "";
			for (int i = 1; i < text.Length; i++) {
				result += text [i];
				if (i != text.Length - 1)
					result += ":";
			}
			// Quick solution to encoding problem with text file
			result = result.Replace ("\\n", "\n");
			result = result.Replace ("\\t", "\t");

			return result.Trim();
		}
	}
}