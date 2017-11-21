using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour, IPMLevelChanged, IPMCaseSwitched {

	public string[] tasks;


	void Awake(){
		PMWrapper.AddSmartButton ("svara()");
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.U))
			PMWrapper.StopCompiler ();
	}

	public void OnPMLevelChanged(){
	}

	public void OnPMCaseSwitched(int caseNumber){
		if (caseNumber == 0)
			PMWrapper.SetCaseAnswer (1);
	}
}
