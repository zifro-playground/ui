using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Guide {

	public class GuidePlayer : MonoBehaviour, IPMLevelChanged {

		public LevelGuide currentGuide;
		public bool shouldPlayNext = true;

		public void OnPMLevelChanged(){
			currentGuide = GuideLoader.GetCurrentLevelGuide ();
			if (currentGuide != null)
				currentGuide.currentGuideIndex = 0;
			shouldPlayNext = true;
		}

		private void FixedUpdate(){
			if (currentGuide != null) {
				if (shouldPlayNext) {
					currentGuide.PlayNextGuide ();
					shouldPlayNext = false;
				}
			}
		}

		public void resetCurrentGuide(){
			if (currentGuide != null)
				currentGuide.ResetGuide ();
			shouldPlayNext = true;
		}
	}
}