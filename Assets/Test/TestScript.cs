using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour, IPMLevelChanged {

	// Use this for initialization
	void Start () {
		PMWrapper.numOfLevels = 2;
		PMWrapper.SetLevelCompleted ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnPMLevelChanged(){
		//PMWrapper.mainCode = "for i in range(4):\ntestcode = 0";
		PMWrapper.AddCodeAtStart ("for i in range(4):\n\ttestcode = 0");
	}
}
