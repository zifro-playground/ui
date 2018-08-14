using System;
using UnityEngine;


namespace PM
{
	public class LevelModeController : MonoBehaviour, IPMCompilerStopped, IPMCaseSwitched, IPMCompilerStarted
	{
		public GameObject CorrectProgramPanel;

		[HideInInspector]
		public LevelMode LevelMode;

		public static LevelModeController Instance;

		private void Awake()
		{
			if (Instance == null)
				Instance = this;
		}

		public void RunProgram()
		{
			if (Main.Instance.LevelData.sandbox != null)
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
			LevelMode = LevelMode.Sandbox;
			Main.Instance.SetSandboxSettings();

			LevelModeButtons.Instance.SetSandboxButtonState(LevelModeButtonState.Active);
			LevelModeButtons.Instance.SetCaseButtonsToDefault();

			foreach (var ev in UISingleton.FindInterfaces<IPMSwitchedToSandbox>())
				ev.OnPMSwitchedToSandbox();
		}

		public void InitCaseMode()
		{
			LevelMode = LevelMode.Case;
			LevelModeButtons.Instance.SetSandboxButtonToDefault();
			Main.Instance.CaseHandler.SetCurrentCase(0);
		}

		public void StartCorrection()
		{
			CorrectProgramPanel.SetActive(false);
			InitCaseMode();
			Main.Instance.CaseHandler.RunCase(0);
		}

		public void OnPMCompilerStopped(HelloCompiler.StopStatus status)
		{
			if (status == HelloCompiler.StopStatus.Finished)
			{
				if (LevelMode == LevelMode.Sandbox && Main.Instance.LevelData.cases != null && Main.Instance.LevelData.cases.Count > 0)
					CorrectProgramPanel.SetActive(true);
				else if (LevelMode == LevelMode.Case)
					foreach (var ev in UISingleton.FindInterfaces<IPMTimeToCorrectCase>())
						ev.OnPMTimeToCorrectCase();
				else
					PMWrapper.SetLevelCompleted();
			}

			if (status == HelloCompiler.StopStatus.RuntimeError)
			{
				if (LevelMode == LevelMode.Case)
					Main.Instance.CaseHandler.CaseFailed();
			}

			if (status == HelloCompiler.StopStatus.UserForced)
			{
				if (LevelMode == LevelMode.Case)
					Main.Instance.CaseHandler.IsCasesRunning = false;
			}
		}

		public void OnPMCaseSwitched(int caseNumber)
		{
			CorrectProgramPanel.SetActive(false);
		}

		public void OnPMCompilerStarted()
		{
			CorrectProgramPanel.SetActive(false);
		}
	}

	public enum LevelMode
	{
		Sandbox,
		Case
	}
}