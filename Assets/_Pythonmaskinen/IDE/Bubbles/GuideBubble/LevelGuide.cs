using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Guide {

	public class LevelGuide {

		public int level;
		public List<Guide> guides = new List<Guide>();
		public int numOfGuides { get { return guides.Count; } }
		public bool hasNext { get { return currentGuideIndex < numOfGuides; } }
		public bool hasBeenPlayed = false;

		public int currentGuideIndex = 0;

		public void PlayNextGuide(){
			if (!hasBeenPlayed) {

				// TODO possibility to show message at any given target associated with the guide
				UISingleton.instance.guideBubble.ShowMessage (guides [currentGuideIndex].lineNumber);
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