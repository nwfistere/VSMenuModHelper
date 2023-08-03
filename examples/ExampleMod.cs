using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Il2CppVampireSurvivors.UI;
using MelonLoader;
using System.IO;
using UnityEngine;
using VSMenuHelper;
using static Il2CppVampireSurvivors.UI.OptionsController;

namespace ExampleMod
{
    public static class ModInfo
    {
        public const string Name = "Example";
        public const string Description = "Example mod for VSMenuModHelper.";
        public const string Author = "Nick";
        public const string Company = "Nick";
        public const string Version = "1.0.0";
        public const string Download = "https://github.com/nwfistere/VSMenuModHelper";
    }

    public class ExampleMod : MelonMod
    {
        private MelonPreferences_Category preferences;
        private static MelonPreferences_Entry<bool> enabled;
        private static MelonPreferences_Entry<bool> someToggle;
        private static MelonPreferences_Entry<float> somePercentage;
        private static MelonPreferences_Entry<bool> buttonPressed;
        private static MelonPreferences_Entry<string> dropDownValue;
        private static MelonPreferences_Entry<string> multipleChoiceValue;

        public override void OnInitializeMelon()
        {
            preferences = MelonPreferences.CreateCategory("example_preferences");
            enabled = preferences.CreateEntry("enabled", true);
            someToggle = preferences.CreateEntry("someToggle", true);
            somePercentage = preferences.CreateEntry("somePercentage", 1f);
            buttonPressed = preferences.CreateEntry("buttonPressed", false);
            dropDownValue = preferences.CreateEntry("dropDownValue", "");
            multipleChoiceValue = preferences.CreateEntry("multipleChoiceValue", "");
        }

        [HarmonyPatch(typeof(OptionsController))]
        class Example_OptionsController_Patch
        {
            private static Tab CustomTab;

            [HarmonyPatch(nameof(OptionsController.GenerateNavigation))]
            [HarmonyPostfix]
            static void GenerateNavigation_Postfix() => Tab.OnGenerateNavigation();

            [HarmonyPatch(nameof(OptionsController.AddTabs))]
            [HarmonyPostfix]
            static void AddTabs_Postfix(OptionsController __instance)
            {
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "resources", "example", "some-icon.png");
                CustomTab = new Tab("TestTab", SpriteImporter.LoadSprite(imagePath));
                CustomTab.OnTabCreation(__instance);

                void addButton()
                {
                    __instance.AddLabeledButton("Labeled button", "button text", new System.Action(() =>
                    {
                        Melon<ExampleMod>.Logger.Msg("Button pressed!");
                        buttonPressed.Value = !buttonPressed.Value;
                    }), false);

                    __instance.AddSlider("Slider text", 1f, new System.Action<float>((value) =>
                    {
                        Melon<ExampleMod>.Logger.Msg($"New slider value {value}");
                        somePercentage.Value = value;
                    }), false);

                    __instance.AddTickBox("Tickbox text", false, new System.Action<bool>((value) =>
                    {
                        Melon<ExampleMod>.Logger.Msg($"New tickbox value {value}");
                        someToggle.Value = value;
                    }), false);

                    List<string> DropDownList = new();
                    DropDownList.Add("one"); DropDownList.Add("two"); DropDownList.Add("three");
                    __instance.AddDropDown("Dropdown text", DropDownList, 0, new System.Action<int>((value) =>
                    {
                        Melon<ExampleMod>.Logger.Msg($"New dropdown value {DropDownList[value]}");
                        dropDownValue.Value = DropDownList[value];
                    }), 4, false);

                    List<string> ButtonLabelList = new();
                    ButtonLabelList.Add("one");
                    ButtonLabelList.Add("two");
                    ButtonLabelList.Add("three");
                    ButtonLabelList.Add("four");

                    System.Action<int> MultipleChoiceValue = ((value) => {
                        Melon<ExampleMod>.Logger.Msg($"New MultipleChoiceValue value {value}");
                        multipleChoiceValue.Value = ButtonLabelList[value];
                    });

                    List<Il2CppSystem.Action> MultipleChoiceCBs = new();
                    MultipleChoiceCBs.Add(new System.Action(() => MultipleChoiceValue(0)));
                    MultipleChoiceCBs.Add(new System.Action(() => MultipleChoiceValue(1)));
                    MultipleChoiceCBs.Add(new System.Action(() => MultipleChoiceValue(2)));
                    MultipleChoiceCBs.Add(new System.Action(() => MultipleChoiceValue(3)));

                    __instance.AddMultipleChoice("Multiple Choice", ButtonLabelList, MultipleChoiceCBs, 0, false);
                }

                CustomTab.OnBuildPage(addButton);
            }

            [HarmonyPatch(nameof(OptionsController.GetTabSprite))]
            [HarmonyPostfix]
            static void GetTabSprite_Postfix(OptionsTabType t, ref Sprite __result) => __result = Tab.OnGetTabSprite(t) ?? __result;

            [HarmonyPatch(nameof(OptionsController.BuildPage))]
            [HarmonyPrefix]
            static bool BuildPage_Prefix(OptionsTabType type) => !Tab.OnBuildPage(type);

            [HarmonyPatch(nameof(OptionsController.GetTabName))]
            [HarmonyPostfix]
            static void GetTabName_Postfix(OptionsTabType t, ref string __result) => __result = Tab.OnGetTabName(t) ?? __result;
        }
    }
}
