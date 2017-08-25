[&lt; Back to table of contents](/README.md)

---

# `3.2.` _IDE/Compiler_ - **Accessing compiler**

Accessing the compiler is _kinda_ essential.

You as the game programmer needs to be able to change some parameters of the compiler, right? Such as setting which functions to be used, starting and stopping the compiler from running, as well as sensing when the compiler is started and stopped.

> _**Note:** This page will guide you how to set which functions the compiler should use. For defining those functions, visit the [`3.5.`&nbsp;Writing python functions](/content/3-functions.md) page._

> _If you're looking for ways to unpause the code walker, you can also find that at the [`3.5.`&nbsp;Writing python functions](/content/3-functions.md) page._

---

## Setting environment functions

Setting/changing which functions are included in the compiler at launch is done very simply via the wrappers `SetCompilerFunctions` and `AddCompilerFunctions` functions.

There's two distinct differences between those two functions.  
1. The `Set` function overrides the entire list. Useful upon level change where you need to scrap all the previous functions.  
2. The `Add` function adds a set of functions to the existing list. ~~This is useful if you want to...~~<br>
_I can't come up with any good reasons. But the option is there._


### Specifications for `PMWrapper.SetCompilerFunctions`

> Scope: `PMWrapper`<br>
> Definition: `public static void SetCompilerFunctions(params Compiler.Function[] functions)`<br>
> Available since version: `0.5`<br>

### Specifications for `PMWrapper.AddCompilerFunctions`

> Scope: `PMWrapper`<br>
> Definition: `public static void AddCompilerFunctions(params Compiler.Function[] functions)`<br>
> Available since version: `0.5`<br>

### Example

```CS
public class MyScript : MonoBehaviour {
    private void Start() {
        PMWrapper.SetCompilerFunctions(
            new MyFirstFunction(),
            new MySecondFunction()
        );
    }
}
```

---

## Sensing starts and stops

Sensing the compilers state can be done in different ways:  
1. Via the events `OnPMCompilerStarted` and `OnPMCompilerStopped`.  
2. Via the property `isCompilerRunning`.

They all do precisely what their names say. They report the state of the compiler.

One neat bonus of using events is that when the compiler stops the event `OnPMCompilerStopped` contains the way it was stopped via the `StopStatus` enum.

The `StopStatus` can have one of the following values:
- `HelloCompiler.StopStatus.Forced`: The compiler was stopped mid execution. Example via pressing the stop button.
- `HelloCompiler.StopStatus.RuntimeError`: The compiler had an error when executing the code. For example unassigned variables or syntax errors.
- `HelloCompiler.StopStatus.Finished`: The compiler finished its execution successfully.

### Specifications for `OnPMCompilerStarted`

> Interface: `IPMCompilerStarted`<br>
> Definition: `void OnPMCompilerStarted()`<br>
> Available since version: `0.5`<br>

### Specifications for `OnPMCompilerStopped`

> Interface: `IPMCompilerStopped`<br>
> _~~Definition: `void OnPMCompilerStopped(HelloCompiler.StopStatus status)`~~ CHANGED IN VERSION `0.7`_<br>
> Definition: `void OnPMCompilerStopped(PM.HelloCompiler.StopStatus status)`
> Available since version: `0.5`<br>

### Specifications for `isCompilerRunning`

> Scope: `PMWrapper`<br>
> Definition: `public static bool isCompilerRunning { get; }`<br>
> Available since version: `0.5`<br>

### Example

```CS
public class MyScript : MonoBehaviour, IPMCompilerStarted {
    public void OnPMCompilerStarted() {
        Debug.Log("Hello from inside an event!");
    }
}
```

### Result:
![Image of result](/images/getting-started-example-2.png)

---

## Sensing pause and unpause

Since version `0.7` the user has the ability to pause the compiler at any moment, but as far as the implementation is done it only pauses the CodeWalker.

To make it more responsive you as a game developer shall also implement this in your functions that halts the walker _(via `Function.pauseWalker = false`)_.

Sensing the pausing can be done in different ways:  
1. Via the events `OnPMCompilerUserPaused` and `OnPMCompilerUserUnpaused`.  
2. Via the property `isCompilerUserPaused`.

### Specifications for `OnPMCompilerUserPaused`

> Interface: `IPMCompilerUserPaused`<br>
> Definition: `void OnPMCompilerUserPaused()`<br>
> Available since version: `0.7`<br>

### Specifications for `OnPMCompilerUserUnpaused`

> Interface: `IPMCompilerUserUnPaused`<br>
> Definition: `void OnPMCompilerUserUnpaused()`<br>
> Available since version: `0.7`<br>

### Specifications for `isCompilerUserPaused`

> Scope: `PMWrapper`<br>
> Definition: `public static bool isCompilerUserPaused { get; }`<br>
> Available since version: `0.7`<br>

---

## Starting and stopping the compiler

Starting it and stopping it via script should never really be used since the user is the one that should control when it starts and stops.

Either way it's possible via the wrapper functions `StartCompiler` and `StopCompiler`.

Using these functions will also enable/disable the code window so it's not possible to change the code while it's running.

### Specifications for `PMWrapper.StartCompiler`

> Scope: `PMWrapper`<br>
> Definition: `public static void StartCompiler()`<br>
> Available since version: `0.5`<br>

### Specifications for `PMWrapper.StopCompiler`

> Scope: `PMWrapper`<br>
> Definition: `public static void StartCompiler()`<br>
> Available since version: `0.5`<br>

---

[&lt; Back to table of contents](/README.md)
