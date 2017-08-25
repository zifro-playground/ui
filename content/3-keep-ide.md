[&lt; Back to table of contents](/README.md)

---

# `3.6.` _IDE/Compiler_ - **Keep IDE between scenes**

Let's say you wish to use `IPMLevelChanged` to load a different scene. It's easy to setup and everything, but you are forced to have the multiple IDEs, one in each scene. And upon switching you notice that the IDE resets (levels, code, errything). Not very pleasant if you ask me.

There's multiple solutions for this; such as loading each scene in [`LoadSceneMode.Additive` mode (link to Unity docs)](https://docs.unity3d.com/ScriptReference/SceneManagement.LoadSceneMode.Additive.html), or you could set the IDE to not disappear upon loading a new scene. What these methods have in common is they both make sure the IDE is "persistent", stays the same, doesn't reset.

Here we will go through both options.

---

## "Additive loading"

This one is pretty simple, both in theory and in practise.

What we can do in Unity is to have multiple scenes loaded at the same time. *Cool huh?*
We do this by specifying `Additive` mode upon loading a scene.

> _**Note:** You can even have multiple scenes loaded in your editor! Try drag'n'dropping multiple scenes into the Hierarchy. (To remove them again you can right click them in the hierarchy and press remove)_

### Example

> In this example we have one scene which contains everything we want to have existing in each scene. Such as this `MyLevelLoader` script.
>
> _**Note:** The `OnPMLevelChanged` event is even called at the beginning as the first level is set, so loading scenes with it is quite optimal._

```CS
public class MyLevelLoader : MonoBehaviour, IPMLevelChanged {
    private void IPMLevelChanged.OnPMLevelChanged() {
        // Remember to unload the previous scene!
        if (PMWrapper.previousLevel != -1)
            SceneManager.UnloadSceneAsync("LEVEL " + PMWrapper.previousLevel);
            
        SceneManager.LoadScene("LEVEL " + PMWrapper.currentLevel, LoadSceneMode.Additive);
    }
}
```

---

## "Don't Destroy On Load"

The main difference between this method and the one mentioned above, *Additive loading*, is that in loading additivly you have an entire scene staying persistent while in this example you only have the IDE staying persitent.

To make an object not get destroyed when the scene its unloaded, you mark it with the `DontDestroyOnLoad` method. How can we do this for the IDE? Well, simply call the wrappers `DontDestroyIDEOnLoad` function.

### Specifications for `PMWrapper.DontDestroyIDEOnLoad`

> Scope: `PMWrapper`<br>
> Definition: `public static void DontDestroyIDEOnLoad()`<br>
> Available since version: `0.5.4`<br>

### Example

> In this example our setup is a tad custom. To start with, we don't want multiple IDE's to spawn in each time we enter a new scene. *(Because if each scene we load contains the IDE, and we mark each one with DontDestroyOnLoad, we would get a lot of IDEs in no-time)*
>
> To handle this, we will just have a scene with **ONLY** the IDE, and a script that at `Start()` marks the IDE as _DontDestroyOnLoad_ as well as hops to the appropriate levels. 

```CS
public class MyGameLoader : MonoBehaviour, IPMLevelChanged {
    private void Start() {
        PMWrapper.DontDestroyIDEOnLoad();
        // If not placed on the IDE, we must also mark ourselves with DontDestroyOnLoad.
        // That way we can keep listening for OnPMLevelChanged events.
        DontDestroyOnLoad(gameObject);
    }
    private void IPMLevelChanged.OnPMLevelChanged(int level) {
        SceneManager.LoadScene("LEVEL " + level);
    }
}
```

---

[&lt; Back to table of contents](/README.md)
