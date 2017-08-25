[&lt; Back to table of contents](/README.md)

---

# `1.2.` _Setup_ - **Updating from 0.4 and below**

It is a big chance you've hooked your own code into the PythonMaskinen-UI package. There's nothing morally wrong in that, it's just that it's a hazzle hacking the PythonMachine each time you have to update it. This is where the big change of version `0.5` comes in. It's aimed to eliminate the most common uses of these hacks and to unite functionality in the products made with the PythonMachine, such as levels and hints.

---

## Step 1 - Backups

This is where things gets interesting. You see, the main structure of the PythonMachine is changed quite a bit in version `0.5`, so it is advised to follow this guide so no major conflicts occur when upgrading.

Please make a .zip or similar backup before updating. It's a big chance some of your work you put into hacking the machine that will go lost in the upgrading process.

---

## Step 2 - Remove old version

In each update it is important to remove the old one so no files are left straying among your assets.

From version `0.5` the PythonMachine will keep its files in a sub folder, called `_Pythonmaskinen/`. This makes updating easy for you have to only remove one folder.

Please locate all files in your project that comes from the PythonMachine and remove them. Here are some folders that the PythonMachine has occupied before version `0.5`:
```
Assets/CodeWalker/
Assets/Demo/
Assets/NewIDE/
Assets/Prefabs/
Assets/plugins/
```

Be careful when deleting, as you may also have files in these folder, for example the `Prefabs/` folder.

---

## Step 3 - Download

If you haven't already, you should get your hands on the latest release of the **PythonMachine UI**. Download can be found at the [Releases tab (link)](https://github.com/HelloWorldSweden/PythonMaskinen-UI/releases).

---

## Step 4 - Import package

Importing a package into Unity is super simple. Personally, I just prefer drag'n'dropping the package file into the Unity window, but can also be done by going into the `Assets` tab, choosing `Import Package`, and then `Custom Package...`

![Guide on importing package, image 1](/images/import-howto-1.png)

After that you select the file `PyhtonMaskinen.UI.unitypackage` from your file system, and when Unity gives you the option to select what to import, you just press `Import` to import everything.

![Guide on importing package, image 2](/images/import-howto-2.png)

Now you may have to wait a minute while Unity does all the hard work of compiling the IDE, but once the loading bar has disappeared you're ready to go!

---

## Step 5 - Handling errors

When updating you probably get your console filled with red messages. This is because a lot of functions and fields from the old `0.4` version have been marked as `Obsolete`.

This means they have been deprecated, i.e. they're old stuff and needs other functions needs to be used instead. Luckily, each deprecated function has been marked with the info of where to find the replacement. So what you do is you just go through the list of console errors and fix each one.

You may wish to visit the [`1.4.` Getting started](/content/1-getting-started.md) page for getting an understanding of the new PythonMachine structure.

### Example

Where you maybe previously used
```CS
Camera.main.GetComponent<HelloCompiler> ().setAddedFunctions(
    new List<Function> {
        new moveForward(),
        new turnRight(),
        new turnLeft(),
        new collect()
    }
);
```
You now want to use
```CS
PMWrapper.SetCompilerFunctions(
    new moveForward(),
    new turnRight(),
    new turnLeft(),
    new collect()
);
```

---

## Step 6 - Getting started

Even though you may have used the PythonMachine before, lot's have changed from `0.4` to `0.5`. Therefore, it's recommended to visit the [`1.4.` Getting started](/content/1-getting-started.md) page at least once.

---

[&lt; Back to table of contents](/README.md)
