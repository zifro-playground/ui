using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM {

	public class SetNumLevels : MonoBehaviour {

		public int numOfLevels = 5;

#if UNITY_EDITOR
		private void OnValidate() {
			numOfLevels = Mathf.Clamp(numOfLevels, 1, 20);
		}
#endif

		private void Start() {
			// TODO: Load saved unlocked
			PMWrapper.numOfLevels = numOfLevels;
		}

	}

}