[&lt; Back to table of contents](/README.md)

---

# `4.1.` _Resources_ - **Hints system**

Hints has been added to the PythonMachine! They are integrated together with the [`2.2.`&nbsp;Levels System](/content/2-levels.md) to give good aid to those who are stuck on a level, based off of which level they are currently on.

The PythonMachine package comes with some example tips and a guide on how to write tips.

---

## Hints folder

Hints are loaded from the `Assets/Resources/` folder.<br>
Format: `levelhint_X_Y` _(where `X` is the level index and `Y` is the hint number)_

---

## Loading hints

**Hints are fully automatically loaded** for the correct level by the PythonMachine. No setup required. This is just a documentation of how it loads.

When a level is loaded, the machine looks inside the `Assets/Resources/` folder for hints for the current level. Hints starts with index `1`, but if you wish to have a hint popup on level load then you can create a hint with index `0`. Though index `0` is optional, if you dont want it to automatically open, start at `1` instead.

Here's the basic algorithm:

> <i>**Note:** In this algorithm, level id `X` and hint id `Y` starts at `0`. First hint file for level 0 would therefore be `levelhint_0_0`.</i>

```
Algorithm for loading hints for level X:
1. Start Y at 0.
2. Check if text file "levelhint_X_Y" exists.
  2.1. True:
    2.1.1. Get hint title and content from "levelhint_X_Y" text file.
    2.1.2. Get hint image from "levelhint_X_Y" sprite file.
    2.1.3. Increment Y.
    2.1.4. Goto step 2.
  2.2. False:
    2.2.1. If Y is 0, goto step 2.
    2.2.1. Else, end algorithm.
```

Because the structure of this algorithm, there are some flaws.
- Hints index must start at 0 and 1.
- All hints for a level must not have gaps in among the indexes. Example: `levelhint_1_1`, `levelhint_1_2`, `levelhint_1_5`, here it would stop after not finding a hint nr3 and therefor not finding hint nr5.
- It's case sensitive. Meaning `levelHINT_0_1.txt` will not be loaded.

---

## Story hints

Defining a "story" hint is identical to writing "normal" hints (as shown below) but with the exception of their index being `0`.

You see normal hints starts counting at `1`, but if a hint is defined with the index `0` that hint is automatically revealed when a level is loaded for the first time.

There's one property together with this that is called `storydLevel`. This integer defines which level has had their story hint revealed.

### Specifications for `PMWrapper.storydLevel`

> Scope: `PMWrapper`<br>
> Definition: `public static int storydLevel { get; }`<br>
> Available since version: `0.7`<br>

---

## Writing hints

> _All this info can be found in the `Assets/Resources/How to write hints.txt` file._

This is really simple. Each hint file is defined as such:

- The first row defines the title.
- The rest defines the hints content.

### Example:
```
Hello World!
Lorem ipsum dolor sit amet, consectetur adipisicing elit
```
### Result:

![Screenshot of in-game hint](/images/hints-screenshot-1.png)

---

## Adding images

The system supports `1` image per hint.
To add an image, you just have a sprite with the same name as the `levelhint.txt` file.

> _**Note:** For the images to be marked as sprites the `Texture Type` must be marked as `Sprite` in `Import Settings`._
>
> ![Image of import settings for a sprite](/images/sprite-import-settings.png)

### Example:
Where `levelhint_1_1.txt` contains the following:

```
PythonMaskinen FTW
Lorem ipsum quis nostrud exercitation ullamco laboris nisi ut aliquip ex.

Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi
```

Where `levelhint_1_1.png` contains the following:

![Example of hint image](/images/levelhint_1_1.png)

### Result:

![Screenshot of in-game hint](/images/hints-screenshot-2.png)

---

## Rich Text

Not only does it support images, it also supports Unitys **Rich Text**.<br>
_(Rich Text is kinda like HTML on sleeping pills)_

You can change the style _(bold, italic)_, size, and color:

```
Bold:    <b>example</b>
Italic:  <i>example</i>
Size:    <size=32>example</size>
Color:   <color=green>example</color>
         <color=#ffffff40>example</color>
         ^ here the 40 is actually the opacity in hex
```

Read more here:<br>
https://docs.unity3d.com/Manual/StyledText.html

> _**Note:** The default size used in the hints title is `48` and the default size used in the hints content is `32`._

### Example:

```
<b>Rich Text</b> Yaaay
<size=24>Small</size>
Normal
<size=48>Big</size>
<i>Italic</i>
<color=green>Colored</color>
```

### Result:

![Screenshot of in-game hint](/images/hints-screenshot-3.png)

---

## Open hints via script

Hints are meant to be opened via the "Hint" button, but for the odd occasion where you wish to open it via script you may use the `ShowHintsPopup` and `HideHintsPopup` wrapper functions.

### Specifications for `PMWrapper.ShowHintsPopup`

> Scope: `PMWrapper`<br>
> Definition: `public static void ShowHintsPopup()`<br>
> Available since version: `0.5`<br>

### Specifications for `PMWrapper.HideHintsPopup`

> Scope: `PMWrapper`<br>
> Definition: `public static void HideHintsPopup()`<br>
> Available since version: `0.5`<br>

---

[&lt; Back to table of contents](/README.md)
