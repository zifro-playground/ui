using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PM;

namespace PM.Level {

	public class LevelHandler : MonoBehaviour, IPMCompilerStopped {

		public const string settingsResourceName = "settings-master";
		private static readonly string[] linebreaks = new string[] { "\n\r", "\r\n", "\n", "\r" };

		private Level[] levels;
		public Level currentLevel { get { return levels [PMWrapper.currentLevel]; } }

		// Contains pre created buttons because script does not create buttons
		public List<GameObject> caseButtons;

		public void LoadLevel (int level) {
			PMWrapper.StopCompiler();

			// TODO Save mainCode to database
			UISingleton.instance.saveData.ClearPreAndMainCode ();
			currentLevel.levelSetting.UseSettings ();
			currentLevel.caseHandler.ResetHandlerAndButtons ();

			// Call every implemented event
			foreach (var ev in UISingleton.FindInterfaces<IPMLevelChanged>())
				ev.OnPMLevelChanged();
		}

		public void BuildLevels (){


			TextAsset masterAsset = Resources.Load<TextAsset>(settingsResourceName);

			if (masterAsset == null)
			{
				throw new Exception("The file settings-master.txt could not be found.");
			}

			string[] textRows = masterAsset.text.Split(linebreaks, StringSplitOptions.RemoveEmptyEntries);

			levels = new Level[PMWrapper.numOfLevels];
			int levelsBuilt = 0;

			for (int i = 0; i < textRows.Length; i++) {

				// Ignore comments
				if (textRows[i].StartsWith("//") || textRows[i].StartsWith("#"))
					continue;

				// Check if there are more settings specified in master then there are levels
				if (levelsBuilt >= PMWrapper.numOfLevels)
					throw new Exception("There are more files specified in settings-master then in the UI numberOfLevels. Use // to comment out row");

				levels[levelsBuilt] = new Level
				{
					levelAnswer = new LevelAnswer()
				};


				string settingsFileName = textRows[i].Trim();
				levels [levelsBuilt].BuildLevelSettings (levelsBuilt, settingsFileName);

				levelsBuilt++;
			}
			
			if (levelsBuilt != PMWrapper.numOfLevels)
				throw new Exception("The number of levels in settings-master.txt does not match the specified number in the UI.");
		}

		public void OnPMCompilerStopped(PM.HelloCompiler.StopStatus status)
		{
			currentLevel.levelAnswer.compilerHasBeenStopped = true;

			if (status == HelloCompiler.StopStatus.RuntimeError)
			{
				// TODO Animate currentCaseButton to red
				currentLevel.caseHandler.CaseFailed();

				// TODO wait before reseting handler and buttons
			}
		}
	}
}