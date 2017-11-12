using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PM.Level;
using PM.Guide;

namespace PM
{
	[System.Serializable]
	public class Main : MonoBehaviour
	{
		public int numberOfLevels = 5;
		public float gameSpeed = 1;
		// TODO Set current language?

		// Everything should be placed in Awake() but there are some things that needs to be set in Awake() in some other script before the things currently in Start() is called
		private void Awake()
		{
			PMWrapper.codewalkerBaseSpeed = gameSpeed;
		}

		private void Start ()
		{
			PMWrapper.numOfLevels = numberOfLevels;

			UISingleton.instance.levelHandler.BuildLevels ();
			GuideLoader.BuildAll ();

			// TODO Load last level played from database
			UISingleton.instance.levelHandler.LoadLevel (0);
		}
	}
}
