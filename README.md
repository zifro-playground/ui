# Zifro Playground UI

## For _Unity Package Manager_

This is a release meant for be used via UPM (Unity Package Manager).

## Installation

Minimal Unity version: `2018.3.10f1` or `2019.1.0b9`

### 1. Add package

The Zifro Playground UI is dependent on [Zifro Mellis Compiler](https://github.com/zardan/compiler).
So both Playground UI and Mellis must be added.

To do so, add the following inside the `dependencies` object in your `<project root>/Packages/manifest.json`:

If unsure of which variant to use, go for the HTTPS variant.

#### 1.1 HTTPS variant

```diff
@@ Packages/manifest.json @@
{
  "dependencies": {
+   "se.zifro.mellis": "https://github.com/zardan/compiler.git#upm",
+   "se.zifro.ui": "https://github.com/zardan/ui.git#upm",
    "com.unity.analytics": "3.2.2",
    "com.unity.collab-proxy": "1.2.16",
    "com.unity.package-manager-ui": "2.0.7",
    "com.unity.modules.ai": "1.0.0",

    /* ... rest of Unity packages ... */
  }
}
```

#### 1.2 SSH variant

```diff
@@ Packages/manifest.json @@
{
  "dependencies": {
+   "se.zifro.mellis": "ssh://git@github.com/zardan/compiler.git#upm",
+   "se.zifro.ui": "ssh://git@github.com/zardan/ui.git#upm",
    "com.unity.analytics": "3.2.2",
    "com.unity.collab-proxy": "1.2.16",
    "com.unity.package-manager-ui": "2.0.7",
    "com.unity.modules.ai": "1.0.0",

    /* ... rest of Unity packages ... */
  }
}
```

You'll probably be using the SSH variant for easier CI/CD integration.

> **Note**: Both repositories are (at time or writing) private, therefore you must be logged in to `git` on your local machine when UPM updates.
>
> UPM does not ask for credentials so they must be cached or have a SSH key on your computer. If you're not logged in it will fail on downloading the packages.

### 2. Enable embedded resource (if targetting WebGL)

[Zifro Mellis compiler](https://github.com/zardan/compiler) relies heavily on embedded resources when it comes to localized exceptions.
In Unity, embedded resources are disabled by default in WebGL builds.

#### 2.1 Enable via script

[Josh Peterson recommends](https://forum.unity.com/threads/enabling-embedded-resources-with-webgl.326069/) enabling the settings via an editor script.
To do so, create the following script in an editor folder (ex: `<project root>/Assets/Editor/WebGLEditorScript.cs`):

```cs
using UnityEditor;
public class WebGLEditorScript
{
    [MenuItem("WebGL/Enable Embedded Resources")]
    public static void EnableErrorMessageTesting()
    {
        PlayerSettings.SetPropertyBool("useEmbeddedResources", true, BuildTargetGroup.WebGL);
    }
}
```

Then from the menu bar follow "WebGL > Enable Embedded Resources". Clicked once, and the setting should be active.

#### 2.2 Enable manually in file

> This operation can only be achived if your project settings are serialized in text mode.

Change the `webGLUseEmbeddedResources` setting to `1` inside your `<project root>/ProjectSettings/ProjectSettings.asset`

```diff
@@ ProjectSettings/ProjectSettings.asset @@ PlayerSettings:
   webGLModulesDirectory:
   webGLTemplate: APPLICATION:Default
   webGLAnalyzeBuildSize: 0
-  webGLUseEmbeddedResources: 0
+  webGLUseEmbeddedResources: 1
   webGLCompressionFormat: 1
   webGLLinkerTarget: 1
   webGLThreadsSupport: 0
```

## Updating

There's at time of writing no way to update via the Package Manager UI.

To do it manually, remove the lock of `se.zifro.ui` in your `<project root>/Packages/manifest.json`

```diff
@@ Packages/manifest.json @@
{
  "dependencies": {
    "se.zifro.mellis": "ssh://git@github.com/zardan/compiler.git#upm",
    "se.zifro.ui": "ssh://git@github.com/zardan/ui.git#upm",
    "com.unity.analytics": "3.2.2",
    "com.unity.collab-proxy": "1.2.16",
    "com.unity.package-manager-ui": "2.0.7",
    "com.unity.modules.ai": "1.0.0",

    /* ... rest of Unity packages ... */
  },
  "lock": {
    "se.zifro.mellis": {
      "hash": "8a61a6b637735194520a896ed3bafc322f7710ce",
      "revision": "upm"
    },
-    "se.zifro.ui": {
-      "hash": "9b9871e395be2f53e53f830af9ab586ee6e1ee3d",
-      "revision": "upm"
-    }
  }
}
```

Then jump back into Unity and watch it gather the latest release.

---

Zifro Playground UI is developed, maintained, and owned by © Zifro AB ([zifro.se](https://zifro.se/))
