using System;
using System.Collections;
using System.Collections.Generic;
using PM;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LevelModeButton : MonoBehaviour
{
	[Header("Button states")]
	[FormerlySerializedAs("Default")]
	public Sprite fallback;

	[FormerlySerializedAs("Active")]
	public Sprite active;

	[FormerlySerializedAs("Completed")]
	public Sprite completed;

	[FormerlySerializedAs("Failed")]
	public Sprite failed;

	[FormerlySerializedAs("Image")]
	public Image image;

	[NonSerialized]
	public LevelModeButtonState currentState = LevelModeButtonState.Default;

	public void SetButtonDefault()
	{
		currentState = LevelModeButtonState.Default;
		image.sprite = fallback;
	}

	public void SetButtonActive()
	{
		currentState = LevelModeButtonState.Active;
		image.sprite = active;
	}

	public void SetButtonCompleted()
	{
		currentState = LevelModeButtonState.Completed;
		image.sprite = completed;
	}

	public void SetButtonFailed()
	{
		currentState = LevelModeButtonState.Failed;
		image.sprite = failed;
	}

	public void SwitchToCase(int caseNumber)
	{
		if (PMWrapper.isCompilerRunning || PMWrapper.isCompilerUserPaused || PMWrapper.isCasesRunning)
		{
			return;
		}

		LevelModeButtons.instance.SetSandboxButtonToDefault();
		SetButtonActive();
		PMWrapper.SwitchCase(caseNumber);
	}

	public void SwitchToSandbox()
	{
		if (PMWrapper.isCompilerRunning || PMWrapper.isCompilerUserPaused || PMWrapper.isCasesRunning)
		{
			return;
		}

		LevelModeController.instance.InitSandboxMode();
		SetButtonActive();
	}
}
