using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PM;

public class IDETaskDescription : MonoBehaviour, IPMLevelChanged {

	public GameObject taskDescriptionBigParent;

	public Text taskDescriptionSmall;
	public Text taskDescriptionBig;
	public string[] taskDescriptions;

	public void SetLevelTaskDescription (string[] tasks){
		taskDescriptions = tasks;
		//ChangeTaskDescription ();
	}

	public void OnPMLevelChanged(){
		ChangeTaskDescription ();
	}

	public void ChangeTaskDescription (){
		taskDescriptionBigParent.SetActive (false);
		if (taskDescriptions.Length > PMWrapper.currentLevel) {
			taskDescriptionSmall.text = taskDescriptions [PMWrapper.currentLevel];
			if (!UISingleton.instance.levelHandler.currentLevel.hasShownTaskDescription) {
				taskDescriptionBig.text = taskDescriptions [PMWrapper.currentLevel];
				taskDescriptionBigParent.SetActive (true);
				UISingleton.instance.levelHandler.currentLevel.hasShownTaskDescription = true;
			}
		}
	}
}
