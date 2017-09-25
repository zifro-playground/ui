using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM {

	public class Level : MonoBehaviour {

		private List<LevelAnswere> answeres = new List<LevelAnswere> ();
		public LevelAnswere currentAnswere { get { return answeres [PMWrapper.currentLevel]; } }

		
	}
}