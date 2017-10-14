using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Level {

	public class CaseHandler {

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

			// Call every implemented event
			foreach (var ev in UISingleton.FindInterfaces<IPMCaseSwitched>())
				ev.OnPMCaseSwitched(currentCase);

		}

		//Add to pmwrapper
		public void RunCase(int caseNumber){
			Debug.Log ("Run case " + caseNumber);
			//TODO animate currentCaseButtonRunning
			PMWrapper.StartCompiler();

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
			Debug.Log("Case nr " + currentCase + " completed!");
			currentCase++;

			if (currentCase > numberOfCases) {
				PMWrapper.SetLevelCompleted ();
				return;
			}

			SetCurrentCase(currentCase);
			RunCase (currentCase);
		}
	}
}