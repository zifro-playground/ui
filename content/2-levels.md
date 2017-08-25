[&lt; Back to table of contents](/README.md)

---

# `2.2.` _GUI_ - **Levels system**

One of the _"Universalization"_ additions of version `0.5` is levels! The functionality supplied with the PythonMachine-UI is seeing how many levels there are, seeing which one you're currently on, seeing which ones are locked, and switching between levels.

Your job as a game programmer is implementing a system of switching the actual level.<br>
How? Well It's all written here down below.

> **Note:** ~~<i>Compared with the hints system, the level id you find in multiple parts of the wrapper such as `OnPMLevelChanged(int level)` and `PMWrapper.currentLevel` count from `0` and upwards, while in the hints system it counts from `1` and upwards.~~ **THIS HAS BEEN CHANGED!**
> All levels now count from 0, including the hints and tooltips.</i>

---

## Listening for level events

The events you can play around with when it comes to levels are `OnPMLevelChanged` and `OnPMLevelCompleted`.

The most important step that needs to be implemented by you as game programmer when it comes to levels is the events. More specifically the `OnPMLevelChanged` event.

You see the event `OnPMLevelChanged` only fires when the UI level is changed. You need to upon getting this event change the current game level.

In combination with the events, you may use `currentLevel` and `previousLevel` as well.

### Specifications for `OnPMLevelChanged`

> Interface: `IPMLevelChanged`<br>
> _~~Definition: `void OnPMLevelChanged(int level)`~~ PARAMETER REMOVED IN 0.6_<br>
> Definition: `void OnPMLevelChanged()`<br>
> Available since version: `0.5`<br>

### Specifications for `OnPMLevelCompleted`

> Interface: `IPMLevelCompleted`<br>
> _~~Definition: `void OnPMLevelCompleted(int level)`~~ PARAMETER REMOVED IN 0.6_<br>
> Definition: `void OnPMLevelCompleted()`<br>
> Available since version: `0.5`<br>

### Specifications for `PMWrapper.currentLevel`
> Scope: `PMWrapper`<br>
> Definition: `public static int currentLevel { get; set; }`<br>
> Available since version: `0.5`

### Specifications for `PMWrapper.previousLevel`
> Scope: `PMWrapper`<br>
> Definition: `public static int previousLevel { get; }`<br>
> Available since version: `0.6`

### Example

```CS
public class MyScript : MonoBehaviour, IPMLevelCompleted {
  	public void OnPMLevelCompleted() {
  		  SceneManager.LoadScene("Level " + PMWrapper.currentLevel);
  	}
}
```

---

## Setting number of levels

Setting the number of levels will only be needed to be done once in the entire lifecycle of the game. But no matter when you wish to change it, you do it via the wrappers property `numOfLevels`.

Since changing the number of levels is only done once, there's already a script called `SetNumLevels` ready that lays on the topmost object in the UI prefab.

![Inspector view of SetNumLevels](/images/setnumlevels.png)

### Specifications for `PMWrapper.numOfLevels`

> Scope: `PMWrapper`<br>
> Definition: `public static int numOfLevels { get; set; }`<br>
> Available since version: `0.5`

### Example `1`

```CS
public class MyScript : MonoBehaviour {
  	private void Start() {
  		  PMWrapper.numOfLevels = 5;
  	}
}
```

### Result `1`

![Levelbar with 10 buttons](/images/levelbar-5.png)

### Example `2`

```CS
public class MyScript : MonoBehaviour {
  	private void Start() {
  		  PMWrapper.numOfLevels = 10;
  	}
}
```

### Result `2`

![Levelbar with 10 buttons](/images/levelbar-10.png)

---

## Marking levels as complete

This functionality will be used almost as much as unpausing the CodeWalker.

There happens to be a wrapper function just for this, and it's called `SetLevelCompleted`.

