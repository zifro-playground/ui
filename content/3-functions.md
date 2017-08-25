[&lt; Back to table of contents](/README.md)

---

# `3.5.` _IDE/Compiler_ - **Writing python functions**

Defining functions is the core of the compiler.

And it's quite the simple task.

---

## Function requirements

- Each function is specified into its own class. _(This does not mean its own file since you can have multiple classes in the same file)_

- The class shall inherit from `Compiler.Function`.

- The function is required to define certain fields:
  - the name of the function the user can call via python
  - acceptable amounts of parameters
  - whether or not it pauses the CodeWalker
  - whether or not it has a return value


- The class is required to define the `runFunction` method.

---

## The constructor

It's easiest to define the fields in the class constructor.

> `inputParameterAmount` may not be so self explanatory. It's a list of the accepted parameter counts. If you have a function without any parameters, you then add this to the constructor:<br>
> `this.inputParameterAmount.Add(0);`

### Example

> In these examples we will create a function that rounds a number.<br>
> The usage of this function will in python be `x = Round(5.5)`

```CS
public class MyFunction : Compiler.Function {
    public MyFunction() {
        this.name = "Round";
        this.inputParameterAmount.Add(1);
        this.pauseWalker = false;
        this.hasReturnVariable = true;
    }
}
```

---

## The `runFunction` method

When the user calls your function _(as defined by the `name`)_ the compiler will execute the `runFunction` method.

- The `runFunction` method must be defined as such:
```CS
public override Variable runFunction(Scope currentScope, Variable[] inputParas, int lineNumber) { }
```

- The `runFunction` method must return a value of type `Variable`.
Even if you intend on not returning any values you must return a `None` which is done by making a new variable with only a name (no value). For example `new Variable("None")`

### Example

> Continuing with the example of creating a rounding function.<br>
> _**Note:** Here we're using `Math` (System) and not `Mathf` (UnityEngine) because `Variable.getNumber()` gives us a `double` which `Mathf.Round` can't handle._

```CS
public class MyFunction : Compiler.Function {
    public MyFunction() {
        this.name = "Round";
        this.inputParameterAmount.Add(1);
        this.pauseWalker = false;
        this.hasReturnVariable = true;
    }

    public override Variable runFunction(Scope currentScope, Variable[] inputParas, int lineNumber) {

        double number = inputParas[0].getNumber();
        double rounded = Math.Round(number);

        return new Variable("Rounded value", rounded);
    }
}
```

### Result

![Rounding function in action](/images/roundfunc-example.png)

> _**Note:** You also have to register the function to the compiler for the function to be usable. You do this via the wrappers `SetCompilerFunctions` function. More info about that can be found at the [`3.2.`&nbsp;Accessing compiler](/content/3-compiler.md#setting-environment-functions) page._

---

## Pausing functions

Some functions may need longer time to process than an instant. For example you want an animation to finish playing before moving on in the code. This is where pausing the CodeWalker comes in.

To make a function pause the CodeWalker, start by setting the `pauseWalker` value to `true`.

Then to resume the CodeWalker you call the wrappers `UnpauseWalker` function.

### Specifications for `PMWrapper.UnpauseWalker`

> Scope: `PMWrapper`<br>
> Definition: `public static void UnpauseWalker()`<br>
> Available since version: `0.5`<br>

### Example

```CS
public class MyScript : MonoBehaviour {
    [NonSerialized]
    public float movingRemaining;

    private void Start() {
        PMWrapper.SetCompilerFunctions(new MyFunction(this));
    }

    private void Update() {
        if (movingRemaining > 0) {
            movingRemaining -= Time.deltaTime;
            transform.position += Vector3.right * Time.deltaTime;

            if (movingRemaining <= 0)
                PMWrapper.UnpauseWalker();
        }
    }
}

public class MyFunction : Compiler.Function {
    private MyScript myScript;

    public MyFunction(MyScript myScript) {
        this.name = "move_right";
        this.inputParameterAmount.Add(0);
        this.hasReturnVariable = false;
        this.pauseWalker = true;

        this.myScript = myScript;
    }

    public override Variable runFunction(Scope currentScope, Variable[] inputParas, int lineNumber) {
        myScript.movingRemaining = 2;

        return new Variable("None");
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
