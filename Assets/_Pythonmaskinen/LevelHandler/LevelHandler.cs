using System;
using System.Collections.Generic;
using UnityEngine;

namespace PM
{
	public class LevelHandler : MonoBehaviour
	{
		//public const string settingsResourceName = "settings-master";
		//private static readonly string[] linebreaks = new string[] { "\n\r", "\r\n", "\n", "\r" };

		private OldLevel[] levels;
		public OldLevel currentLevel { get { return levels[PMWrapper.currentLevel]; } }

		// Contains pre created buttons because script does not create buttons
		public List<GameObject> caseButtons;

		public void LoadLevel(int level)
		{
			PMWrapper.StopCompiler();

			// TODO Save mainCode to database
			UISingleton.instance.saveData.ClearPreAndMainCode();
			//currentLevel.levelSetting.UseSettings();

			// Call every implemented event
			foreach (var ev in UISingleton.FindInterfaces<IPMLevelChanged>())
				ev.OnPMLevelChanged();
		}
	}
}