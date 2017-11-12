using PM;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PMWrapper, short for "Python Machine Wrapper".
/// <para>This class contains lots of static methods and properties for hooking into the "PythonMachine" without editing the source.</para>
/// <para>Except this class you may also use the interface events. All events interfaces starts with the prefix IPM, for example <seealso cref="IPMCompilerStarted"/></para>
/// </summary>
public static class PMWrapper {
	/// <summary>
	/// Version of the Pythonmachine wrapper/UI.
	/// </summary>
	public const string version = "1.1.0";

	/// <summary>
	/// Value from the speed slider. Ranges from 0 to 1, with a default of 0.5.
	/// <para>To get instant updates from the speed change, use <see cref="IPMSpeedChanged"/>.</para>
	/// </summary>
	public static float speedMultiplier {
		get { return UISingleton.instance.speed.currentSpeed; }
		set { UISingleton.instance.speed.currentSpeed = value; }
	}

	public static float codewalkerBaseSpeed
	{
		get { return UISingleton.instance.walker.BaseWalkerWaitTime; }
		set { UISingleton.instance.walker.BaseWalkerWaitTime = value; }
	}
	
	/// <summary>
	/// The pre code, i.e. the un-changeable code BEFORE the main code.
	/// <para>Changing this value will automatically offset the main codes Y position.</para>
	/// </summary>
	public static string preCode {
		get { return UISingleton.instance.textField.preCode; }
		set {
			UISingleton.instance.textField.preCode = value;
			UISingleton.instance.textField.InitTextFields();
			UISingleton.instance.textField.ColorCodeDaCode();
		}
	}
	/// <summary>
	/// The main code, i.e. the code the user is able to change.
	/// Colorcoding is automatically applied if <seealso cref="IDETextField"/> is disabled.
	/// <para>Changing this value while the user is coding may lead to unexpected glitches. Be careful!</para>
	/// </summary>
	public static string mainCode {
		get { return UISingleton.instance.textField.theInputField.text; }
		set {
			UISingleton.instance.textField.theInputField.text = value;
			if (!UISingleton.instance.textField.enabled)
				UISingleton.instance.textField.ColorCodeDaCode();
		}
	}
	/// <summary>
	/// The post code, i.e. the un-changeable code AFTER the main code.
	/// <para>At the moment post code is not available and will throw an <seealso cref="NotImplementedException"/>.</para>
	/// </summary>
	/// <exception cref="NotImplementedException">IDE doesn't have full support for post code yet.</exception>
	public static string postCode {
		get { throw new NotImplementedException("IDE doesn't have full support for post code yet!"); }
		set { throw new NotImplementedException("IDE doesn't have full support for post code yet!"); }
	}

	/// <summary>
	/// All codes combined, i.e. <see cref="preCode"/> + <see cref="mainCode"/> + <see cref="postCode"/> (with linebreaks inbetween).
	/// <para>This is the property that <seealso cref="CodeWalker"/> uses when sending the code to compile to the <seealso cref="Compiler.SyntaxCheck"/>.</para>
	/// </summary>
	public static string fullCode {
		get { return (preCode.Length > 0 ? preCode + '\n' + mainCode : mainCode) /*+ '\n' + postCode*/; }
	}

	/// <summary>
	/// Replacement for <see cref="IDETextField.amountOfRows"/>. Defines how many lines of code the user is allowed to enter.
	/// <para>Setting a value lower than the current number of rows user has typed will result in buggy behaviour. Be aware!</para>
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when setting this to any non-positive values.</exception>
	public static int codeRowsLimit {
		get { return UISingleton.instance.textField.codeRowsLimit; }
		set {
			if (value > 0) {
				UISingleton.instance.textField.rowLimit = value;
			}
			else throw new ArgumentOutOfRangeException("codeRowsLimit", value, "Zero and negative values are not accepted!");
		}
	}

	/// <summary>
	/// Adds code to the main code where the cursor currently is. Typically used by smart buttons.
	/// </summary>
	public static void AddCode(string code, bool smartButtony = false) {
		UISingleton.instance.textField.InsertText(code, smartButtony);
	}

