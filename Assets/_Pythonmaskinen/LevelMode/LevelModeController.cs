using System;
using System.Linq;
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
			SetSandboxSettings();

			LevelModeButtons.Instance.SetSandboxButtonState(LevelModeButtonState.Active);
			LevelModeButtons.Instance.SetCaseButtonsToDefault();
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

		public void SetSandboxSettings()
		{
			if (Main.Instance.LevelData.sandbox != null)
			{
				var sandboxSettings = Main.Instance.LevelData.sandbox.sandboxSettings;

				if (sandboxSettings == null)
				{
					PMWrapper.preCode = "";
					return;
				}

				if (!String.IsNullOrEmpty(sandboxSettings.precode))
					PMWrapper.preCode = sandboxSettings.precode;

				if (sandboxSettings.walkerStepTime > 0)
					PMWrapper.walkerStepTime = sandboxSettings.walkerStepTime;
			}
			else
			{
				print("Hittar inga settings");
				PMWrapper.preCode = "";
			}
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