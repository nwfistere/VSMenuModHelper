# VSMenuModHelper
A library for enabling modifying Vampire Survivors' menu

## Features
Create a new custom tab with custom:
 - icon
 - title
 - button
 - slider
 - tickbox
 - dropdown
 - multiple choice buttons

## Installation as dependency
This library is NOT a mod for Vampire Survivors. It's a library that mods can utilize. Installing only this to Vampire Survivors will do nothing.

### Requirements
 - New Engine version of Vampire Survivors
 - MelonLoader installed
 - A mod for Vampire Survivors that utilizes this library.

### Install
1. Download dll (link)
2. Install dll into Vampire Survivors' mods directory along with the Mod dependent on this library.


## Developers: How to consume
1. Download and Install library
2. Point Visual Studio at dll
3. Utilize library (see example)


## Example Usage
Assuming the mod is a MelonLoader Melon and uses Harmony:

### Create new tab in Options Menu
```C#
[HarmonyPatch(typeof(OptionsController))]
static class Example_OptionsController_Patch
{
    private static Tab CustomTab;

    [HarmonyPatch(nameof(OptionsController.GenerateNavigation))]
    [HarmonyPostfix]
    static void GenerateNavigation_Postfix() => Tab.OnGenerateNavigation();

    [HarmonyPatch(nameof(OptionsController.AddTabs))]
    [HarmonyPostfix]
    static void AddTabs_Postfix(OptionsController __instance, System.Reflection.MethodBase __originalMethod)
    {
        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "resources", "menu_example", "some-icon.png");
        CustomTab = new Tab("TestTab", SpriteImporter.LoadSprite(imagePath));
        CustomTab.OnTabCreation(__instance);

        System.Action addButton = () =>
        {
            __instance.AddLabeledButton("Labeled button", "button text", new System.Action(() => {
                Logger.Log("Button pressed!");
            }), false);

            __instance.AddSlider("Slider text", 1f, new System.Action<float>((value) => {
                Logger.Log($"New slider value {value}");
            }), false);

            __instance.AddTickBox("Tickbox text", false, new System.Action<bool>((value) => {
                Logger.Log($"New tickbox value {value}");
            }), false);

            List<string> DropDownList = new List<string>();
            DropDownList.Add("one"); DropDownList.Add("two"); DropDownList.Add("three");
            __instance.AddDropDown("Dropdown text", DropDownList, 0, new System.Action<int>((value) => {
                Logger.Log($"New dropdown value {DropDownList[value]}");
            }), 4, false);

            List<string> ButtonLabelList = new List<string>();
            ButtonLabelList.Add("one"); ButtonLabelList.Add("two"); ButtonLabelList.Add("three"); ButtonLabelList.Add("four");
            System.Action<int> MultipleChoiceValue = ((value) => { Logger.Log($"New MultipleChoiceValue value {value}"); });
            List<Il2CppSystem.Action> MultipleChoiceCBs = new List<Il2CppSystem.Action>();
            MultipleChoiceCBs.Add(new System.Action(() => MultipleChoiceValue(0)));
            MultipleChoiceCBs.Add(new System.Action(() => MultipleChoiceValue(1)));
            MultipleChoiceCBs.Add(new System.Action(() => MultipleChoiceValue(2)));
            MultipleChoiceCBs.Add(new System.Action(() => MultipleChoiceValue(3)));
            __instance.AddMultipleChoice("Multiple Choice", ButtonLabelList, MultipleChoiceCBs, 0, false);
        };

        CustomTab.OnBuildPage(addButton);
    }

    [HarmonyPatch(nameof(OptionsController.GetTabSprite))]
    [HarmonyPostfix]
    static void GetTabSprite_Postfix(OptionsTabType t, ref Sprite __result) => __result = Tab.OnGetTabSprite(t) ?? __result;

    [HarmonyPatch(nameof(OptionsController.BuildPage))]
    [HarmonyPrefix]
    static bool BuildPage_Prefix(OptionsController __instance, OptionsController.OptionsTabType type) => !Tab.OnBuildPage(type);

    [HarmonyPatch(nameof(OptionsController.GetTabName))]
    [HarmonyPostfix]
    static void GetTabName_Postfix(OptionsController __instance, OptionsController.OptionsTabType t, ref string __result) => __result = Tab.OnGetTabName(t) ?? __result;
}

```
### Produces!
![Custom menu](https://github.com/nwfistere/VSMenuModHelper/assets/9168048/e990c69c-17bb-4983-b530-1cd4ff4cc368)

## Known Bugs
 - After being in a game, the custom tab doesn't render any longer.
 - Tab icon isn't highlighted when tab is selected.

## TODO
 - Clean up interface
 - Add option for library to handle Harmony patches
 - Add option to alter existing tabs
 - **Add pause menu options**
 - Example mod in src
