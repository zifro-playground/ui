using PM;
using System;
using System.Collections.Generic;
using System.Linq;
using Mellis.Core.Interfaces;
using UnityEngine;

/// <summary>
/// PMWrapper, short for "Python Machine Wrapper".
/// <para>This class contains lots of static methods and properties for hooking into the "PythonMachine" without editing the source.</para>
/// <para>Except this class you may also use the interface events. All events interfaces starts with the prefix IPM, for example <seealso cref="IPMCompilerStarted"/></para>
/// </summary>
public static class PMWrapper
{
	/// <summary>
	/// Tells which mode the level is currently running. See <see cref="PM.LevelMode"/> for available modes.
	/// </summary>
	public static LevelMode levelMode => LevelModeController.instance.levelMode;

	/// <summary>
	/// Value from the speed slider. Ranges from 0 to 1, with a default of 0.5.
	/// <para>To get instant updates from the speed change, use <see cref="IPMSpeedChanged"/>.</para>
	/// </summary>
	public static float speedMultiplier
	{
		get => UISingleton.instance.speed.currentSpeed;
		set => UISingleton.instance.speed.currentSpeed = value;
	}

	/// <summary>
	/// A base factor in how long it takes for the walker to take one step in the code. 
	/// It is multiplied with <see cref="speedMultiplier"/> to calculate the actual step time.
	/// </summary>
	public static float walkerStepTime
	{
		get => UISingleton.instance.walker.sleepTime;
		set => UISingleton.instance.walker.sleepTime = value;
	}

	public static Level currentLevel => Main.instance.levelDefinition;

	/// <summary>
	/// The pre code, i.e. the un-changeable code BEFORE the main code.
	/// <para>Changing this value will automatically offset the main codes Y position.</para>
	/// </summary>
	public static string preCode
	{
		get => UISingleton.instance.textField.preCode;
		set
		{
			UISingleton.instance.textField.preCode = value;
			UISingleton.instance.textField.InitTextFields();
			UISingleton.instance.textField.ColorCodeDaCode();
		}
	}

	/// <summary>
	/// The main code, i.e. the code the user is able to change.
	/// Color coding is automatically applied if <seealso cref="IDETextField"/> is disabled.
	/// <para>Changing this value while the user is coding may lead to unexpected glitches. Be careful!</para>
	/// </summary>
	public static string mainCode
	{
		get => UISingleton.instance.textField.theInputField.text;
		set => UISingleton.instance.textField.InsertMainCode(value);
	}

	/// <summary>
	/// The post code, i.e. the un-changeable code AFTER the main code.
	/// <para>At the moment post code is not available and will throw an <seealso cref="NotImplementedException"/>.</para>
	/// </summary>
	/// <exception cref="NotImplementedException">IDE doesn't have full support for post code yet.</exception>
	public static string postCode
	{
		get => throw new NotImplementedException("IDE doesn't have full support for post code yet!");
		set => throw new NotImplementedException("IDE doesn't have full support for post code yet!");
	}

	/// <summary>
	/// All codes combined, i.e. <see cref="preCode"/> + <see cref="mainCode"/> + <see cref="postCode"/> (with linebreaks inbetween).
	/// <para>This is the property that <seealso cref="CodeWalker"/> uses when sending the code to compile to the <seealso cref="Compiler.SyntaxCheck"/>.</para>
	/// </summary>
	public static string fullCode => preCode.Length > 0 ? preCode + '\n' + mainCode : mainCode;

	/// <summary>
	/// Replacement for <see cref="IDETextField.amountOfRows"/>. Defines how many lines of code the user is allowed to enter.
	/// <para>Setting a value lower than the current number of rows user has typed will result in buggy behaviour. Be aware!</para>
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when setting this to any non-positive values.</exception>
	public static int codeRowsLimit
	{
		get => UISingleton.instance.textField.codeRowsLimit;
		set
		{
			if (value > 0)
			{
				UISingleton.instance.textField.rowLimit = value;
			}
			else
			{
				throw new ArgumentOutOfRangeException("codeRowsLimit", value,
					"Zero and negative values are not accepted!");
			}
		}
	}

