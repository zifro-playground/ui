using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM {

	public class Main : MonoBehaviour {

		public int numberOfLevels = 5;

		void Start () {
			PMWrapper.numOfLevels = numberOfLevels;

			Guide.Loader.BuildAll ();
			// TODO 
			// - LoadLevelSettings
			// - LoadLevelAnsweres

			// TODO Load last level played from database
			UISingleton.instance.levelHandler.LoadLevel (0);

		}
	}
}
