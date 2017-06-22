using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM {

	[DisallowMultipleComponent]
	public class Hax : MonoBehaviour {
		
		void Update() {
			// Why use parantases when you can use multiple IF statements? B)
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
				if (Input.GetKeyDown(KeyCode.F4)) {
					if (PMWrapper.isDemoingLevel)
						UISingleton.instance.manusPlayer.SetIsManusPlaying(false);
					else
						PMWrapper.SetLevelCompleted();
				}
			}
		}

	}

}