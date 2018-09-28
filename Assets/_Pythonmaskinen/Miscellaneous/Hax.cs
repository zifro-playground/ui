using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM {

	[DisallowMultipleComponent]
	public class Hax : MonoBehaviour {
		
		void Update()
		{
			// Why use parantases when you can use multiple IF statements? B)
			if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
			{
				if (Input.GetKeyDown(KeyCode.RightArrow))
				{
					if (PMWrapper.CurrentLevelIndex < PMWrapper.numOfLevels - 1)
					{
						if (PMWrapper.unlockedLevel < PMWrapper.CurrentLevelIndex + 1)
							PMWrapper.unlockedLevel = PMWrapper.CurrentLevelIndex + 1;
						PMWrapper.CurrentLevelIndex += 1;
					}
				}
				else if (Input.GetKeyDown(KeyCode.LeftArrow))
				{
					if(PMWrapper.CurrentLevelIndex > 0)
						PMWrapper.CurrentLevelIndex -= 1;
				}
				else if (Input.GetKeyDown(KeyCode.UpArrow))
				{
					PMWrapper.unlockedLevel = PMWrapper.numOfLevels - 1;
				}
			}
		}
	}
}