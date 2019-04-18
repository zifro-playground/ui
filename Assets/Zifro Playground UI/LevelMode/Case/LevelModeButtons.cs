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
		public LevelModeButton sandboxButton;

		[FormerlySerializedAs("CaseButtons")]
		public List<LevelModeButton> caseButtons;

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
				sandboxButton.gameObject.SetActive(true);
			}
			else
			{
				sandboxButton.gameObject.SetActive(false);
			}

			SetCaseButtonsToDefault();
		}

		public void SetCurrentCaseButtonState(LevelModeButtonState state)
		{
			int caseNumber = Main.instance.caseHandler.currentCase;

			if (state == LevelModeButtonState.Default)
			{
				caseButtons[caseNumber].SetButtonDefault();
			}
			else if (state == LevelModeButtonState.Active)
			{
				caseButtons[caseNumber].SetButtonActive();
			}
			else if (state == LevelModeButtonState.Completed)
			{
				caseButtons[caseNumber].SetButtonCompleted();
			}
			else if (state == LevelModeButtonState.Failed)
			{
				caseButtons[caseNumber].SetButtonFailed();
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
				if (i < numberOfCases && (numberOfCases > 1 || sandboxButton.gameObject.activeSelf))
				{
					caseButtons[i].gameObject.SetActive(true);
					caseButtons[i].SetButtonDefault();
				}
				else
				{
					caseButtons[i].gameObject.SetActive(false);
				}
			}
		}

		public void SetSandboxButtonToDefault()
		{
			sandboxButton.SetButtonDefault();
		}

		public void SetSandboxButtonState(LevelModeButtonState state)
		{
			if (state == LevelModeButtonState.Default)
			{
				sandboxButton.SetButtonDefault();
			}
			else if (state == LevelModeButtonState.Active)
			{
				sandboxButton.SetButtonActive();
			}
			else if (state == LevelModeButtonState.Completed)
			{
				sandboxButton.SetButtonCompleted();
			}
			else if (state == LevelModeButtonState.Failed)
			{
				sandboxButton.SetButtonFailed();
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