	/// <summary>
	/// Adds code to the main code where the cursor currently is. Typically used by smart buttons.
	/// </summary>
	public static void AddCode(string code, bool smartButtony = false)
	{
		UISingleton.instance.textField.InsertText(code, smartButtony);
	}

	/// <summary>
	/// Adds code to the main code only if the main code is empty. Should be used for setting given code first time the user opens the level.
	/// </summary>
	public static void AddCodeAtStart(string code)
	{
		UISingleton.instance.textField.InsertMainCodeAtStart(code);
	}

	public static int currentLineNumber => UISingleton.instance.walker.currentLineNumber;

	/// <summary>
	/// Scrolls so that the <paramref name="lineNumber"/> is visible in the IDE.
	/// </summary>
	public static void FocusOnLineNumber(int lineNumber)
	{
		UISingleton.instance.textField.theScrollLord.FocusOnLineNumber(lineNumber);
	}

	/// <summary>
	/// Scrolls so that the current selected line is visible in the IDE.
	/// </summary>
	public static void FocusOnLineNumber()
	{
		UISingleton.instance.textField.FocusCursor();
	}

	/// <summary>
	/// Boolean representing wether cases is currently running or not.
	/// </summary>
	public static bool isCasesRunning => Main.instance.caseHandler.isCasesRunning;

	/// <summary>
	/// Boolean representing wether the compiler is currently executing or not.
	/// </summary>
	public static bool isCompilerRunning => UISingleton.instance.walker.isWalkerRunning;

	/// <summary>
	/// Boolean representing wether the walker is currently paused by the user (via pressing the pause button).
	/// </summary>
	public static bool isCompilerUserPaused => UISingleton.instance.walker.isUserPaused;

	/// <summary>
	/// Starts the compiler if it's not currently running. Static wrapper for <see cref="HelloCompiler.CompileCode"/>
	/// </summary>
	public static void StartCompiler()
	{
		UISingleton.instance.walker.CompileFullCode();
	}

	/// <summary>
	/// Starts case 0 and starts compiler. Will run all test cases if possible
	/// </summary>
	public static void RunCode()
	{
		LevelModeController.instance.RunProgram();
	}

	/// <summary>
	/// Stops the compiler if it's currently running. Static wrapper for <see cref="HelloCompiler.StopCompiler(StopStatus)"/> with the argument <seealso cref="StopStatus.CodeForced"/>
	/// </summary>
	public static void StopCompiler()
	{
		UISingleton.instance.walker.StopCompiler(StopStatus.CodeForced);
	}

	/// <summary>
	/// Sets the available functions of type <see cref="IClrFunction"/> or <see cref="IClrYieldingFunction"/> in the compiler.
	/// </summary>
	public static void SetCompilerFunctions(List<IEmbeddedType> functions)
	{
		SetSmartButtons(functions.Select(function => function.FunctionName + "()").ToList());
		UISingleton.instance.walker.addedFunctions.Clear();
		UISingleton.instance.walker.addedFunctions.AddRange(functions);
	}

	/// <summary>
	/// Sets the available functions of type <see cref="IClrFunction"/> or <see cref="IClrYieldingFunction"/> in the compiler.
	/// </summary>
	public static void SetCompilerFunctions(params IEmbeddedType[] functions)
	{
		SetSmartButtons(functions.Select(function => function.FunctionName + "()").ToList());
		UISingleton.instance.walker.addedFunctions.Clear();
		UISingleton.instance.walker.addedFunctions.AddRange(functions);
	}

	/// <summary>
	/// Adds to the available list of functions of type <see cref="IClrFunction"/> or <see cref="IClrYieldingFunction"/> to the already existing list of compiler functions.
	/// </summary>
	public static void AddCompilerFunctions(List<IEmbeddedType> functions)
	{
		AddSmartButtons(functions.Select(function => function.FunctionName + "()").ToList());
		UISingleton.instance.walker.addedFunctions.AddRange(functions);
	}

