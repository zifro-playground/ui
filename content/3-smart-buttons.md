[&lt; Back to table of contents](/README.md)

---

# `3.4.` _IDE/Compiler_ - **Smart Buttons**

_Smart buttons!! Yeaaa they rock!_

Don't know which buttons I'm talking about? Well it's these of course:

![Smart buttons demo](/images/smartbuttons-example.gif)

Just below the compiler is a list of variables/functions that the user can use in the current level. If you press a button, it adds its content to the code window where your cursor currently is.

It helps the user seeing which functions are available. And if they keep misspelling it they can just press the button!

---

## Setting Smart Buttons

In the same way as setting compiler functions, but with just `string`s instead of `Function`s!

The functions to know of is the following wrapper functions: `SetSmartButtons`, `AddSmartButton`, and `AutoSetSmartButtons`.

- `SetSmartButtons` replaces all the existing buttons with a set of new ones.  
- `AddSmartButton` adds buttons to the existing pool of buttons.
- `AutoSetSmartButtons` replaces all the existing buttons, where the content is automatically taken from the functions defined with `SetCompilerFunctions`.

> _**Note:** `SetSmartRichButtons` and `AddSmartRichButton` have been marked as obsolete and will deprecate in an upcoming update, so consider using the other functions, for example  `SetSmartButtons`._

### Specifications for `PMWrapper.SetSmartButtons`

> Scope: `PMWrapper`<br>
> Definition: `public static void SetSmartButtons(params string[] codes)`<br>
> Available since version: `0.5`<br>

### Specifications for `PMWrapper.AddSmartButton`

> Scope: `PMWrapper`<br>
> Definition: `public static void AddSmartButton(string code)`<br>
> Available since version: `0.5`<br>

### Specifications for `PMWrapper.AutoSetSmartButtons`

> Scope: `PMWrapper`<br>
> Definition: `public static void AutoSetSmartButtons()`<br>
> Available since version: `0.6`<br>

### Example

```CS
public class MyScript : MonoBehaviour {
    private void Start() {
        PMWrapper.SetSmartButtons("move_right()", "move_left()", "move_up()", "move_down()");
    }
}
```

### Result

_See GIF at start of page._

---

## Smart _Rich_ Buttons?

>As of version `0.6` smart rich buttons has been marked as obsolete. They still are available but as a warning that they will soon be removed entirely.

![SmartRichButtons has been deprecated](/images/smartrichbuttons-deprecated.png)

---

[&lt; Back to table of contents](/README.md)
