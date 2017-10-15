using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IDETaskDescription : MonoBehaviour, IPMLevelChanged {

	public Text currentTaskDescriptionSmall;
	public Text currentTaskDescriptionBig;
	public string[] taskDescriptions;

	public void SetLevelTaskDescription (string[] tasks){
		taskDescriptions = tasks;
	}

	public void OnPMLevelChanged(){
		if (taskDescriptions.Length > PMWrapper.currentLevel) {
			currentTaskDescriptionSmall.text = taskDescriptions [PMWrapper.currentLevel];
			//currentTaskDescriptionBig.text = taskDescriptions [PMWrapper.currentLevel];
		}
	}
}
