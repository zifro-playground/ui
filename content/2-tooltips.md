[&lt; Back to table of contents](/README.md)

---

# `2.5.` _GUI_ - **Adding tooltips**

Since the release of version `0.6` we got the addition of tooltips for buttons! If you wish to add a tooltip, it's really easy!

---

## Setup

All you need to do is add the `UI Tooltip` component to the UI element you wish to have a tooltip for.

### Specifications for `UITooltip`

> Scope: `global`<br>
> Available since version: `0.6`<br>

### Example

Adding a script to a GameObject should be common knowledge. But if you're uncertain, here's how:

![Add UITooltip component](/images/tooltip-add.png)

Then from there you can just change the settings all you want in the inspector.

![UITooltip component](/images/tooltip-settings.png)

> _**Note:** the `offset` field has been deprecated since version `0.7` for now it follows the mouse instead._

### Result

> _**Note:** This is obviously more than one tooltip in action, but I've just repeated the setup step for multiple buttons. Nothing fancy._

![Tooltips in action](/images/tooltip-example.gif)

---

## Changing settings via script

Changing the tooltips is nice, but what if you wish to change it during runtime, via script?

Well, fear not, because `ApplyTooltipTextChange` is here for you!

### Example

```CS
UITooltip tooltip = GetComponent<UITooltip>();
tooltip.text = "Foo bar";
tooltip.ApplyTooltipTextChange();
```

---

[&lt; Back to table of contents](/README.md)
