using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour, IPMLevelChanged {

	// Use this for initialization
	void Start () {
		PMWrapper.numOfLevels = 2;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnPMLevelChanged(){
		PMWrapper.AddCodeAtStart ("for i in range(4):\n\ttestcode = 0");
		PMWrapper.ShowGuideBubble(1, "Här ser du ett exempel på en for-slinga");
	}
}
