using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM {

	[DisallowMultipleComponent]
	public class Hax : MonoBehaviour {
		
		void Update() {
			// Why use parantases when you can use multiple IF statements? B)
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
				if (Input.GetKey(KeyCode.Z)) {
					if (Input.GetKeyDown(KeyCode.X)){
						if (PMWrapper.isDemoingLevel)
							UISingleton.instance.manusPlayer.SetIsManusPlaying (false);
						else {
							PMWrapper.unlockedLevel = PMWrapper.currentLevel + 1;
							PMWrapper.currentLevel += 1;
						}
					} if (Input.GetKeyDown(KeyCode.A)){
						if (PMWrapper.isDemoingLevel)
							UISingleton.instance.manusPlayer.SetIsManusPlaying(false);
						else
							PMWrapper.unlockedLevel = PMWrapper.numOfLevels-1;
					}
				}
			}
		}

	}

}