	/// <summary>
	/// Adds code to the main code only if the main code is empty. Should be used for setting given code first time the user opens the level.
	/// </summary>
	public static void AddCodeAtStart(string code){
		UISingleton.instance.textField.insertMainCodeAtStart (code);
	}

	/// <summary>
	/// Scrolls so that the <paramref name="lineNumber"/> is visible in the IDE.
	/// </summary>
	public static void FocusOnLineNumber(int lineNumber) {
		UISingleton.instance.textField.theScrollLord.FocusOnLineNumber(lineNumber);
	}

	/// <summary>
	/// Scrolls so that the current selected line is visible in the IDE.
	/// </summary>
	public static void FocusOnLineNumber() {
		UISingleton.instance.textField.FocusCursor();
	}

	/// <summary>
	/// Boolean representing wether the compiler is currently executing or not.
	/// </summary>
	public static bool isCompilerRunning { get { return UISingleton.instance.compiler.isRunning; } }

	/// <summary>
	/// Boolean representing wether the walker is currently paused by the user (via pressing the pause button).
	/// </summary>
	public static bool isCompilerUserPaused { get { return UISingleton.instance.walker.isUserPaused; } }

	/// <summary>
	/// Wether or not the level is playing && is in "demo" mode.
	/// </summary>
	public static bool isDemoingLevel {
		get { return PM.Manus.ManusPlayer.isPlaying; }
	}
	/// <summary>
	/// Wether or not the current level has demo manus.
	/// <para>"One could ask, is general Juhziz in charge?"</para>
	/// </summary>
	public static bool isDemoLevel {
		get { return PMWrapper.currentLevel < PM.Manus.Loader.allManuses.Count && PMWrapper.currentLevel >= 0 && PM.Manus.Loader.allManuses[PMWrapper.currentLevel] != null; }
	}

	/// <summary>
	/// Starts the compiler if it's not currently running. Static wrapper for <see cref="HelloCompiler.compileCode"/>
	/// </summary>
	public static void StartCompiler() {
		UISingleton.instance.compiler.compileCode();
	}

	/// <summary>
	/// Starts case 0 and starts compiler. Will run all test cases if possible
	/// </summary>
	public static void RunCode() {
		UISingleton.instance.levelHandler.currentLevel.caseHandler.ResetHandlerAndButtons ();
		UISingleton.instance.levelHandler.currentLevel.caseHandler.RunCase (0);
	}

	/// <summary>
	/// Stops the compiler if it's currently running. Static wrapper for <see cref="HelloCompiler.stopCompiler(HelloCompiler.StopStatus)"/> with the argument <seealso cref="HelloCompiler.StopStatus.Forced"/>
	/// </summary>
	public static void StopCompiler() {
		UISingleton.instance.compiler.stopCompiler(HelloCompiler.StopStatus.Forced);
	}

	/// <summary>
	/// Sets the compiler functions avalible for the user. Called from levelsettings when new level is loaded and adds functions from levelsettings text file.
	/// </summary>
	public static void SetCompilerFunctions(params Compiler.Function[] functions) {
		UISingleton.instance.compiler.addedFunctions = new List<Compiler.Function>(functions);
	}

	/// <summary>
	/// Adds a list of functions to the already existing list of compiler functions.
	/// </summary>
	public static void AddCompilerFunctions(params Compiler.Function[] functions) {
		UISingleton.instance.compiler.addedFunctions.AddRange(functions);
	}

	/// <summary>
	/// Replacement for <see cref="CodeWalker.unPauseWalker"/>
	/// </summary>
	public static void UnpauseWalker() {
		UISingleton.instance.walker.resumeWalker();
	}

	/// <summary>
	/// Creates multiple smart buttons below the code view. Once pressed, it inserts code into the main code view.
	/// <para>If you wish to clear away all smart buttons, run this function or <seealso cref="SetSmartRichButtons(string[])"/> with empty parameters.</para>
	/// </summary>
	public static void SetSmartButtons(params string[] codes) {
		UISingleton.instance.smartButtons.ClearSmartButtons();
		for (int i = 0; i < codes.Length; i++) {
			UISingleton.instance.smartButtons.AddSmartButton(codes[i], codes[i]);
		}
	}

	/// <summary>
	/// Creates one smart button below the code view. Once pressed, it inserts code into the main code view.
	/// </summary>
	public static void AddSmartButton(string code) {
		UISingleton.instance.smartButtons.AddSmartButton(code, code);
	}

