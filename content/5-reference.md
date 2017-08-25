[&lt; Back to table of contents](/README.md)

---

# `5.1.` _Miscellaneous_ - **Reference list**

This is just a long list of all the available wrapper functions, wrapper properties, and events _(sorted by name)_; but also where you can read more about them in this documentation.

Please note that there are multiple things that are not listed here; for example [hints](/content/4-hints.md) and [demo levels](/content/4-demo.md). **This is not a complete list of all the PythonMachine-UI functionality.** It's just a list of all the functions available in the **wrapper**.

---

## Wrapper functions

Function | Documentation
:------- | -------------
`PMWrapper.AddCode`|[`3.1.`&nbsp;**Code window**#Accessing the code](/content/3-code-window.md#accessing-the-code)
`PMWrapper.AddCompilerFunctions`|[`3.2.`&nbsp;**Accessing compiler**#Setting environment functions](/content/3-compiler.md#setting-environment-functions)
`PMWrapper.AddSmartButton`|[`3.4.`&nbsp;**Smart Buttons**#Setting Smart Buttons](/content/3-smart-buttons.md#setting-smart-buttons)
`PMWrapper.AddSmartRichButton`<br>_**Warning:** Deprecated feature._ |[`3.4.`&nbsp;**Smart Buttons**#Smart _Rich_ Buttons?](/content/3-smart-buttons.md#smart-rich-buttons)
`PMWrapper.AutoSetSmartButtons`|[`3.4.`&nbsp;**Smart Buttons**#Setting Smart Buttons](/content/3-smart-buttons.md#setting-smart-buttons)
`PMWrapper.DontDestroyIDEOnLoad`|[`3.6.`&nbsp;**Keep IDE between scenes**#"Don't Destroy On Load"](/content/3-keep-ide.md#dont-destroy-on-load)
`PMWrapper.HideHintsPopup`|[`4.1.`&nbsp;**Hints system**#Open hints via script](/content/4-hints.md#open-hints-via-script)
`PMWrapper.JumpToFirstLevel`|[`2.2.`&nbsp;**Levels system**#Changing level](/content/2-levels.md#changing-level)
`PMWrapper.JumpToLastLevel`|[`2.2.`&nbsp;**Levels system**#Changing level](/content/2-levels.md#changing-level)
`PMWrapper.RaiseError`|[`2.4.`&nbsp;**Custom error messages**#Specifications _(since 0.7)_](/content/2-errors.md#specifications-for-pmwrapperraiseerror-since-07)
`PMWrapper.SetCompilerFunctions`|[`3.2.`&nbsp;**Accessing compiler**#Setting environment functions](/content/3-compiler.md#setting-environment-functions)
`PMWrapper.SetLevelCompleted`|[`2.2.`&nbsp;**Levels system**#Marking levels as complete](/content/2-levels.md#marking-levels-as-complete)
`PMWrapper.SetSmartButtons`|[`3.4.`&nbsp;**Smart Buttons**#Setting Smart Buttons](/content/3-smart-buttons.md#setting-smart-buttons)
`PMWrapper.SetSmartRichButtons`<br>_**Warning:** Deprecated feature._ |[`3.4.`&nbsp;**Smart Buttons**#Smart _Rich_ Buttons?](/content/3-smart-buttons.md#smart-rich-buttons)
`PMWrapper.ShowHintsPopup`|[`4.1.`&nbsp;**Hints system**#Open hints via script](/content/4-hints.md#open-hints-via-script)
`PMWrapper.StartCompiler`|[`3.2.`&nbsp;**Accessing compiler**#Starting and stopping the compiler](/content/3-compiler.md#starting-and-stopping-the-compiler)
`PMWrapper.StopCompiler`|[`3.2.`&nbsp;**Accessing compiler**#Starting and stopping the compiler](/content/3-compiler.md#starting-and-stopping-the-compiler)
`PMWrapper.UnpauseWalker`|[`3.5.`&nbsp;**Writing python functions**#Pausing functions](/content/3-functions.md#pausing-functions)

---

## Wrapper properties

Property | Documentation
:------- | -------------
`PMWrapper.codeRowsLimit`|[`3.1.`&nbsp;**Code window**#Code rows limit](/content/3-code-window.md#code-rows-limit)
`PMWrapper.currentLevel`|[`2.2.`&nbsp;**Levels system**#Listening for level events](/content/2-levels.md#listening-for-level-events)<br>[`2.2.`&nbsp;**Levels system**#Changing level](/content/2-levels.md#changing-level)
`PMWrapper.fullCode`|[`3.1.`&nbsp;**Code window**#Accessing the code](/content/3-code-window.md#accessing-the-code)
`PMWrapper.isCompilerRunning`|[`3.2.`&nbsp;**Accessing compiler**#Sensing starts and stops](/content/3-compiler.md#sensing-start-and-stops)
`PMWrapper.isCompilerUserPaused`|[`3.2.`&nbsp;**Accessing compiler**#Sensing pause and unpause](/content/3-compiler.md#sensing-pause-and-unpause)
`PMWrapper.isDemoingLevel`|[`2.2.`&nbsp;**Levels system**#Demo levels](/content/2-levels.md#demo-levels)
`PMWrapper.isDemoLevel`|[`2.2.`&nbsp;**Levels system**#Demo levels](/content/2-levels.md#demo-levels)
`PMWrapper.mainCode`|[`3.1.`&nbsp;**Code window**#Accessing the code](/content/3-code-window.md#accessing-the-code)
`PMWrapper.numOfLevels`|[`2.2.`&nbsp;**Levels system**#Setting number of levels](/content/2-levels.md#setting-number-of-levels)
`PMWrapper.postCode`<br>_**Warning:** Experimental feature._ |[`3.1.`&nbsp;**Code window**#Accessing the code](/content/3-code-window.md#accessing-the-code)
`PMWrapper.preCode`|[`3.1.`&nbsp;**Code window**#Accessing the code](/content/3-code-window.md#accessing-the-code)
`PMWrapper.previousLevel`|[`2.2.`&nbsp;**Levels system**#Listening for level events](/content/2-levels.md#listening-for-level-events)
`PMWrapper.speedMultiplier`|[`3.3.`&nbsp;**Speed multiplier**#Accessing the speed multiplier](/content/3-speed.md#accessing-the-speed-multiplier)
`PMWrapper.storydLevel`|[`4.1.`&nbsp;**Hints system**#Story hints](/content/4-hints.md#story-hints)
`PMWrapper.uiDisabled`<br>_**Warning:** Deprecated feature._ |_N/A_
`PMWrapper.unlockedLevel`|[`2.2.`&nbsp;**Levels system**#Unlocking levels](/content/2-levels.md#unlocking-levels)

---

## Wrapper events

Interface | Documentation
:-------- | -------------
`IPMCompilerStarted`|[`3.2.`&nbsp;**Accessing compiler**#Sensing starts and stops](/content/3-compiler.md#sensing-start-and-stops)
`IPMCompilerStopped`|[`3.2.`&nbsp;**Accessing compiler**#Sensing starts and stops](/content/3-compiler.md#sensing-start-and-stops)
`IPMCompilerUserPaused`|[`3.2.`&nbsp;**Accessing compiler**#Sensing pause and unpause](/content/3-compiler.md#sensing-pause-and-unpause)
`IPMCompilerUserUnPaused`|[`3.2.`&nbsp;**Accessing compiler**#Sensing pause and unpause](/content/3-compiler.md#sensing-pause-and-unpause)
`IPMLevelChanged`|[`2.2.`&nbsp;**Levels system**#Listening for level events](/content/2-levels.md#listening-for-level-events)
`IPMLevelCompleted`|[`2.2.`&nbsp;**Levels system**#Listening for level events](/content/2-levels.md#listening-for-level-events)
`IPMSpeedChanged`|[`3.3.`&nbsp;**Speed multiplier**#Accessing the speed multiplier](/content/3-speed.md#accessing-the-speed-multiplier)

---

[&lt; Back to table of contents](/README.md)