	/// <summary>
	/// Adds to the available list of functions of type <see cref="IClrFunction"/> or <see cref="IClrYieldingFunction"/> to the already existing list of compiler functions.
	/// </summary>
	public static void AddCompilerFunctions(params IEmbeddedType[] functions)
	{
		AddSmartButtons(functions.Select(function => function.FunctionName + "()").ToList());
		UISingleton.instance.walker.addedFunctions.AddRange(functions);
	}

	/// <summary>
	/// Used to resolve a yielded function <see cref="IClrYieldingFunction"/>
	/// and uses the compilers NULL representation <see cref="IScriptTypeFactory.Null"/> as return value.
	/// <para>Replacement for <seealso cref="UnpauseWalker"/></para>
	/// </summary>
	public static void ResolveYield()
	{
		UISingleton.instance.walker.ResolveYield();
	}

	/// <summary>
	/// Used to resolve a yielded function <see cref="IClrYieldingFunction"/>
	/// and uses parameter <paramref name="returnValue"/> as return value.
	/// <para>Replacement for <seealso cref="UnpauseWalker"/></para>
	/// </summary>
	public static void ResolveYield(IScriptType returnValue)
	{
		UISingleton.instance.walker.ResolveYield(returnValue);
	}

	/// <summary>
	/// Used to resolve a yielded function <see cref="IClrYieldingFunction"/>
	/// and uses the current <seealso cref="IScriptTypeFactory"/> to create
	/// a return value of type <see cref="bool"/>.
	/// <para>Replacement for <seealso cref="UnpauseWalker"/></para>
	/// </summary>
	public static void ResolveYield(bool returnBool)
	{
		UISingleton.instance.walker.ResolveYield(returnBool);
	}

	/// <summary>
	/// Used to resolve a yielded function <see cref="IClrYieldingFunction"/>
	/// and uses the current <seealso cref="IScriptTypeFactory"/> to create
	/// a return value of type <see cref="int"/>.
	/// <para>Replacement for <seealso cref="UnpauseWalker"/></para>
	/// </summary>
	public static void ResolveYield(int returnInt)
	{
		UISingleton.instance.walker.ResolveYield(returnInt);
	}

	/// <summary>
	/// Used to resolve a yielded function <see cref="IClrYieldingFunction"/>
	/// and uses the current <seealso cref="IScriptTypeFactory"/> to create
	/// a return value of type <see cref="double"/>.
	/// <para>Replacement for <seealso cref="UnpauseWalker"/></para>
	/// </summary>
	public static void ResolveYield(double returnDouble)
	{
		UISingleton.instance.walker.ResolveYield(returnDouble);
	}

	/// <summary>
	/// Used to resolve a yielded function <see cref="IClrYieldingFunction"/>
	/// and uses the current <seealso cref="IScriptTypeFactory"/> to create
	/// a return value of type <see cref="string"/>.
	/// <para>Replacement for <seealso cref="UnpauseWalker"/></para>
	/// </summary>
	public static void ResolveYield(string returnString)
	{
		UISingleton.instance.walker.ResolveYield(returnString);
	}

	/// <summary>
	/// Set smart buttons from parameters.
	/// </summary>
	public static void SetSmartButtons(params string[] codes)
	{
		UISingleton.instance.smartButtons.ClearSmartButtons();
		for (int i = 0; i < codes.Length; i++)
		{
			UISingleton.instance.smartButtons.AddSmartButton(codes[i], codes[i]);
		}
	}

	/// <summary>
	/// Set smart buttons from list.
	/// </summary>
	public static void SetSmartButtons(List<string> buttonTexts)
	{
		UISingleton.instance.smartButtons.ClearSmartButtons();
		for (int i = 0; i < buttonTexts.Count; i++)
		{
			UISingleton.instance.smartButtons.AddSmartButton(buttonTexts[i], buttonTexts[i]);
		}
	}

	/// <summary>
	/// Add one smart button below code window.
	/// <para>Text on button</para>
	/// </summary>
	public static void AddSmartButton(string buttonText)
	{
		UISingleton.instance.smartButtons.AddSmartButton(buttonText, buttonText);
	}

	/// <summary>
	/// Add one smart button below code window.
	/// <para>Text on button</para>
	/// </summary>
	public static void AddSmartButtons(List<string> buttonTexts)
	{
		foreach (string str in buttonTexts)
		{
			UISingleton.instance.smartButtons.AddSmartButton(str, str);
		}
	}

