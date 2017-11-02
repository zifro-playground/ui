using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PM.Level;
using PM.Guide;

namespace PM {

	public class Main : MonoBehaviour {

		public int numberOfLevels = 5;
		// TODO Set current language?

		void Start () {
			PMWrapper.numOfLevels = numberOfLevels;

			UISingleton.instance.levelHandler.BuildLevels ();
			GuideLoader.BuildAll ();

			// TODO Load last level played from database
			UISingleton.instance.levelHandler.LoadLevel (0);

		}
	}
}