	/// <summary>
	/// Adds smart buttons with names taken from registered functions via <see cref="SetCompilerFunctions(Compiler.Function[])"/> or <see cref="AddCompilerFunctions(Compiler.Function[])"/>
	/// </summary>
	public static void AutoSetSmartButtons() {
		SetSmartButtons(UISingleton.instance.compiler.addedFunctions.ConvertAll(f => f.name + "()").ToArray());
	}

	/// <summary>
	/// Set the task description for current level. If passed empty string, both placeholders for task description will be deactivated.
	/// </summary>
	public static void SetTaskDescription(string taskDescription){
		UISingleton.instance.taskDescription.SetTaskDescription (taskDescription);
	}

	public static void SetCurrentLevelAnswere (Compiler.VariableTypes type, string[] answere) {
		int parameterAmount = answere.Length;
		UISingleton.instance.levelHandler.currentLevel.answere = new PM.Level.LevelAnswere (parameterAmount, type, answere);
	}

	/// <summary>
	/// Represents the current level. Setting this value will automatically setoff <see cref="IPMLevelChanged.OnPMLevelChanged(int)"/>
	/// <para>If set to a value higher than highest unlocked level then <seealso cref="unlockedLevel"/> will also be set to the same value.</para>
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if set to value outside of levels list index range, i.e. thrown if <seealso cref="currentLevel"/>.set &lt; 0 or ≥ <seealso cref="numOfLevels"/></exception>
	public static int currentLevel {
		get { return UISingleton.instance.levelbar.current; }
		set {
			if (value < 0 || value >= numOfLevels)
				throw new ArgumentOutOfRangeException("currentLevel", value, "Level index out of range!");
			else
				UISingleton.instance.levelbar.ChangeLevel(value);
		}
	}

	/// <summary>
	/// Represents the previous value of <see cref="currentLevel"/>. When there hasen't been a previous level, previousLevel has a value of -1.
	/// </summary>
	public static int previousLevel {
		get { return UISingleton.instance.levelbar.previous; }
	}

	/// <summary>
	/// This value determites how many levels are shown on the levelbar in the UI.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">In the case of non-positive values in setting <see cref="numOfLevels"/>.</exception>
	public static int numOfLevels {
		get { return UISingleton.instance.levelbar.numOfLevels; }
		set {
			if (value > 0) UISingleton.instance.levelbar.RecreateButtons(value, Mathf.Clamp(currentLevel, 0, value - 1), unlockedLevel);
			else throw new ArgumentOutOfRangeException("numOfLevels", value, "Zero and negative values are not accepted!");
		}
	}

	/// <summary>
	/// The highest level that's unlocked. Value of 0 means only first level is unlocked. Value of (<seealso cref="numOfLevels"/> - 1) means last level is unlocked, i.e. all levels.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">In the case of invalid values in setting <see cref="unlockedLevel"/></exception>
	public static int unlockedLevel {
		get { return UISingleton.instance.levelbar.unlocked; }
		set {
			if (value >= 0 && value < numOfLevels) UISingleton.instance.levelbar.UpdateButtons(currentLevel, value);
			else throw new ArgumentOutOfRangeException("unlockedLevel", value, "Level value is out of range of existing levels.");
		}
	}

	/// <summary>
	/// The highest level that has had it's story revealed.
	/// </summary>
	public static int storydLevel {
		get { return UISingleton.instance.levelHints.storyRevealedForLevel; }
	}


	public static int currentCase {
		get { return UISingleton.instance.levelHandler.currentLevel.caseHandler.currentCase; }
	}

	/// <summary>
	/// Stops the compiler, shows the "Level complete!" popup, marks the current level as complete and unlocks the next level.
	/// </summary>
	public static void SetLevelCompleted() {
		UISingleton.instance.winScreen.SetLevelCompleted();
	}

	/// <summary>
	/// Set case completed if test case is passed. Marks current case as completed and starts next test case.
	/// </summary>
	public static void SetCaseCompleted() {
		UISingleton.instance.levelHandler.currentLevel.caseHandler.CaseCompleted ();
	}

