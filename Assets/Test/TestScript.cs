using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour, IPMLevelChanged, IPMCaseSwitched {

	public string[] tasks;


	void Awake(){
		PMWrapper.SetTaskDescriptions (tasks);
		PMWrapper.SetCompilerFunctions (new Compiler.Function[] {
			new AnswereFunction()
		});
		PMWrapper.AddSmartButton ("svara()");
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.U))
			PMWrapper.SetCaseCompleted ();
	}

	public void OnPMLevelChanged(){

	}

	public void OnPMCaseSwitched(int caseNumber){
		Debug.Log ("Game switched to case " + caseNumber);
		//if (caseNumber == 1)
			//PMWrapper.SetCurrentLevelAnswere (Compiler.VariableTypes.number, new string[1] { "5" });
	}
}
