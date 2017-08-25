[&lt; Back to table of contents](/README.md)

---

# `3.3.` _IDE/Compiler_ - **Speed multiplier**

_The almighty speed multiplier._

Yes so you've probably noticed the slider in the PythonMachine UI. That slider determines the speed of the execution. The more you slide it to the right, the faster the execution. The more you drag it to the left, the slower the execution.

But! The PythonMachine doesn't handle this timewarping all on it's own, since you maybe have some precious parts in your game that doesn't really work well under the influence of for example a `Time.timeScale` change, it was decided to leave this up to the game programmers.

---

## Accessing the speed multiplier

_"Hw do I git da value of dis awsum slidr den?"_

Welp, I'm glad you asked. For I've prepared both events and properties to scale your game against time! _(It's actually only one event and one property, but making it plural sounded more dramatic)_ You may find these tools under the names of `OnPMSpeedChanged` and `speedMultiplier`.

> _**Note:** the value of both `speedMultiplier` and the argument in `OnPMSpeedChanged` ranges from `0` (leftmost) to `1` (rightmost) with `0.5` (center) being the default value._

### Specifications of `OnPMSpeedChanged`

> Interface: `IPMSpeedChanged`<br>
> _~~Definition: `void OnPMSpeedChanged(float value)`~~ PARAMETER REMOVED IN 0.7.1_<br>
> _~~Definition: `void OnPMSpeedChanged()`~~ PARAMETER READDED IN 0.7.2_<br>
> Definition: `void OnPMSpeedChanged(float speed)`<br>
> Available since version: `0.5`<br>

### Specifications of `PMWrapper.speedMultiplier`

> Scope: `PMWrapper`<br>
> _~~Definition: `public static float speedMultiplier { get; }`~~ CHANGED IN 0.7.1_<br>
> Definition: `public static float speedMultiplier { get; set; }`<br>
> Available since version: `0.5`<br>

### Example `1`

> Example using the `speedMultiplier` property and a time counter of our own.

```CS
public class MyScript : MonoBehaviour {
    private float timePassed = 0;

    private void Update() {
        timePassed += Time.deltaTime * PMWrapper.speedMultiplier;
        transform.position = Vector3.right * Mathf.Sin(timePassed * 10);
    }
}
```

### Example `2`

> Example using the `OnPMSpeedChanged` event and `Time.timeScale`.

```CS
public class MyScript : MonoBehaviour, IPMSpeedChanged {
    public void OnPMSpeedChanged() {
        Time.timeScale = PMWrapper.speedMultiplier;
    }

    private void Update() {
        transform.position = Vector3.right * Mathf.Sin(Time.time * 10);
    }
}
```

### Result

> Identical result for both example `1` and example `2`.

![Sinus wave scaled to speed multiplier](/images/speedmultiplier-example.gif)

---

[&lt; Back to table of contents](/README.md)