	/// <summary>
	/// Sets smart buttons with names taken from registered functions via <see cref="SetCompilerFunctions(IEmbeddedType[])"/> or <see cref="AddCompilerFunctions(IEmbeddedType[])"/>
	/// </summary>
	public static void AutoSetSmartButtons()
	{
		SetSmartButtons(UISingleton.instance.walker.addedFunctions.ConvertAll(f => f.FunctionName + "()"));
	}

	/// <summary>
	/// Set the task description for current level. If passed empty string, both placeholders for task description will be deactivated.
	/// </summary>
	public static void SetTaskDescription(string header, string body)
	{
		UISingleton.instance.taskDescription.SetTaskDescription(header, body);
	}

	/// <summary>
	/// Set the correct answeres for the current case.
	/// </summary>
	public static void SetCaseAnswer<T>(params T[] answer)
	{
		Main.instance.levelAnswer = new LevelAnswer<T>(answer);
	}

	/// <summary>
	/// Set the correct answeres for the current case.
	/// </summary>
	public static void SetCaseAnswer(params int[] intAnswers)
	{
		Main.instance.levelAnswer = new LevelAnswer<int>(intAnswers);
	}

	/// <summary>
	/// Set the correct answeres for the current case.
	/// </summary>
	public static void SetCaseAnswer(params string[] stringAnswers)
	{
		Main.instance.levelAnswer = new LevelAnswer<string>(stringAnswers);
	}

	/// <summary>
	/// Set the correct answeres for the current case.
	/// </summary>
	public static void SetCaseAnswer(params bool[] boolAnswers)
	{
		Main.instance.levelAnswer = new LevelAnswer<bool>(boolAnswers);
	}

	/// <summary>
	/// Represents the current level. Setting this value will automatically setoff <see cref="IPMLevelChanged.OnPMLevelChanged(int)"/>
	/// <para>If set to a value higher than highest unlocked level then <seealso cref="unlockedLevel"/> will also be set to the same value.</para>
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if set to value outside of levels list index range, i.e. thrown if <seealso cref="currentLevelIndex"/>.set &lt; 0 or ≥ <seealso cref="numOfLevels"/></exception>
	public static int currentLevelIndex
	{
		get => UISingleton.instance.levelbar.current;
		set
		{
			if (value < 0 || value >= numOfLevels)
			{
				throw new ArgumentOutOfRangeException(nameof(currentLevelIndex), value, "Level index out of range!");
			}

			UISingleton.instance.levelbar.ChangeLevel(value);
		}
	}

	/// <summary>
	/// This value determites how many levels are shown on the levelbar in the UI.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">In the case of non-positive values in setting <see cref="numOfLevels"/>.</exception>
	public static int numOfLevels
	{
		get => UISingleton.instance.levelbar.numberOfLevels;
		set
		{
			if (value > 0)
			{
				UISingleton.instance.levelbar.RecreateButtons(value, Mathf.Clamp(currentLevelIndex, 0, value - 1),
					unlockedLevel);
			}
			else
			{
				throw new ArgumentOutOfRangeException(nameof(numOfLevels), value,
					"Zero and negative values are not accepted!");
			}
		}
	}

	/// <summary>
	/// Returns true if current level has defined Answer and the user is supposed to answer level.
	/// </summary>
	public static bool levelShouldBeAnswered
		=> UISingleton.instance.walker.addedFunctions.OfType<Answer>().Any();

	/// <summary>
	/// The highest level that's unlocked. Value of 0 means only first level is unlocked. Value of (<seealso cref="numOfLevels"/> - 1) means last level is unlocked, i.e. all levels.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">In the case of invalid values in setting <see cref="unlockedLevel"/></exception>
	public static int unlockedLevel
	{
		get => UISingleton.instance.levelbar.unlocked;
		set
		{
			if (value >= 0 && value < numOfLevels)
			{
				UISingleton.instance.levelbar.UpdateButtons(currentLevelIndex, value);
			}
			else
			{
				throw new ArgumentOutOfRangeException(nameof(unlockedLevel), value,
					"Level value is out of range of existing levels.");
			}
		}
	}

