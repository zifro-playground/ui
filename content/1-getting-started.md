[&lt; Back to table of contents](/README.md)

---

# `1.4.` _Setup_ - **Getting started**

Let's assume the scenario that you've just installed the package and you need to get going. Game ain't gonna make itself, maybe your console if filled with error messages because of obsolete functions, and all you need the UI to start working.

Let's get started.

---

## Step 1 - Adding the UI

Jump into your scene you want the UI to be in and remove the old UI prefab if you just upgraded.

Open the following folder in your Project tab in Unity:
`Assets/_Pythonmaskinen/Prefabs/`
And just drag'n'drop the `IDE` prefab into the `Hierarchy` tab.

_Lika 'a so:_<br>
![Instructions via GIF](/images/add_ide_to_scene.gif)

---

## Step 2 - Fix your camera

Right after importing it might not look that promising, but this is because your `Main Camera` is taking up all the screen instead of just using the `RectTransform` that's designated for the camera.

Lets fix that by adding the script `MatchViewportToGameRect` to your main game camera, as well as enabling it in edit mode.

_Voilà:_<br>
![Instructions via GIF](/images/match_viewport.gif)

---

## Step 3 - Setting number of levels

At the top of the IDE prefab is the script `SetNumLevels` that sets the number of levels for you. If you wish to set the number of levels elsewhere please disable or remove this script.

> _**Note:** Removing/Changing this script will have to be done EACH TIME YOU UPGRADE (because the prefab resets)_

---

## Step 4 - Writing your first function

Writing functions has remained the same, for good reason. It would be quite the pain to rewrite all your function structure.

Registering functions however, has become quite the simple task. Mostly everything usable in the game is now accessible via `PMWrapper`.

### Example:

> _**Note:** This is inside `MyScript.cs` that's added to a `GameObject` in the scene. This is important for otherwise `MyScript.Start()` would never execute._

```CS
public class MyScript : MonoBehaviour {
    private void Start() {
        PMWrapper.SetCompilerFunctions(new MyFunction());
    }
}


public class MyFunction : Compiler.Function {
  	public MyFunction() {
    		this.name = "foobar";
    		this.inputParameterAmount.Add(0);
  	}

  	public override Variable runFunction(Scope currentScope, Variable[] inputParas, int lineNumber) {
    		Debug.Log("Hello from inside a function!");

    		return new Variable("NULL");
  	}
}
```

### Result:
![Image of result](/images/getting-started-example-1.png)

---

## Step 5 - Writing your first event listener

This is one of the core improvements to the PythonMachine. When you previously wanted to listen for when the compiler started you had to get a reference to `HelloCompiler` and use it's function `startStopCompilerEvent(Action)`. There was no way of unregistering events, and no solid way of finding the compiler.

From version `0.5` we now use interfaces!

The event interfaces can be found in the [`WrapperEvents.cs`](https://github.com/HelloWorldSweden/PythonMaskinen-UI/blob/master/Assets/_Pythonmaskinen/UI%20Wrapper/WrapperEvents.cs) file, but an easier way to find them is by either looking at the [`4.1.` Using interface events](/content/4-events.md) page or if you have IntelliSense on your editor you can find them easily since they all starts with `IPM` _(short for <b>I</b>nterface <b>P</b>ython<b>M</b>achine)_.

![IntelliSense example](/images/getting-started-intellisense.png)

### Example:

Let's try implementing a listener for when the compiler is started. The interface for that is called `IPMCompilerStarted`.

First we need to use the interface. Quite simply, just add it as a type you wish to inherit from, like so:

```CS
public class MyScript : MonoBehaviour, IPMCompilerStarted {

}
```

Your editor will probably redline the interface, and if you jump back into Unity you will see the error message `'MyScript' does not implement interface member 'IPMCompilerStarted.OnPMCompilerStarted()'`.

This is because we need to, as the error message sais, implement the interface _member_. Implementing it looks like so:

```CS

public class MyScript : MonoBehaviour, IPMCompilerStarted {
  	public void OnPMCompilerStarted() {
        Debug.Log("Hello from inside an event!");
  	}
}
```

Alright, save that, jump into Unity, and try starting the compiler. This should be your result:

> _**Note:** The script `MyScript.cs` has to be added to a `GameObject` in the scene. This is important for an instance of `MyScript.cs` needs to exist._

> _**Note 2:** One catch with the interface events is that they only work if the object they are assigned to inherits from `UnityEngine.Object`._

### Result:
![Image of result](/images/getting-started-example-2.png)

---

[&lt; Back to table of contents](/README.md)
