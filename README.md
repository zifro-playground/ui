# Zifro Playground UI

## For _Unity Package Manager_

This is a release meant for be used via UPM (Unity Package Manager).

The Zifro Playground UI is dependent on [Zifro Mellis Compiler](https://github.com/zardan/compiler).

## Installation

Minimal Unity version: `2018.3.10f1` or `2019.1.0b9`

Add the following inside the `dependencies` object in your `Packages/manifest.json`:

```json
    "se.zifro.mellis": "https://github.com/zardan/compiler.git#upm",
    "se.zifro.ui": "https://github.com/zardan/ui.git#upm",
```

For SSH, please refer to [this forum post by okcompute_unity](https://forum.unity.com/threads/git-support-on-package-manager.573673/#post-3819487).

> **Note**: Both repositories are (at time or writing) private, therefore you must be logged in to `git` on your local machine when UPM updates.
>
> UPM does not ask for credentials so they must be cached or have a SSH key on your computer. If you're not logged in it will fail on downloading the packages.
