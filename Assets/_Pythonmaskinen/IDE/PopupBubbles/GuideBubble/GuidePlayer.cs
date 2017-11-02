using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Guide {

	public class GuidePlayer : MonoBehaviour, IPMLevelChanged, IPMCompilerStarted {

		public LevelGuide currentGuide;
		public bool shouldPlayNext = true;

		public List<GuideTargets> guideTargets;

		[Serializable]
		public struct GuideTargets {
			public RectTransform guideTargets;
			public List<string> names;
		}

		public void OnPMLevelChanged(){
			currentGuide = GuideLoader.GetCurrentLevelGuide ();
			if (currentGuide != null)
				currentGuide.currentGuideIndex = 0;
			shouldPlayNext = true;
		}

		public void OnPMCompilerStarted(){
			if (currentGuide != null) {
				currentGuide.hasBeenPlayed = true;
				UISingleton.instance.guideBubble.HideMessage ();
			}
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