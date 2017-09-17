﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Guide {

	public class LevelGuide {

		public int level;
		public List<Guide> guides = new List<Guide>();
		public int numOfGuides { get { return guides.Count; } }
		public bool hasNext { get { return currentGuideIndex < numOfGuides; } }

		public int currentGuideIndex = 0;

		public void PlayNextGuide(){
			PMWrapper.ShowGuideBubble (guides [currentGuideIndex].lineNumber, guides [currentGuideIndex].message);
			currentGuideIndex++;
		}
	}
}