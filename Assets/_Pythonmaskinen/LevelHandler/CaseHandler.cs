using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Level {

	public class CaseHandler : IPMCompilerStopped {

		private int numberOfCases = 1;
		private int currentCase = 0;
		private List<GameObject> caseButtons = new List<GameObject>();

		public CaseHandler(int numOfCases){
			numberOfCases = numOfCases;
		}

		private void SetCurrentCase(int caseNumber){
			//TODO animate currentCaseButtonUnpressed
			Debug.Log("Set current case number to " + caseNumber);
			currentCase = caseNumber;
			//TODO animate currentCaseButtonPressed
			//TODO Game.LoadCase(caseNumber);

		}

		//Add to pmwrapper
		public void RunCase(int caseNumber){
			Debug.Log ("Run case " + caseNumber);
			if (caseNumber <= numberOfCases) {
				//TODO animate currentCaseButtonRunning
				//TODO RunCode
			} else {
				PMWrapper.SetLevelCompleted ();
			}
		}

		public void OnPMCompilerStopped(PM.HelloCompiler.StopStatus status){
			//TODO Animate currentCaseButton to red
			Debug.Log("Compiler stoped with status " + status);
			ResetHandlerAndButtons();
		}

		public void ResetHandlerAndButtons() {
			/* TODO for (int i = 0; i < numberOfCases; i++) {
				button = Instanciate(prefab, insideButtonHolder)
				button.addListner()    (OnClick calls SetCurrentCase(button.text))
				button.text = i
				caseButtons.add(button)
			}*/
			Debug.Log ("Reseting case handler and buttons");
			SetCurrentCase(0);
		}

		public void CaseCompleted() {
			// TODO animate currentCaseButton to green
			Debug.Log("Case completed!");
			SetCurrentCase(currentCase++);
			RunCase (currentCase);
		}
	}
}