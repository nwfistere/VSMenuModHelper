# Optional Example
An example of a mod that optionally loads VSModHelper if it is also installed.

This mod will render a tab in the options menu if VSMenuModHelper is installed in the mods folder. If not, it will still read in the preferences and save them when the game closes.

Try setting buttonPressed to false in the MelonPreferences.cfg file prior to starting the game without VSMenuModHelper. Then see how it get's set to true when the game is started.

## Optional Dependency Requirements
When creating a mod that optionally uses VSMenuModHelper, you must keep all references to VSMenuModHelper out of files that have your required code. Otherwise your mod will attempt to load VSMenuModHelper classes when it might not be installed. Creating a facade for any objects to interact with the menu is strongly recommeneded.

## Notes
### When running this example without MenuModHelper, you'll see the following error in the console:
```
...
[07:49:13.360] Failed to load Melon Assembly from '<Path-to-Mods>\VSMenuModHelper.dll':
System.IO.FileNotFoundException: Could not find file '<Path-to-Mods>\VSMenuModHelper.dll'.
File name: '<Path-to-Mods>\VSMenuModHelper.dll'
<Stacktrace here>
[07:57:23.692] [Optional_Example] Not Found - VSMenuModHelperAssembly
```
The stacktrace is produced by MelonLoader, and not an error. The Mod will work as expected.

### If you see the following warning:
```
[07:57:22.621] Some Melons are missing dependencies, which you may have to install.
If these are optional dependencies, mark them as optional using the MelonOptionalDependencies attribute.
This warning will turn into an error and Melons with missing dependencies will not be loaded in the next version of MelonLoader.
- 'Optional Example' is missing the following dependencies:
    - 'VSMenuModHelper' v1.1.0.0
```
You need to add VSMenuModHelper as an optional dependency in your assembleInfo. E.G.:
```csharp
[assembly: MelonOptionalDependencies("VSMenuModHelper")]
```
