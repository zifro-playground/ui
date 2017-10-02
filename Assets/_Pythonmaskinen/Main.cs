using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PM.Level;

namespace PM {

	public class Main : MonoBehaviour {

		public int numberOfLevels = 5;
		// TODO Set current language?

		void Start () {
			PMWrapper.numOfLevels = numberOfLevels;

			Guide.Loader.BuildAll ();
			UISingleton.instance.levelHandler.BuildLevels ();

			// TODO Load last level played from database
			UISingleton.instance.levelHandler.LoadLevel (0);

		}
	}
}
