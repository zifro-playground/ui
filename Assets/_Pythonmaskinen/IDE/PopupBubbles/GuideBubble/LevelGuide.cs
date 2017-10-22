using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Guide {

	public class LevelGuide {

		public int level;
		public List<Guide> guides = new List<Guide>();
		private Guide currentGuide { get { return guides [currentGuideIndex]; } }
		public int numOfGuides { get { return guides.Count; } }
		public bool hasNext { get { return currentGuideIndex < numOfGuides; } }
		public bool hasBeenPlayed = false;

		public int currentGuideIndex = 0;

		public void PlayNextGuide(){
			if (!hasBeenPlayed) {
				string target = currentGuide.target;

				if (currentGuide.lineNumber >= 0)
					UISingleton.instance.guideBubble.ShowMessage (guides [currentGuideIndex].lineNumber);
				else {
					int index = UISingleton.instance.guidePlayer.guideTargets.FindIndex(s => s.names.Contains(target));
					if (index < 0)
						throw new Exception("No selectable with name \"" + target + "\"!");
					UISingleton.instance.guideBubble.ShowMessage (UISingleton.instance.guidePlayer.guideTargets[index].guideTargets);
				}

				UISingleton.instance.guideBubble.SetGuideMessage(guides [currentGuideIndex].message, currentGuideIndex, numOfGuides);
				currentGuideIndex++;
				if (!hasNext)
					hasBeenPlayed = true;
			}
		}

		public void ResetGuide(){
			hasBeenPlayed = false;
			currentGuideIndex = 0;
		}
	}
}