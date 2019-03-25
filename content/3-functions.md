[&lt; Back to table of contents](/README.md)

---

# `3.5.` _IDE/Compiler_ - **Writing python functions**

Defining functions is the core of the compiler.

And it's quite the simple task.

> **NOTE:** Since 2.0.0, the old compiler has been replaced by the new Mellis compiler.
> With this includes changes in signature and namespaces.
>
> Please take note of the new syntax below.

---

## Function requirements

- Each function is specified into its own class. _(This does not mean its own file since you can have multiple classes in the same file)_

- The class shall inherit from `Mellis.ClrFunction`.
- The function is required to define the name of the function the user can call via python
- The class is required to define the `Invoke` method.

---

## The constructor

You assign the name of the function via in the base class via the constructor.
This is done by adding the `: base("myFuncName")` to your constructor.

### Example

> In these examples we will create a function that rounds a number.  
> The usage of this function will in python be `x = Round(5.5)`

```CS
public class MyFunction : ClrFunction {
    public MyFunction()
        : base("Round")
    {
    }
    // ...
}
```

---

## The `Invoke` method

When the user calls your function _(as defined by the `: base(name)`)_ the compiler will execute the `Invoke` method.

- The `Invoke` method must be defined as such:

```CS
public override IScriptType Invoke(params IScriptType[] arguments) { }
```

- The `Invoke` method must return a value of type `IScriptType`.
Even if you intend on not returning any values you must return a `None` which is done via using the value factory that can be accessed through the `Processor` variable. For example `Processor.Factory.Null` for the python `None` value.

### Example

> Continuing with the example of creating a rounding function.  
> _**Note:** Here we're using `Math` (System) and not `Mathf` (UnityEngine). This is an implementation detail as `Mathf` does not support double maths._

```CS
using Mellis;

public class MyFunction : ClrFunction
{
    public MyFunction()
        : base("Round")
    {
    }

    public override IScriptType Invoke(params IScriptType[] arguments)
    {
        double number = arguments[0].TryConvert<double>();
        double rounded = Math.Round(number);

        return Processor.Factory.Create(rounded);
    }
}
```

### Result

![Rounding function in action](/images/roundfunc-example.png)

> _**Note:** You also have to register the function to the compiler for the function to be usable. You do this via the wrappers `SetCompilerFunctions` function. More info about that can be found at the [`3.2.`&nbsp;Accessing compiler](/content/3-compiler.md#setting-environment-functions) page._

---

## Yielding functions

Some functions may need longer time to process than an instant. For example you want an animation to finish playing before moving on in the code. This is where yielding the CodeWalker comes in.

To make a function yield the CodeWalker, you inherit from `ClrYieldingFunction` instead of `ClrFunction`.

Then to resume the CodeWalker you call the wrappers `UnpauseWalker` function.

### Yielding functions `InvokeEnter` and `InvokeExit` methods

Compared with the `Invoke` from `ClrFunction`, a **yielding** function has two methods. One `InvokeEnter` and an `InvokeExit`.

It is required to override the `InvokeEnter`, but it cannot handle return values. You can override InvokeExit, that is called from `PMWrapper.UnpauseWalker()`, to specify the return value.

You can also pass a return value directly to `PMWrapper.UnpauseWalker(IScriptType)`, that is with the default implementation of `InvokeExit` used as the return value.

### Specifications for `PMWrapper.UnpauseWalker`

> Scope: `PMWrapper`  
> Definition: `public static void UnpauseWalker()`  
> Available since version: `0.5`  

> Scope: `PMWrapper`  
> Definition: `public static void UnpauseWalker(IScriptType returnValue)`  
> Available since version: `2.0`  

### Example

```CS
public class MyScript : MonoBehaviour
{
    public float movingRemaining;

    private void Start()
    {
        PMWrapper.SetCompilerFunctions(new MyFunction(this));
    }

    private void Update()
    {
        if (movingRemaining > 0)
        {
            movingRemaining -= Time.deltaTime;
            transform.position += Vector3.right * Time.deltaTime;

            if (movingRemaining <= 0)
            {
                PMWrapper.UnpauseWalker();
            }
        }
    }
}

public class MyFunction : ClrYieldingFunction
{
    private MyScript myScript;

    public MyFunction(MyScript myScript)
        : base("move_right")
    {
        this.myScript = myScript;
    }

    public override void InvokeEnter(params IScriptType[] arguments)
    {
        myScript.movingRemaining = 2;
    }
}
```

### Result

> If we never called `PMWrapper.UnpauseWalker()` then it would stop moving, but the code would never get to the second `move_right()`

![Walker pauses and waits until UnpauseWalker is called](/images/pausewalker-example.gif)

## Custom errors

> Custom error messages has been moved to their very own page. _"Such luxury!"_
>
> [`2.5.`&nbsp;Custom error messages](/content/2-errors.md)

---

[&lt; Back to table of contents](/README.md)
