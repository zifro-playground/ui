[&lt; Back to table of contents](/README.md)

---

# `5.2.` _Miscellaneous_ - **Using events**

Using events is really easy as they're all now in one collected place.

> _"Heh, yea, in the global namespace"  
> "HUSH! Let me believe they're collected!"_

The new events that has come with PythonMachine-UI version `0.5` is based of C# interfaces.

---

## Interfaces 101

`.NET` is an object based library, which makes `C#` an object oriented language.<br>
Bottom line is that we have objects that inherit from each other.

You probably know that in `.NET` a class may only inherit from ONE other class.

Interfaces however, which can also be used for inheritage, does not have a silly _"CAN ONLY INHERIT FROM 1"_ rule.

This leaves us with neat possibilities such as in this case using them for event, without needing to register the events. Cool huh?

> _One guideline in .NET/C# when it comes to interfaces is that they should always start with the uppercase letter `i` to indicate that it's an interface. So the prefix of the event interfaces stands for `I`nterface&nbsp;`P`ython`M`achine._

---

## Finding the interface definition

Let's take `IPMCompilerStarted` as an example; it contains the definition of a method called `OnPMCompilerStarted` that you must implement if you inherit from the interface. How do you know which method that needs to be implemented? There's a couple of options for you:

- It's written here in the DOCUMENTATION for each interface, see the _"Definition"_ segment of any event.
- If you posses an editor with IntelliSense, for example Visual&nbsp;Studio or MonoDevelop, big chance you can just let them tell you by the definition. For example just right click on `IPMCompilerStarted` that you just wrote on your class and select _"Go to Declaration"_ or something similar to see this being viewed on your screen:
```CS
public interface IPMCompilerStarted {
	void OnPMCompilerStarted();
}
```
- Last option, which you can always fall back on if you forget an events name, and that is to just look inside the `WrapperEvents.cs` file that's located here:<br>
`Assets/_Pythonmaskinen/UI Wrapper/WrapperEvents.cs`<br>
In there you will see exactly the same definition as stated in the list item above.

---

## Implementing an interface

If you've read the [`1.4.`&nbsp;Getting started](/content/1-getting-started.md) page, which I presume you have _(if not go read it now)_, you probably know how to implement an interface. It's not difficult at all.

But let's go through it anyways.

We have two major options to choose from, either doing a normal implementation of the members in the interface, or doing a so called _"explicit"_ implementation.

To do an implementation, you pretty much just put a `public` on everything it requires you to add. Lika à so:
```CS
public class MyScript : MonoBehaviour, IPMCompilerStarted {
    public void OnPMCompilerStarted() {

    }
}
```

To do an _"explicit"_ implementation, you need to specify the origin interface in the definition, but adding `public` is no longer a requirement. Something like this:
```CS
public class MyScript : MonoBehaviour, IPMCompilerStarted {
    void IPMCompilerStarted.OnPMCompilerStarted() {

    }
}
```

---

## All events

Examples of each event can be found all around the DOCUMENTATION.  
~~Here's a full list of all the events:~~

The full list has been moved to the [`5.1.`&nbsp;Miscellaneous - Reference](/content/5-reference.md) page.

---

[&lt; Back to table of contents](/README.md)
