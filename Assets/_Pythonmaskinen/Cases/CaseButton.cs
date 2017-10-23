using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaseButton : MonoBehaviour {

	[Header("Button states")]
	public Sprite active;
	public Sprite inactive;
	public Sprite completed;
	public Sprite failed;

	public Image image;

	public void SetButtonActive(){
		image.sprite = active;
	}

	public void SetButtonInactive(){
		image.sprite = inactive;
	}

	public void SetButtonCompleted(){
		image.sprite = completed;
	}

	public void SetButtonFailed(){
		image.sprite = failed;
	}

	public void SwitchToCase (int caseNumber){
		PMWrapper.SwitchCase (caseNumber);
	}
}
