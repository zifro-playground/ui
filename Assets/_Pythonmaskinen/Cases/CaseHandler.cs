using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Level {

	public class CaseHandler {

		private int numberOfCases = 1;
		public int currentCase = 0;
		public List<GameObject> caseButtons = new List<GameObject>();

		public CaseHandler(int numOfCases){
			numberOfCases = numOfCases;
			caseButtons = UISingleton.instance.levelHandler.caseButtons;
		}

		public void SetButtons (List<GameObject> buttons){
			caseButtons = buttons;
		}
		
		public void ResetHandlerAndButtons() {
			
			// This script should create buttons but right now they are pre created
			/* TODO for (int i = 0; i < numberOfCases; i++) {
				button = Instanciate(prefab, insideButtonHolder)
				button.addListner()    (OnClick calls SetCurrentCase(button.text))
				button.text = i
				caseButtons.add(button)
			}*/

			for (int i = 0; i < caseButtons.Count; i++) {
				// Don't show buttons if there is only one case
				if (i < numberOfCases && numberOfCases > 1) {
					caseButtons [i].SetActive (true);
					caseButtons [i].GetComponent<CaseButton> ().SetButtonActive ();
				} else {
					caseButtons [i].SetActive (false);
				}
			}
			SetCurrentCase(0);
		}

		public void SetCurrentCase(int caseNumber){

			//TODO animate currentCaseButtonUnpressed
			currentCase = Mathf.Clamp(caseNumber, 0, numberOfCases);
			//TODO animate currentCaseButtonPressed

			// Call every implemented event
			foreach (var ev in UISingleton.FindInterfaces<IPMCaseSwitched>())
				ev.OnPMCaseSwitched(currentCase);
		}

		//Add to pmwrapper
		public void RunCase(int caseNumber){
			//TODO animate currentCaseButtonRunning
			PMWrapper.StartCompiler();

		}


		public void CaseCompleted() {
			PMWrapper.StopCompiler ();
			
			caseButtons[currentCase].GetComponent<CaseButton>().SetButtonCompleted();

			currentCase++;

			if (currentCase >= numberOfCases) {
				PMWrapper.SetLevelCompleted ();

				return;
			}

			SetCurrentCase(currentCase);
			RunCase (currentCase);
		}

		public void CaseFailed() {
			UISingleton.instance.StartCoroutine (ResetFailButton());
		}

		private IEnumerator ResetFailButton (){
			caseButtons[currentCase].GetComponent<CaseButton>().SetButtonFailed();
			yield return new WaitForSeconds(2);
			UISingleton.instance.answerBubble.HideMessage ();
			// TODO UISingleton.instance.errorBubble.HideMessage ();
			ResetHandlerAndButtons();
		}

	}
}