	/// <summary>
	/// Gets the index of the current case. Index starts from 0.
	/// </summary>
	public static int currentCase => Main.instance.caseHandler.currentCase;

	/// <summary>
	/// Gets the state of the current case.
	/// </summary>
	public static LevelCaseState currentCaseState => LevelModeButtons.instance.caseButtons[currentCase].currentState;

	/// <summary>
	/// Stops the compiler, shows the "Level complete!" popup, marks the current level as complete and unlocks the next level.
	/// </summary>
	public static void SetLevelCompleted()
	{
		UISingleton.instance.winScreen.SetLevelCompleted();
	}

	/// <summary>
	/// Set case completed if test case is passed. Marks current case as completed and starts next test case.
	/// </summary>
	public static void SetCaseCompleted()
	{
		Main.instance.caseHandler.CaseCompleted();
	}

	/// <summary>
	/// Switches to chosen case.
	/// </summary>
	/// <param name="caseNumber">The case number to switch to.</param> 
	public static void SwitchCase(int caseNumber)
	{
		Main.instance.caseHandler.SetCurrentCase(caseNumber);
	}

	/// <summary>
	/// Jumps to the last level. <paramref name="ignoreUnlocked"/> determines if it should respect or ignore <seealso cref="unlockedLevel"/>.
	/// </summary>
	/// <param name="ignoreUnlocked">If true, it will jump to the absolute last level. If false, it will jump to the last unlocked level.</param>
	public static void JumpToLastLevel(bool ignoreUnlocked = false)
	{
		currentLevelIndex = ignoreUnlocked ? numOfLevels - 1 : unlockedLevel;
	}

	/// <summary>
	/// Jumps to the first level.
	/// </summary>
	public static void JumpToFirstLevel()
	{
		currentLevelIndex = 0;
	}

	/// <summary>
	/// Stops the compiler and shows a dialog box containing the error message on the current line <see cref="currentLineNumber"/>.
	/// </summary>
	/// <exception cref="PMRuntimeException">Is always thrown</exception>
	public static void RaiseError(string message)
	{
		UISingleton.instance.textField.theLineMarker.SetErrorMarker(currentLineNumber, message);
	}

	/// <summary>
	/// Stops the compiler and shows a dialog box containing the error message on the target <paramref name="newLineNumber"/>.
	/// </summary>
	/// <exception cref="PMRuntimeException">Is always thrown</exception>
	public static void RaiseError(int newLineNumber, string message)
	{
		UISingleton.instance.textField.theLineMarker.SetErrorMarker(newLineNumber, message);
	}

	/// <summary>
	/// Stops the compiler and shows a dialog box containing the error message on the target <see cref="UnityEngine.UI.Selectable"/>.
	/// </summary>
	/// <exception cref="PMRuntimeException">Is always thrown</exception>
	public static void RaiseError(UnityEngine.UI.Selectable targetSelectable, string message)
	{
		UISingleton.instance.textField.theLineMarker.SetErrorMarker(targetSelectable, message);
	}

	/// <summary>
	/// Stops the compiler and shows a dialog box containing the error message on the target <see cref="RectTransform"/>.
	/// </summary>
	/// <exception cref="PMRuntimeException">Is always thrown</exception>
	public static void RaiseError(RectTransform targetRectTransform, string message)
	{
		UISingleton.instance.textField.theLineMarker.SetErrorMarker(targetRectTransform, message);
	}

	/// <summary>
	/// Stops the compiler and shows a dialog box containing the error message on the target canvas position.
	/// </summary>
	/// <exception cref="PMRuntimeException">Is always thrown</exception>
	public static void RaiseError(Vector2 targetCanvasPosition, string message)
	{
		UISingleton.instance.textField.theLineMarker.SetErrorMarker(targetCanvasPosition, message);
	}

	/// <summary>
	/// Stops the compiler and shows a dialog box containing the error message on the target world position.
	/// <para>NOTE: The game camera must be marked with the "Main Camera" tag for this to work.</para>
	/// </summary>
	/// <exception cref="PMRuntimeException">Is always thrown</exception>
	public static void RaiseError(Vector3 targetWorldPosition, string message)
	{
		UISingleton.instance.textField.theLineMarker.SetErrorMarker(targetWorldPosition, message);
	}

