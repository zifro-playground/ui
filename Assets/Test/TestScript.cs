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
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void OnPMLevelChanged(){

	}
}
