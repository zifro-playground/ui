using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Level
{

	public class CaseHandler
	{

		private int numberOfCases = 1;

		public int CurrentCase = 0;
		public List<GameObject> CaseButtons;

		public CaseHandler(int numOfCases)
		{
			numberOfCases = numOfCases;
			CaseButtons = UISingleton.instance.levelHandler.caseButtons;
		}

		public void SetButtons(List<GameObject> buttons)
		{
			CaseButtons = buttons;
		}

		// Is called OnLevelChanged and OnRunButtonClicked
		public void ResetHandlerAndButtons()
		{
			for (int i = 0; i < CaseButtons.Count; i++)
			{
				// Don't show buttons if there is only one case
				if (i < numberOfCases && numberOfCases > 1)
				{
					CaseButtons[i].SetActive(true);
					CaseButtons[i].GetComponent<CaseButton>().SetButtonActive();
				}
				else
				{
					CaseButtons[i].SetActive(false);
				}
			}
			SetCurrentCase(0);
		}

		public void SetCurrentCase(int caseNumber)
		{

			//TODO animate currentCaseButtonUnpressed
			CurrentCase = Mathf.Clamp(caseNumber, 0, numberOfCases);
			//TODO animate currentCaseButtonPressed

			// Call every implemented event
			foreach (var ev in UISingleton.FindInterfaces<IPMCaseSwitched>())
				ev.OnPMCaseSwitched(CurrentCase);
		}

		//Add to pmwrapper
		public void RunCase(int caseNumber)
		{
			//TODO animate currentCaseButtonRunning
			PMWrapper.StartCompiler();

		}


		public void CaseCompleted()
		{
			PMWrapper.StopCompiler();

			CaseButtons[CurrentCase].GetComponent<CaseButton>().SetButtonCompleted();

			CurrentCase++;

			if (CurrentCase >= numberOfCases)
			{
				PMWrapper.SetLevelCompleted();

				return;
			}

			SetCurrentCase(CurrentCase);
			RunCase(CurrentCase);
		}

		public void CaseFailed()
		{
			UISingleton.instance.StartCoroutine(ResetFailButton());
		}

		private IEnumerator ResetFailButton()
		{
			CaseButtons[CurrentCase].GetComponent<CaseButton>().SetButtonFailed();
			yield return new WaitForSeconds(2);
			UISingleton.instance.answerBubble.HideMessage();
			// TODO UISingleton.instance.errorBubble.HideMessage ();
			ResetHandlerAndButtons();
		}

	}
}