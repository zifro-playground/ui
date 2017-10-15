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

	private Image image;

	private void Start(){
		image = GetComponent<Image> ();
	}


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
