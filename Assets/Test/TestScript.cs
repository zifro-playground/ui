using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour, IPMLevelChanged {

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
		PMWrapper.preCode = "x = 19";
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.K)) {
			PMWrapper.ShowGuideBubble (1, "Detta är <b>ett</b> mycket bra tips!");
		}
	}

	public void OnPMLevelChanged(){

	}
}
