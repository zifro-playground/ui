using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PM;

/// <summary>
/// Executes <see cref="OnPMCompilerStopped"/> when the compiler is told to stop.
/// Either by finishing the code, crashing from python error, or just when the stop button is pressed while playing.
/// </summary>
public interface IPMCompilerStopped {
	void OnPMCompilerStopped(HelloCompiler.StopStatus status);
}
/// <summary>
/// Executes <see cref="OnPMCompilerStarted"/> when the compiler is told to start.
/// Only happens when the start button is pressed.
/// </summary>
public interface IPMCompilerStarted {
	void OnPMCompilerStarted();
}
/// <summary>
/// Executes <see cref="OnPMCompilerUserUnpaused"/> when the compiler is unpaused by the user.
/// Only happens when the pause button is pressed.
/// </summary>
public interface IPMCompilerUserUnpaused {
	void OnPMCompilerUserUnpaused();
}
/// <summary>
/// Executes <see cref="OnPMCompilerUserPaused"/> when the compiler is paused by the user.
/// Only happens when the pause button is pressed.
/// </summary>
public interface IPMCompilerUserPaused {
	void OnPMCompilerUserPaused();
}
/// <summary>
/// Executes <see cref="OnPMSpeedChanged(float)"/> when the speed slider is changed.
/// <para>See also: <seealso cref="PMWrapper.speedMultiplier"/></para>
/// </summary>
public interface IPMSpeedChanged {
	void OnPMSpeedChanged(float speed);
}
/// <summary>
/// Executes <see cref="OnPMLevelChanged()"/> when the current level in the UI is changed.
/// <para>See also: <seealso cref="PMWrapper.currentLevel"/> and <seealso cref="PMWrapper.previousLevel"/></para>
/// </summary>
public interface IPMLevelChanged {
	void OnPMLevelChanged();
}
/// <summary>
/// Executes <see cref="OnPMLevelCompleted()"/> when the current level in the UI is marked as complete by the <seealso cref="PMWrapper.SetLevelCompleted"/> function.
/// <para>See also: <seealso cref="PMWrapper.currentLevel"/> and <seealso cref="PMWrapper.previousLevel"/></para>
/// </summary>
public interface IPMLevelCompleted {
	void OnPMLevelCompleted();
}

/// <summary>
/// Executes <see cref="OnPMCaseSwitched()"/> when the current case is switched.
/// </summary>
public interface IPMCaseSwitched {
	void OnPMCaseSwitched(int caseNumber);
}

/// <summary>
/// Executes <see cref="OnPMWrongAnswer()"/> when wrong answer is passed to the AnswerFunction.
/// </summary>
public interface IPMWrongAnswer
{
	void OnPMWrongAnswer(string answer);
}

/// <summary>
/// Executes <see cref="OnPMCorrectAnswer()"/> when correct answer is passed to the AnswerFunction.
/// </summary>
public interface IPMCorrectAnswer
{
	void OnPMCorrectAnswer(string answer);
}