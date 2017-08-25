[&lt; Back to table of contents](/README.md)

---

# `3.1.` _IDE/Compiler_ - **Code window**

The code window is where the user can type their code. It is also the main component of the **IDE**.

## Its three parts

### The pre code

The pre code is added to be compiled among with the other code parts. The pre code, as its name suggests, is added BEFORE the main code.

The pre code cannot be edited by the user.

### The main code

The main code is the part the user gets to interact with.

### The post code

The post code is added to be compiled among with the other code parts. The post code is added, *yea you guessed it*, AFTER the main code.

The post code cannot be edited by the user.

> _**Note:** post code is not yet fully supported by the IDE and wrapper. Take caution in using it._

---

## Accessing the code

Getting and setting each parts of the code is done via the wrapper. There are four properties, one for each code part (`preCode`, `mainCode`, and `postCode`) and one for them combined (`fullCode`).

There's also a function available for adding text to the `mainCode` where the user currently holds their cursor: `AddCode` _(If `smartButtony = true` it's identical to what the [SmartButtons](/content/3-smart-buttons.md) are doing, where they add new lines and indentation if needed)_

### Specifications for `PMWrapper.preCode`

> Scope: `PMWrapper`<br>
> Definition: `public static string preCode { get; set; }`<br>
> Available since version: `0.5`<br>

### Specifications for `PMWrapper.mainCode`

> Scope: `PMWrapper`<br>
> Definition: `public static string mainCode { get; set; }`<br>
> Available since version: `0.5`<br>

### Specifications for `PMWrapper.postCode`

> Scope: `PMWrapper`<br>
> Definition: `public static string postCode { get; set; }`<br>
> Available since version: `0.5`<br>
>
> _**Note:** post code is not yet fully supported by the IDE and wrapper. Take caution in using it._

### Specifications for `PMWrapper.fullCode`

> Scope: `PMWrapper`<br>
> Definition: `public static string fullCode { get; }`<br>
> Available since version: `0.6`<br>
>
> _**Note:** post code is not yet fully supported by the IDE and wrapper. Take caution in using it._

### Specifications for `PMWrapper.AddCode`

> Scope: `PMWrapper`<br>
> Definition: `public static void AddCode(string text, bool smartButtony = false)`<br>
> Available since version: `0.5`<br>

### Example

```CS
public class MyScript : MonoBehaviour {
  	private void FixedUpdate() {
  		  PMWrapper.preCode = "x = " + Random.Range(0, 10);
  	}
}
```

### Result

![Precode constantly changing](/images/precode-changing.gif)

---

## Code rows limit

The amount of rows the user is allowed to enter in the code window is very changeable. Just change some values on the `codeRowsLimit` property.


### Specifications for `PMWrapper.codeRowsLimit`

> Scope: `PMWrapper`<br>
> Definition: `public static int codeRowsLimit { get; set; }`
> Available since version: `0.5`
>
> _**Note:** Setting a value lower than the current number of rows the user has typed will result in buggy behaviour. Be aware!_

---

[&lt; Back to table of contents](/README.md)
