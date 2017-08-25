[&lt; Back to table of contents](/README.md)

---

# `5.3.` _Miscellaneous_ - **Using wrapper**

Ok let's wrap this up. _(hehehe, I find that funny, 'cause you know, it's about the wrapper, and it's the last page, so we're actually wrapping the docs up. Two points to me, check!)_

Serious mode: `ON`

Using the static wrapper is really simple. You don't have to find any references, nofin'. Just using the static properties and functions as wonderful as they are.

---

## Static 101

For those who have lived under a rock and are unfamiliar with the core concept of `static` can get their daily dose of explanation right here!

The keyword `static` can be applied to fields, properties, classes, and methods. What this does is that instead of having to keep track of a reference to _"which instance was it that had that variable"_, all instances reference the same variable. Like literally the same one. Same location in memory. Change one, change all.

Static methods/functions can be used in the exact same way: without an object instance reference.

Static members are accessed directly via writing the class type followed by the members name. For example:

```CS
class MyClass {
    static int myStaticVariable;

    public MyClass() {
        MyClass.myStaticVariable += 3;
    }
}
```

---

## All wrapper members

The full list has been moved to the [`5.1.`&nbsp;Miscellaneous - Reference](/content/5-reference.md) page.

---

[&lt; Back to table of contents](/README.md)
