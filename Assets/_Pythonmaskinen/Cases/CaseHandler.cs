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
					CaseButtons[i].GetComponent<CaseButton>().SetButtonDefault();
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
			if (caseNumber != CurrentCase)
			{
				// currentCaseButtonUnpressed
				CaseButtons[CurrentCase].GetComponent<CaseButton>().SetButtonDefault();

				CurrentCase = Mathf.Clamp(caseNumber, 0, numberOfCases);

				CaseFlash.Instance.HideFlash();
				if (numberOfCases > 1)
					CaseFlash.Instance.ShowNewCaseFlash(CurrentCase);
			}

			// currentCaseButtonPressed
			CaseButtons[CurrentCase].GetComponent<CaseButton>().SetButtonActive();

			// Call every implemented event
			foreach (var ev in UISingleton.FindInterfaces<IPMCaseSwitched>())
				ev.OnPMCaseSwitched(CurrentCase);
		}
		
		public void RunCase(int caseNumber)
		{
			CaseFlash.Instance.HideFlash();
			if (numberOfCases > 1)
				CaseFlash.Instance.ShowNewCaseFlash(CurrentCase, true);
			else
				PMWrapper.StartCompiler();

		}


		public void CaseCompleted()
		{
			PMWrapper.StopCompiler();

			UISingleton.instance.levelHandler.StartCoroutine(ShowFeedbackAndRunNextCase());
		}

		public void CaseFailed()
		{
			UISingleton.instance.levelHandler.StartCoroutine(ResetFailButton());
		}

		private IEnumerator ResetFailButton()
		{
			CaseButtons[CurrentCase].GetComponent<CaseButton>().SetButtonFailed();
			yield return new WaitForSeconds(2);
			UISingleton.instance.answerBubble.HideMessage();
			// TODO UISingleton.instance.errorBubble.HideMessage ();
			ResetHandlerAndButtons();
		}

		private IEnumerator ShowFeedbackAndRunNextCase()
		{
			UISingleton.instance.taskDescription.ShowPositiveMessage("Test " + (CurrentCase + 1) + " avklarat!");

			yield return new WaitForSeconds(3 * (1 - PMWrapper.speedMultiplier));

			UISingleton.instance.answerBubble.HideMessage();
			UISingleton.instance.taskDescription.HideTaskFeedback();
			CaseButtons[CurrentCase].GetComponent<CaseButton>().SetButtonCompleted();

			CurrentCase++;

			if (CurrentCase >= numberOfCases)
			{
				PMWrapper.SetLevelCompleted();
				yield break;
			}

			SetCurrentCase(CurrentCase);
			RunCase(CurrentCase);
		}
	}
}