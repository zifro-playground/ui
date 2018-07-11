using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PM
{
	public class CaseHandler
	{
		public int numberOfCases = 1;

		public bool AllCasesCompleted = false;
		public int CurrentCase = 0;

		public CaseHandler(int numOfCases)
		{
			numberOfCases = numOfCases;
		}

		// Is called OnLevelChanged and OnRunButtonClicked
		public void ResetHandlerAndButtons()
		{
			LevelModeButtons.Instance.SetCaseButtonsToDefault();
			SetCurrentCase(0);
		}

		public void SetCurrentCase(int caseNumber)
		{
			SetCaseSettings(caseNumber);

			if (caseNumber != CurrentCase)
			{
				// currentCaseButtonUnpressed
				LevelModeButtons.Instance.SetCaseButtonsToDefault();

				CurrentCase = Mathf.Clamp(caseNumber, 0, numberOfCases);

				CaseFlash.Instance.HideFlash();
				if (numberOfCases > 1)
					CaseFlash.Instance.ShowNewCaseFlash(CurrentCase);
			}

			// currentCaseButtonPressed
			LevelModeButtons.Instance.SetCurrentCaseButtonState(LevelModeButtonState.Active);

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
			LevelModeButtons.Instance.SetCurrentCaseButtonState(LevelModeButtonState.Failed);
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
			LevelModeButtons.Instance.SetCurrentCaseButtonState(LevelModeButtonState.Completed);

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