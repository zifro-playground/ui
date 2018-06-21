using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PM
{
	public class CaseHandler
	{
		private int numberOfCases = 1;

		public bool AllCasesCompleted = false;
		public int CurrentCase = 0;
		public List<GameObject> CaseButtons;

		public CaseHandler(int numOfCases)
		{
			numberOfCases = numOfCases;
			CaseButtons = CaseParent.Instance.CaseButtons;
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
			SetCaseSettings(caseNumber);

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

		private void SetCaseSettings(int caseNumber)
		{
			if (Main.Instance.LevelData.cases != null && Main.Instance.LevelData.cases.Any())
			{
				var caseSettings = Main.Instance.LevelData.cases[caseNumber].caseSettings;

				if (caseSettings == null)
				{
					PMWrapper.preCode = "";
					return;
				}

				if (!String.IsNullOrEmpty(caseSettings.precode))
					PMWrapper.preCode = caseSettings.precode;

				if (caseSettings.walkerStepTime > 0)
					PMWrapper.walkerStepTime = caseSettings.walkerStepTime;
			}
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

			Main.Instance.StartCoroutine(ShowFeedbackAndRunNextCase());
		}

		public void CaseFailed()
		{
			CaseButtons[CurrentCase].GetComponent<CaseButton>().SetButtonFailed();
		}

		private IEnumerator ShowFeedbackAndRunNextCase()
		{
            string positiveMassage;
            if (numberOfCases == 1)
                positiveMassage = "Bra jobbat!";
            else
                positiveMassage = "Test " + (CurrentCase + 1) + " avklarat!";

            UISingleton.instance.taskDescription.ShowPositiveMessage(positiveMassage);

			yield return new WaitForSeconds(3 * (1 - PMWrapper.speedMultiplier));

			UISingleton.instance.answerBubble.HideMessage();
			UISingleton.instance.taskDescription.HideTaskFeedback();
			CaseButtons[CurrentCase].GetComponent<CaseButton>().SetButtonCompleted();

			CurrentCase++;

			if (CurrentCase >= numberOfCases)
			{
				AllCasesCompleted = true;
				PMWrapper.SetLevelCompleted();
				yield break;
			}

			SetCurrentCase(CurrentCase);
			RunCase(CurrentCase);
		}
	}
}