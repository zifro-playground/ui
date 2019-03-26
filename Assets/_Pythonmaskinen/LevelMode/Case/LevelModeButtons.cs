using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace PM
{
	public class LevelModeButtons : MonoBehaviour
	{
		[FormerlySerializedAs("SandboxButton")]
		public GameObject sandboxButton;

		[FormerlySerializedAs("CaseButtons")]
		public List<GameObject> caseButtons;

		public static LevelModeButtons instance;

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
			}
		}

		public void CreateButtons()
		{
			if (Main.instance.levelDefinition.sandbox != null)
			{
				sandboxButton.SetActive(true);
			}
			else
			{
				sandboxButton.SetActive(false);
			}

			SetCaseButtonsToDefault();
		}

		public void SetCurrentCaseButtonState(LevelModeButtonState state)
		{
			int caseNumber = Main.instance.caseHandler.currentCase;

			if (state == LevelModeButtonState.Default)
			{
				caseButtons[caseNumber].GetComponent<LevelModeButton>().SetButtonDefault();
			}
			else if (state == LevelModeButtonState.Active)
			{
				caseButtons[caseNumber].GetComponent<LevelModeButton>().SetButtonActive();
			}
			else if (state == LevelModeButtonState.Completed)
			{
				caseButtons[caseNumber].GetComponent<LevelModeButton>().SetButtonCompleted();
			}
			else if (state == LevelModeButtonState.Failed)
			{
				caseButtons[caseNumber].GetComponent<LevelModeButton>().SetButtonFailed();
			}
		}

		public void SetCaseButtonsToDefault()
		{
			int numberOfCases = 0;
			if (Main.instance.levelDefinition.cases != null && Main.instance.levelDefinition.cases.Any())
			{
				numberOfCases = Main.instance.levelDefinition.cases.Count;
			}
			else
			{
				if (Main.instance.levelDefinition.sandbox == null)
				{
					numberOfCases = 1;
				}
			}

			for (int i = 0; i < caseButtons.Count; i++)
			{
				// Don't show buttons if there is only one case except if there is a sandbox before
				if (i < numberOfCases && (numberOfCases > 1 || sandboxButton.activeInHierarchy))
				{
					caseButtons[i].SetActive(true);
					caseButtons[i].GetComponent<LevelModeButton>().SetButtonDefault();
				}
				else
				{
					caseButtons[i].SetActive(false);
				}
			}
		}

		public void SetSandboxButtonToDefault()
		{
			sandboxButton.GetComponent<LevelModeButton>().SetButtonDefault();
		}

		public void SetSandboxButtonState(LevelModeButtonState state)
		{
			if (state == LevelModeButtonState.Default)
			{
				sandboxButton.GetComponent<LevelModeButton>().SetButtonDefault();
			}
			else if (state == LevelModeButtonState.Active)
			{
				sandboxButton.GetComponent<LevelModeButton>().SetButtonActive();
			}
			else if (state == LevelModeButtonState.Completed)
			{
				sandboxButton.GetComponent<LevelModeButton>().SetButtonCompleted();
			}
			else if (state == LevelModeButtonState.Failed)
			{
				sandboxButton.GetComponent<LevelModeButton>().SetButtonFailed();
			}
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