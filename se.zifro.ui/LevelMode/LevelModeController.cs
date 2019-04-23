using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace PM
{
	public class LevelModeController : MonoBehaviour, IPMCompilerStopped, IPMCaseSwitched, IPMCompilerStarted
	{
		[FormerlySerializedAs("CorrectProgramPanel")]
		public GameObject correctProgramPanel;

		[HideInInspector]
		[FormerlySerializedAs("LevelMode")]
		public LevelMode levelMode;

		public static LevelModeController instance;

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
			}
		}

		public void RunProgram()
		{
			if (Main.instance.levelDefinition.sandbox != null)
			{
				InitSandboxMode();
				PMWrapper.StartCompiler();
			}
			else
			{
				StartCorrection();
			}
		}

		public void InitSandboxMode()
		{
			levelMode = LevelMode.Sandbox;
			LevelModeButtons.instance.SetSandboxButtonState(LevelCaseState.Active);
			LevelModeButtons.instance.SetCaseButtonsToDefault();

			Main.instance.SetSettings();

			foreach (IPMSwitchedToSandbox ev in UISingleton.FindInterfaces<IPMSwitchedToSandbox>())
			{
				ev.OnPMSwitchedToSandbox();
			}
		}

		public void InitCaseMode()
		{
			levelMode = LevelMode.Case;
			LevelModeButtons.instance.SetSandboxButtonToDefault();
			Main.instance.caseHandler.SetCurrentCase(0);
		}

		public void SwitchToCaseMode()
		{
			levelMode = LevelMode.Case;
			Main.instance.SetSettings();
		}

		public void StartCorrection()
		{
			correctProgramPanel.SetActive(false);
			InitCaseMode();
			Main.instance.caseHandler.RunCase(0);
		}

		public void OnPMCompilerStopped(StopStatus status)
		{
			if (status == StopStatus.Finished)
			{
				if (levelMode == LevelMode.Sandbox && Main.instance.levelDefinition.cases != null &&
				    Main.instance.levelDefinition.cases.Count > 0)
				{
					correctProgramPanel.SetActive(true);
				}
				else if (levelMode == LevelMode.Case)
				{
					foreach (IPMTimeToCorrectCase ev in UISingleton.FindInterfaces<IPMTimeToCorrectCase>())
					{
						ev.OnPMTimeToCorrectCase();
					}
				}
				else
				{
					PMWrapper.SetLevelCompleted();
				}
			}

			if (status == StopStatus.RuntimeError)
			{
				if (levelMode == LevelMode.Case)
				{
					Main.instance.caseHandler.CaseFailed();
				}
			}

			if (status == StopStatus.UserForced)
			{
				if (levelMode == LevelMode.Case)
				{
					Main.instance.caseHandler.isCasesRunning = false;
				}
			}
		}

		public void OnPMCaseSwitched(int caseNumber)
		{
			correctProgramPanel.SetActive(false);
		}

		public void OnPMCompilerStarted()
		{
			correctProgramPanel.SetActive(false);
		}
	}

	public enum LevelMode
	{
		Sandbox,
		Case
	}
}
