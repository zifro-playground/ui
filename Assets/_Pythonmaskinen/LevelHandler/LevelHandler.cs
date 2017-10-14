using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PM;

namespace PM.Level {

	public class LevelHandler : MonoBehaviour, IPMCompilerStopped {

		private Level[] levels;
		public Level currentLevel { get { return levels [PMWrapper.currentLevel]; } }

		public void LoadLevel (int level) {
			PMWrapper.StopCompiler();

			// TODO Save mainCode to database
			UISingleton.instance.saveData.ClearPreAndMainCode ();
			currentLevel.levelSetting.UseSettings ();
			// TODO currentLevel.caseHandler.ResetHandlerAndButtons ();

			// Call every implemented event
			foreach (var ev in UISingleton.FindInterfaces<IPMLevelChanged>())
				ev.OnPMLevelChanged();
		}

		public void BuildLevels (){
			levels = new Level[PMWrapper.numOfLevels];
			for (int i = 0; i < PMWrapper.numOfLevels; i++)
				levels [i] = new Level ();

			BuildAnsweresFromFile ();

			for (int i = 0; i < PMWrapper.numOfLevels; i++) {
				levels [i].BuildLevelSettings (i);
			}
		}


		private void BuildAnsweresFromFile () {
			string[] linebreaks = new string[] { "\n\r", "\r\n", "\n", "\r" };
			TextAsset asset = Resources.Load<TextAsset> ("answeres");

			if (asset == null)
				return;

			string[] textRows = asset.text.Split (linebreaks, StringSplitOptions.RemoveEmptyEntries);

			if (textRows.Length != PMWrapper.numOfLevels)
				throw new Exception ("The number of answeres in \"Assets/Resources/answeres.txt\" does not match the number of levels. Should be " + PMWrapper.numOfLevels);

			for (int i = 0; i < textRows.Length; i++) {
				int parameterAmount = 0;
				Compiler.VariableTypes type = Compiler.VariableTypes.None;
				string[] answere = new string[0];

				if (!textRows [i].StartsWith ("-")) {
					// TODO Parse Variable Type
					string[] splittedRow = textRows[i].Split(':');

					if (splittedRow.Length != 2)
						throw new Exception ("A row should contain a variable type and a answere separated by :");

					string textType = splittedRow [0].Trim ().ToLower();
					switch (textType) {
					case "number": 
						type = Compiler.VariableTypes.number;
						break;
					case "text":
						type = Compiler.VariableTypes.textString;
						break;
					case "bool":
						type = Compiler.VariableTypes.boolean;
						break;
					default: 
						throw new Exception (textType + " is not a supported variable type. Choose number, text or bool.");
					}

					answere = splittedRow [1].Trim().Replace(" ", "").Split (new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
					parameterAmount = answere.Length;
				}
					
				LevelAnswere levelAnswere = new LevelAnswere (parameterAmount, type, answere);
				levels [i].answere = levelAnswere;
			}
		}

		public void OnPMCompilerStopped(PM.HelloCompiler.StopStatus status){
			Debug.Log("Compiler stoped with status " + status);
			if (status == HelloCompiler.StopStatus.RuntimeError){
				// TODO Animate currentCaseButton to red
				// 		wait before reseting handler and buttons
			}
		}

		public void OnPMLevelChanged(){
			UISingleton.instance.levelHandler.currentLevel.caseHandler.ResetHandlerAndButtons ();
		}
	}
}