	/// <summary>
	/// Stops compiler and shows feedback dialog from robot.
	/// </summary>
	public static void RaiseTaskError(string message)
	{
		Debug.LogWarningFormat("RaiseTaskError: {0}", message);
		UISingleton.instance.taskDescription.ShowTaskError(message);
		Main.instance.caseHandler.CaseFailed();
		UISingleton.instance.walker.StopCompiler(StopStatus.TaskError);
	}

	/// <summary>
	/// Makes the IDE not destroy on load, i.e. on level change and such.
	/// <para>Equal to <see cref="UnityEngine.Object.DontDestroyOnLoad(UnityEngine.Object)"/> but on the IDE.</para>
	/// </summary>
	public static void DontDestroyIDEOnLoad()
	{
		if (UISingleton.instance.ideRoot.parent != null)
		{
			UISingleton.instance.ideRoot.parent = null;
		}

		UnityEngine.Object.DontDestroyOnLoad(UISingleton.instance.ideRoot);
	}

	#region OBSOLETE

#pragma warning disable IDE1006 // Naming Styles
	// ReSharper disable UnusedMember.Global
	// ReSharper disable IdentifierTypo

	/// <summary>
	/// Unpauses the walker from a pausing function.
	/// </summary>
	[Obsolete("Renamed to match new terminology in the code base. Use PMWrapper.ResolveYield() instead.")]
	public static void UnpauseWalker()
	{
		ResolveYield();
	}

	/// <inheritdoc cref="levelMode"/>
	[Obsolete("Renamed due to code style. Use PMWrapper.levelMode instead.")]
	public static LevelMode LevelMode => levelMode;

	/// <inheritdoc cref="currentLevel"/>
	[Obsolete("Renamed due to code style. Use PMWrapper.currentLevel instead.")]
	public static Level CurrentLevel => currentLevel;

	/// <inheritdoc cref="isCasesRunning"/>
	[Obsolete("Renamed due to code style. Use PMWrapper.isCasesRunning instead.")]
	public static bool IsCasesRunning => isCasesRunning;

	/// <inheritdoc cref="isCompilerRunning"/>
	[Obsolete("Renamed due to code style. Use PMWrapper.isCompilerRunning instead.")]
	public static bool IsCompilerRunning => isCompilerRunning;

	/// <inheritdoc cref="isCompilerUserPaused"/>
	[Obsolete("Renamed due to code style. Use PMWrapper.isCompilerUserPaused instead.")]
	public static bool IsCompilerUserPaused => isCompilerUserPaused;

	/// <summary>
	/// Sets the compiler functions available for the user.
	/// </summary>
	[Obsolete(
		"New compiler. Use SetCompilerFunctions(List<IEmbeddedValue>) with " +
		nameof(IClrFunction) + " or " +
		nameof(IClrYieldingFunction) + " instead.",
		error: true)]
	public static void SetCompilerFunctions<T>(List<T> functions)
	{
	}

	/// <summary>
	/// Adds a list of functions to the already existing list of compiler functions.
	/// </summary>
	[Obsolete(
		"New compiler. Use AddCompilerFunctions(List<IEmbeddedValue>) with " +
		nameof(IClrFunction) + " or " +
		nameof(IClrYieldingFunction) + " instead.",
		error: true)]
	public static void AddCompilerFunctions<T>(List<T> functions)
	{
	}

	/// <summary>
	/// Adds all parameters of type <see cref="Compiler.Function"/> to the already existing list of compiler functions.
	/// </summary>
	[Obsolete(
		"New compiler. Use AddCompilerFunctions(params IEmbeddedValue[]) with " +
		nameof(IClrFunction) + " or " +
		nameof(IClrYieldingFunction) + " instead.",
		error: true)]
	public static void AddCompilerFunctions<T>(params T[] functions)
	{
	}

	// ReSharper restore IdentifierTypo
	// ReSharper restore UnusedMember.Global
#pragma warning restore IDE1006 // Naming Styles

	#endregion
}
