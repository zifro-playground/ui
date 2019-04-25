using System.Collections;
using UnityEngine;

namespace PM
{
	public class CaseHandler
	{
		public readonly int numberOfCases;

		public bool allCasesCompleted;
		public bool autoContinueTest = true;
		public int currentCase;
		public bool isCasesRunning;

		public CaseHandler(int numOfCases)
		{
			numberOfCases = numOfCases;
		}

		// Is called OnLevelChanged and OnRunButtonClicked
		public void ResetHandlerAndButtons()
		{
			LevelModeButtons.instance.SetCaseButtonsToDefault();
			SetCurrentCase(0);
		}

		public void SetCurrentCase(int caseNumber)
		{
			if (caseNumber != currentCase)
			{
				// currentCaseButtonUnpressed
				LevelModeButtons.instance.SetCaseButtonsToDefault();

				currentCase = Mathf.Clamp(caseNumber, 0, numberOfCases);

				CaseFlash.instance.HideFlash();
				if (numberOfCases > 1)
				{
					CaseFlash.instance.ShowNewCaseFlash(currentCase);
				}
			}

			// currentCaseButtonPressed
			LevelModeButtons.instance.SetCurrentCaseButtonState(LevelCaseState.Active);

			LevelModeController.instance.SwitchToCaseMode();

			// Call every implemented event
			foreach (IPMCaseSwitched ev in UISingleton.FindInterfaces<IPMCaseSwitched>())
			{
				ev.OnPMCaseSwitched(currentCase);
			}
		}

		public void RunCase(int caseNumber)
		{
			isCasesRunning = true;

			CaseFlash.instance.HideFlash();
			if (numberOfCases > 1)
			{
				CaseFlash.instance.ShowNewCaseFlash(currentCase, true);
			}
			else
			{
				PMWrapper.StartCompiler();
			}
		}

		public void CaseCompleted()
		{
			PMWrapper.StopCompiler();

			Main.instance.StartCoroutine(ShowFeedbackAndRunNextCase());
			LevelModeButtons.instance.SetCurrentCaseButtonState(LevelCaseState.Completed);
		}

		public void CaseFailed()
		{
			isCasesRunning = false;
			LevelModeButtons.instance.SetCurrentCaseButtonState(LevelCaseState.Failed);
		}

		IEnumerator ShowFeedbackAndRunNextCase()
		{
			string positiveMassage;
			if (numberOfCases == 1)
			{
				positiveMassage = "Bra jobbat!";
			}
			else
			{
				positiveMassage = "Test " + (currentCase + 1) + " avklarat!";
			}

			UISingleton.instance.taskDescription.ShowPositiveMessage(positiveMassage);

			yield return new WaitForSeconds(3 * (1 - PMWrapper.speedMultiplier));

			UISingleton.instance.answerBubble.HideMessage();
			UISingleton.instance.taskDescription.HideTaskFeedback();

			if (autoContinueTest)
			{
				currentCase++;

				if (currentCase >= numberOfCases)
				{
					isCasesRunning = false;
					allCasesCompleted = true;
					PMWrapper.SetLevelCompleted();
					yield break;
				}

				SetCurrentCase(currentCase);
				RunCase(currentCase);
			}
		}
	}
}
