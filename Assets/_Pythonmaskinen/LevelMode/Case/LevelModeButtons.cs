using System;
using System.Collections.Generic;
using UnityEngine;


namespace PM
{
	public class LevelModeButtons : MonoBehaviour
	{
		public GameObject SandboxButton;
		public List<GameObject> CaseButtons;

		public static LevelModeButtons Instance;

		private void Awake()
		{
			if (Instance == null)
				Instance = this;
		}

		public void CreateButtons()
		{
			if (Main.Instance.LevelData.sandbox != null)
				SandboxButton.SetActive(true);
			else
				SandboxButton.SetActive(false);

			SetCaseButtonsToDefault();
		}

		public void SetCurrentCaseButtonState(LevelModeButtonState state)
		{
			var caseNumber = Main.Instance.CaseHandler.CurrentCase;
			
			if (state == LevelModeButtonState.Default)
				CaseButtons[caseNumber].GetComponent<CaseButton>().SetButtonDefault();
			else if (state == LevelModeButtonState.Active)
				CaseButtons[caseNumber].GetComponent<CaseButton>().SetButtonActive();
			else if (state == LevelModeButtonState.Completed)
				CaseButtons[caseNumber].GetComponent<CaseButton>().SetButtonCompleted();
			else if (state == LevelModeButtonState.Failed)
				CaseButtons[caseNumber].GetComponent<CaseButton>().SetButtonFailed();
		}

		public void SetCaseButtonsToDefault()
		{
			int numberOfCases;
			if (Main.Instance.LevelData.cases != null)
				numberOfCases = Main.Instance.LevelData.cases.Count;
			else
				numberOfCases = 1;
			
			for (int i = 0; i < CaseButtons.Count; i++)
			{
				// Don't show buttons if there is only one case except if there is a sandbox before
				if (i < numberOfCases && (numberOfCases > 1 || SandboxButton.activeInHierarchy))
				{
					CaseButtons[i].SetActive(true);
					CaseButtons[i].GetComponent<CaseButton>().SetButtonDefault();
				}
				else
				{
					CaseButtons[i].SetActive(false);
				}
			}
		}

		public void SetSandboxButtonToDefault()
		{
			SandboxButton.GetComponent<CaseButton>().SetButtonDefault();
		}

		public void SetSandboxButtonState(LevelModeButtonState state)
		{
			if (state == LevelModeButtonState.Default)
				SandboxButton.GetComponent<CaseButton>().SetButtonDefault();
			else if (state == LevelModeButtonState.Active)
				SandboxButton.GetComponent<CaseButton>().SetButtonActive();
			else if (state == LevelModeButtonState.Completed)
				SandboxButton.GetComponent<CaseButton>().SetButtonCompleted();
			else if (state == LevelModeButtonState.Failed)
				SandboxButton.GetComponent<CaseButton>().SetButtonFailed();
		}
	}

	public enum LevelModeButtonState
	{
		Default,
		Active,
		Completed,
		Failed
	}
}
