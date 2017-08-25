[&lt; Back to table of contents](/README.md)

---

# `4.2.` _Resources_ - **Demo levels**

Simply put, demo levels are auto-playing levels. The goal with this functionality is to show the user a live example of how to solve a level, so they can later apply that to the next level.

---

## Script folder

Scripts are loaded from the `Assets/Resources/` folder.<br>
Format: `levelmanus_X` _(where `X` is the level index)_

---

## Loading hints

**Demo level scripts are fully automatically loaded** for the correct level by the PythonMachine. No setup required.

On startup, the PythonMachine looks for `levelmanus_X` TextAssets _(where `X` is the level number)_ for each level and loads the ones it finds. If successful then the level is visible in the levelbar with a different icon. _Like a so:_

**Normal level:** ![normal level icon](/images/lev_done_ok.png)&nbsp;![normal current level icon](/images/lev_current.png)<br>
**Demo level:** ![demo level icon](/images/lev_demo.png)&nbsp;![demo current level icon](/images/lev_demo_current.png)

---

## Writing demo level script

Writing demo levels requires you to follow our custom "manus" syntax.

### Script comments

One part of the demo script syntax is comments that will be ignored by the parser. Only single line comments are available. Comments are defined via two forward slashes `//` or a pound sign `#`.

---

## What's a "Step"?

One step is one task in the demo script. It is defined by it's name, a header, and a body. The name is required while the header and body may be optional.

- #### Name
If only name is required then you simply write the name on a single line. Most steps has multiple names, where all those names are valid. Names are case-insensitive _(example: `POPUP` works as well as `popup`)_.<br>
Example: `name`

- #### Header
The header is defined on the same line as the step name, separated by a comma `:`.<br>
Example: `name: header`

- #### Body
The body is multiline and is defined directly after the name with each line starting with a "greater than" sign `>`. Note that a comma is still required even if no header.<br>
Example:
```
name:
>Multiline
>body
>can go on forever
```

---

## All steps

The following points will list all available steps and then go through each one with a brief explanation of what they all do.

> :white_check_mark: means **required**<br>
> :x: means **not applicable**

