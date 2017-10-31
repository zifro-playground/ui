using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PM;

public class IDETaskDescription : MonoBehaviour {

	public GameObject bigTDParent;
	public GameObject smallTDParent;

	public Text taskDescriptionSmall;
	public Text taskDescriptionBig;

	public void SetTaskDescription (string taskDescription){
		bigTDParent.SetActive (false);
		if (taskDescription.Length < 1) {
			smallTDParent.SetActive (false);
		} else {
			smallTDParent.SetActive (true);
			taskDescriptionSmall.text = taskDescription;
			if (!UISingleton.instance.levelHandler.currentLevel.hasShownTaskDescription) {
				bigTDParent.SetActive (true);
				taskDescriptionBig.text = taskDescription;
				UISingleton.instance.levelHandler.currentLevel.hasShownTaskDescription = true;
			}
		}

	}
}