	/// <summary>
	/// Switches to choosen case.
	/// </summary>
	/// <param name="caseNumber">The case number to switch to.</param> 
	public static void SwitchCase(int caseNumber) {
		UISingleton.instance.levelHandler.currentLevel.caseHandler.SetCurrentCase (caseNumber);
	}

	/// <summary>
	/// Jumps to the last level. <paramref name="ignoreUnlocked"/> determines if it should respect or ignore <seealso cref="unlockedLevel"/>.
	/// </summary>
	/// <param name="ignoreUnlocked">If true, it will jump to the absolute last level. If false, it will jump to the last unlocked level.</param>
	public static void JumpToLastLevel(bool ignoreUnlocked = false) {
		currentLevel = ignoreUnlocked ? numOfLevels - 1 : unlockedLevel;
	}

	/// <summary>
	/// Jumps to the first level.
	/// </summary>
	public static void JumpToFirstLevel() {
		currentLevel = 0;
	}

	/// <summary>
	/// Shows the "Help I'm stuck!" popup.
	/// </summary>
	public static void ShowHintsPopup() {
		UISingleton.instance.levelHints.ButtonShowHintScreen();
	}

	/// <summary>
	/// Hides the "Help I'm stuck!" popup.
	/// </summary>
	public static void HideHintsPopup() {
		UISingleton.instance.levelHints.StartHideFading();
	}

	/// <summary>
	/// Stops the compiler and shows a dialog box containing the error message on the current line.
	/// </summary>
	/// <exception cref="PMRuntimeException">Is always thrown</exception>
	public static void RaiseError(string message) {
		UISingleton.instance.textField.theLineMarker.setErrorMarker(message);
	}
	/// <summary>
	/// Stops the compiler and shows a dialog box containing the error message on the target <paramref name="newLineNumber"/>.
	/// </summary>
	/// <exception cref="PMRuntimeException">Is always thrown</exception>
	public static void RaiseError(int newLineNumber, string message) {
		UISingleton.instance.textField.theLineMarker.setErrorMarker(newLineNumber, message);
	}
	/// <summary>
	/// Stops the compiler and shows a dialog box containing the error message on the target <see cref="UnityEngine.UI.Selectable"/>.
	/// </summary>
	/// <exception cref="PMRuntimeException">Is always thrown</exception>
	public static void RaiseError(UnityEngine.UI.Selectable targetSelectable, string message) {
		UISingleton.instance.textField.theLineMarker.setErrorMarker(targetSelectable, message);
	}
	/// <summary>
	/// Stops the compiler and shows a dialog box containing the error message on the target <see cref="RectTransform"/>.
	/// </summary>
	/// <exception cref="PMRuntimeException">Is always thrown</exception>
	public static void RaiseError(RectTransform targetRectTransform, string message) {
		UISingleton.instance.textField.theLineMarker.setErrorMarker(targetRectTransform, message);
	}
	/// <summary>
	/// Stops the compiler and shows a dialog box containing the error message on the target canvas position.
	/// </summary>
	/// <exception cref="PMRuntimeException">Is always thrown</exception>
	public static void RaiseError(Vector2 targetCanvasPosition, string message) {
		UISingleton.instance.textField.theLineMarker.setErrorMarker(targetCanvasPosition, message);
	}
	/// <summary>
	/// Stops the compiler and shows a dialog box containing the error message on the target world position.
	/// <para>NOTE: The game camera must be marked with the "Main Camera" tag for this to work.</para>
	/// </summary>
	/// <exception cref="PMRuntimeException">Is always thrown</exception>
	public static void RaiseError(Vector3 targetWorldPosition, string message) {
		UISingleton.instance.textField.theLineMarker.setErrorMarker(targetWorldPosition, message);
	}

	/// <summary>
	/// Makes the IDE not destroy on load, i.e. on level change and such.
	/// <para>Equal to <see cref="UnityEngine.Object.DontDestroyOnLoad(UnityEngine.Object)"/> but on the IDE.</para>
	/// </summary>
	public static void DontDestroyIDEOnLoad() {
		if (UISingleton.instance.ideRoot.parent != null)
			UISingleton.instance.ideRoot.parent = null;
		UnityEngine.Object.DontDestroyOnLoad(UISingleton.instance.ideRoot);
	}

}
