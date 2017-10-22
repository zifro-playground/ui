using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PM;

namespace PM.Level {

	public class LevelHandler : MonoBehaviour, IPMCompilerStopped {

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
			levels = new Level[PMWrapper.numOfLevels];
			for (int i = 0; i < PMWrapper.numOfLevels; i++) {
				levels [i] = new Level ();
				levels [i].answere = new LevelAnswere ();
			}


			for (int i = 0; i < PMWrapper.numOfLevels; i++) {
				levels [i].BuildLevelSettings (i);
			}
		}

		public void OnPMCompilerStopped(PM.HelloCompiler.StopStatus status){
			if (status == HelloCompiler.StopStatus.RuntimeError){
				// TODO Animate currentCaseButton to red
				currentLevel.caseHandler.CaseFailed();

				// TODO wait before reseting handler and buttons
			}
		}
	}
}