Syntax | Description | Header | Body
------ | ----------- | ------ | ----
`popup`|[Show popup](#step---show-popup)|️:white_check_mark:|:white_check_mark:
`popup-settings`|[Set popup settings](#step---set-popup-settings)|:x:|:white_check_mark:
`finish`️|[Walk to finish](#step---walk-to-finish)|:x:|:x:
`step`️|[Walk number of steps](#step---walk-number-of-steps)|:white_check_mark:|:x:
`stop`️|[Stop compiler](#step---stop-compiler)|:x:|:x:
`set-code`️|[Set main code](#step---set-main-code)|:x:|:white_check_mark:
`set-pre-code`️|[Set pre code](#step---set-pre-code)|:x:|:white_check_mark:
`set-post-code`️|[Set post code](#step---set-post-code)|:x:|:white_check_mark:
`clear-code`️|[Clear all code](#step---clear-all-code)|:x:|:x:
`clear-main-code`️|[Clear main code](#step---clear-main-code)|:x:|:x:
`clear-pre-code`️|[Clear pre code](#step---clear-pre-code)|:x:|:x:
`clear-post-code`️|[Clear post code](#step---clear-post-code)|:x:|:x:
`wait`|[Wait seconds](#step---wait-seconds)|:white_check_mark:|:x:
`speed`|[Set speed multiplier](#step---set-speed-multiplier)|:white_check_mark:|:x:

---

## Step - Show popup
[^ Go to the list](#all-steps)

> **Names:** `bubble`, `popup`<br>
> **Header:** :white_check_mark: Defines the header text of the popup bubble. Supports rich text!<br>
> **Body:** :white_check_mark: Defines the body text of the popup bubble. Supports rich text!<br>
> **Available since version:** `0.7`<br>

Creates a popup bubble at a predefined location that must be closed before continuing in the demo script.

> _Notes:_
> - Requires `Action` and `Target` to be set beforehand via [Set popup settings](#step---set-popup-settings).

### _Example:_


```
popup: MANUS HEADER
>Hello World!
```

### _Result:_<br>
![Popup bubble example](/images/demo-popupbubble.png)

---

## Step - Set popup settings
[^ Go to the list](#all-steps)


> **Names:** `settings`, `bubble-settings`, `bubblesettings`, `popup-settings`, `popupsettings`<br>
> **Header:** :x: <br>
> **Body:** :white_check_mark: Defines each setting with key-value pairs, one pair per line, connected with equal sign `=`.<br>
> **Available since version:** `0.7`<br>

Sets some settings for the [Show popup](#step---show-popup) step.

The settings that are available are:

- ### `action`
    - `action = ClickNext`  
      The popup will contain a "OK" button that the user must press to continue.

    - `action = ClickTarget`  
      _(ONLY SUPPORTED FOR SELECTABLE TARGETS)_  
      The user must press the target Selectable to continue.

- ### `target`
    - `target = name of selectable`  
      The popup will point at a selectable. Acceptable names are:
        - Run button: `run`, `play`, `start`, `run button`, `play button`, `start button`
        - Stop button: `stop`, `stop button`
        - Hints button: `help`, `tips`, `help button`, `tips button`
        - Speed slider: `speed`, `slider`, `speed slider`
        - Smart button: `sb-{code}` _(where `{code}` is the content of the smart button, ex: `sb-print()`)_
        - Levelbar button: `level{LEVEL}` _(where `{LEVEL}` is the level index, ex: `level0`)_<br><br>

    - `target = (row)`  
      Where row is an integer of which row in the code windows that the popup shall point at. The parentheses are required!

    - `target = (x, y)`  
      Two numbers, separated via a comma `,` or a semicolon `;`, defines the coordinate in canvas space where the popup bubble shall point _(`(-800, 450)` is upper left corner, `(800,-450)` is the lower right corner)_. The parentheses are required!

    - `target = (x, y, z)`  
      Three numbers, separated via comma `,` or semicolon `;`, defines the coordinate in world space where the popup bubble shall point. The position is translated via the main game camera, so it's important that you have a camera marked with the `MainCamera` tag. The parentheses are required!

> _Notes:_
> - The settings are persistent between popups. If you're only going to use `Action = clicknext` then you only need to set it once.
>
> - Since version 0.7.3 the selectable in `Target = Selectable` is fetched during runtime instead of during parsetime. This gives some more freedom to when a selectable has to be defined. But this also means that the parser will not trigger an error until when the step is executed.

### _Example:_
```
popup-settings:
> target = run button
> action = clicktarget
```

---

## Step - Walk to finish
[^ Go to the list](#all-steps)

> **Names:** `finish`, `kör-klart`<br>
> **Header:** :x: <br>
> **Body:** :x: <br>
> **Available since version:** `0.7`<br>

Starts the compiler if not yet running, and runs it until it stops.

---

## Step - Walk number of steps
[^ Go to the list](#all-steps)

> **Names:** `step`, `run`, `steg`, `kör`<br>
> **Header:** :white_check_mark: Defines with an integer how many steps the CodeWalker shall take<br>
> **Body:** :x: <br>
> **Available since version:** `0.7`<br>

Starts the compiler if not yet running and steps `X` times, where `X` is an integer defined in the header.

### _Example:_
```
set-main-code:
>a = 1
>a = 2
>a = 3
>a = 4
>a = 5

step: 3
```

### _Result:_
The variable `a` will have the value of `3`.

---

## Step - Stop compiler
[^ Go to the list](#all-steps)

> **Names:** `stop`, `stopp`<br>
> **Header:** :x: <br>
> **Body:** :x: <br>
> **Available since version:** `0.7`<br>

If the compiler is paused after using the [Walk number of steps](#step---walk-number-of-steps) step, you may use this to reset the compiler.

### _Example:_
```
step: 3
stop
finish
```

### _Result:_
The CodeWalker will first take `3` steps, then play from the start. If `stop` was not there, it would've continued from step `3`.

---

## Step - Set main code
[^ Go to the list](#all-steps)

> **Names:** `code`, `set-code`, `main-code`, `maincode`, `set-main-code`<br>
> **Header:** :x: <br>
> **Body:** :white_check_mark: The value that the code window will be assigned to.<br>
> **Available since version:** `0.7`<br>

Replaces the main code (the one that the user can edit when demo level script is not playing).

---

## Step - Set pre code
[^ Go to the list](#all-steps)

> **Names:** `pre-code`, `precode`, `set-pre-code`<br>
> **Header:** :x: <br>
> **Body:** :white_check_mark: The value that the code window will be assigned to.<br>
> **Available since version:** `0.7`<br>

Replaces the pre code (the one that the user cannot edit and is above the main code).

---

## Step - Set post code
[^ Go to the list](#all-steps)

> **Names:** `post-code`, `postcode`, `set-post-code`<br>
> **Header:** :x: <br>
> **Body:** :white_check_mark: The value that the code window will be assigned to.<br>
> **Available since version:** `0.7`<br>

Replaces the post code (the one that the user cannot edit and is below the main code).

---

## Step - Clear all code
[^ Go to the list](#all-steps)

> **Names:** `clearcode`, `clear-code`, `clearallcode`, `clear-all-code`<br>
> **Header:** :x: <br>
> **Body:** :x: <br>
> **Available since version:** `0.7`<br>

Clears pre, main, and post code fields.

---

## Step - Clear main code
[^ Go to the list](#all-steps)

> **Names:** `clearmaincode`, `clear-main-code`<br>
> **Header:** :x: <br>
> **Body:** :x: <br>
> **Available since version:** `0.7`<br>

Clears the main code fields.

---

## Step - Clear pre code
[^ Go to the list](#all-steps)

> **Names:** `clearprecode`, `clear-pre-code`<br>
> **Header:** :x: <br>
> **Body:** :x: <br>
> **Available since version:** `0.7`<br>

Clears the pre code fields.

---

## Step - Clear post code
[^ Go to the list](#all-steps)

> **Names:** `clearpostcode`, `clear-post-code`<br>
> **Header:** :x: <br>
> **Body:** :x: <br>
> **Available since version:** `0.7`<br>

Clears the post code fields.

---

## Step - Wait seconds
[^ Go to the list](#all-steps)

> **Names:** `wait`, `sleep`, `vänta`<br>
> **Header:** :white_check_mark: Defines the number of seconds in one number.<br>
> **Body:** :x: <br>
> **Available since version:** `0.7`<br>

Pauses the demo level script for `X` number of seconds, where `X` is defined in the header.

### _Example:_
```
step: 1
sleep: 10
step: 1
sleep: 10
step: 1
```

### _Result:_
It will look like the compiler is running in super slow-mo, with 10 more seconds between each step.

---

## Step - Set speed multiplier
[^ Go to the list](#all-steps)

> **Names:** `set-speed`, `speed`<br>
> **Header:** :white_check_mark: Defines the new value for the speed slider.<br>
> **Body:** :x: <br>
> **Available since version:** `0.7.1`<br>

> _Notes:_
> - Similar to `PMWrapper.speedMultiplier` (Read more at: [`3.3.`&nbsp;**Speed multiplier**#Accessing the speed multiplier](/content/3-speed.md#accessing-the-speed-multiplier)) the value goes from `0` as slowest to `1` as fastest, with `0.5` as default.

Changes the speed multiplier that's seen on the speed slider in the UI.

---

[&lt; Back to table of contents](/README.md)