It does multiple things, such as:
- Shows the *"Level complete!"* popup.
- If there is a next level and it's locked, it unlocks it.
- ~~Saves the users progress among levels.~~ PLANNED

### Specifications for `PMWrapper.SetLevelCompleted`

> Scope: `PMWrapper`<br>
> Definition: `public static void SetLevelCompleted()`<br>
> Available since version: `0.5`

### Example

> _**Note:** In this example `MyFunction` is added as function elsewhere to shorten the example._

```CS
public class MyFunction : Compiler.Function {
  	public MyFunction() {
    		this.name = "instant_win";
    		this.inputParameterAmount.Add(0);
  	}

  	public override Variable runFunction(Scope currentScope, Variable[] inputParas, int lineNumber) {

    		PMWrapper.SetLevelCompleted();

    		return new Variable("NULL");
  	}
}
```

### Result

![SetLevelCompleted example](/images/setlevelcompleted-example.gif)

---

## Changing level

You may for some reason wish to change level via script, instead of impatiently waiting for the user to change it.

This is easily done via the wrapper property `currentLevel` and the functions `JumpToFirstLevel` and `JumpToLastLevel`.

### Specifications for `PMWrapper.currentLevel`
> Scope: `PMWrapper`<br>
> Definition: `public static int currentLevel { get; set; }`<br>
> Available since version: `0.5`

### Specifications for `PMWrapper.JumpToFirstLevel`
> Scope: `PMWrapper`<br>
> Definition: `public static void JumpToFirstLevel()`<br>
> Available since version: `0.5.2`

### Specifications for `PMWrapper.JumpToLastLevel`

> Scope: `PMWrapper`<br>
> Definition: `public static void JumpToLastLevel(bool ignoreUnlocks = false)`<br>
> Available since version: `0.5.2`

### Example

> _**Note:** Both `MyScript` and `MyGotoNextLevel` are made up for the sake of this example._

```CS
public class MyScript : MonoBehaviour {
    public void MyGotoNextLevel() {
        PMWrapper.currentLevel++;
    }
}
```

---

## Unlocking levels

Which levels that are locked and unlocked is determined by a single integer: `unlockedLevel`.

If set to `0` then only up to level with index `0` is unlocked _(i.e. only the first level is unlocked)_, while the rest is locked.

If set to `2` then all levels up to index (inclusive) `2` is unlocked _(i.e. first, second, and third levels are unlocked)_.

### Specifications for `PMWrapper.unlockedLevel`

> Scope: `PMWrapper`<br>
> Definition: `public static int unlockedLevel { get; set; }`<br>
> Available since version: `0.5`

### Example

```CS
public class MyScript : MonoBehaviour {
  	private void Start() {
  		  PMWrapper.unlockedLevel = 3;
  	}
}
```

### Result

> If you look closely you can see that only the last level is grayed out. Setting `unlockedLevel` to a value of `3` unlocks all levels up to the level with index `3`, including that level as well. Level with index `3` is of course the forth level.

![All except one level unlocked](/images/levelbar-unlocked.png)

---

## Demo levels

Demo levels are levels that follows a predefined script, that basically plays the level for you.

> _See the [`4.2`&nbsp;Demo levels](/content/4-demo.md) page for more information about writing these demo scripts._

Sensing whether a level is or is not a demo level can be done by a couple of wrapper properties:

- `isDemoLevel`  
If true, the current level is a demo level.

- `isDemoingLevel`  
If true, the current level is a demo level AND is currently playing the demo script.

### Specifications for `PMWrapper.isDemoLevel`

> Scope: `PMWrapper`<br>
> Definition: `public static bool isDemoLevel { get; }`<br>
> Available since version: `0.7`

### Specifications for `PMWrapper.isDemoingLevel`

> Scope: `PMWrapper`<br>
> Definition: `public static bool isDemoingLevel { get; }`<br>
> Available since version: `0.7`

---

[&lt; Back to table of contents](/README.md)
