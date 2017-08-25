[&lt; Back to table of contents](/README.md)

---

# `2.4.` _GUI_ - **Custom error messages**

Maybe you wish to have an educational error message. For example _"Hey you can't do that, try this instead: &lt;insert tip here&gt;"_. You better not be throwing a lot of `System.Exception`s, 'cause I know I did.

What you want to be using is the wrappers `RaiseError` function.

> _**Note:** The `RaiseError` actually throws an error. This is to stop the code that executes it. Consider that while using it that no code after its call will execute._

### Specifications for `PMWrapper.RaiseError` _(from 0.5)_

> ~~Scope: `PMWrapper`<br>
> Definition: `public static void RaiseError(int rowNumber, string message)`<br>
> Available since version: `0.5`<br>~~
> _**DEPRECATED!** See below!_

### Specifications for `PMWrapper.RaiseError` _(since 0.7)_

> Scope: `PMWrapper`<br>
> Definitions:<br>
> -  `public static void RaiseError(string message)`
> -  `public static void RaiseError(int rowNumber, string message)`
> -  `public static void RaiseError(Vector2 canvasPosition, string message)`
> -  `public static void RaiseError(Vector3 worldPosition, string message)`
> -  `public static void RaiseError(RectTransform rect, string message)`
> -  `public static void RaiseError(Selectable rect, string message)`
>
> Available since version: `0.7`<br>

So there's a lot of overloads for this function. Let's go through all of them:

- `RaiseError(string)`:<br>Shows an error message on **the current row** in the walker.

- `RaiseError(int, string)`:<br>Shows an error message on **a specific row** in the walker.

- `RaiseError(Vector2, string)`:<br>Shows an error message at a specific coordinate on the UI. Where `Vector2(-800,450)` is the upper left corner and `Vector2(800,-450)` is the lower right corner.

- `RaiseError(Vector3, string)`:<br>Shows an error message pointing towards a coordinate in world space. Note that you MUST have the game camera marked with the `MainCamera` tag to use this.

- `RaiseError(RectTransform, string)`:<br>Similar to the canvas position one, but it fetches the RectTransform's canvas position based of its pivot.

- `RaiseError(Selectable, string)`:<br>Identical to the RectTransform one.

### Example

```CS
public class MyFunction : Compiler.Function {

    public MyFunction() {
        this.name = "myFunction";
        this.inputParameterAmount.Add(1);
        this.pauseWalker = false;
        this.hasReturnVariable = false;
    }

    public override Variable runFunction(Scope currentScope, Variable[] inputParas, int lineNumber) {

        if (inputParas[0].variableType != VariableTypes.number)
            PMWrapper.RaiseError("Wrong value type! Function only takes numbers");

        return new Variable("NULL");
    }

}
```

### Result

![Error message appears with our custom error message](/images/raiseerror-example.png)

---

[&lt; Back to table of contents](/README.md)
