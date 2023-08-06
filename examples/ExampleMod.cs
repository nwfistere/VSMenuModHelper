using HarmonyLib;
using Il2CppVampireSurvivors.UI;
using MelonLoader;
using System;
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
        private MelonPreferences_Category? preferences;
        private static MelonPreferences_Entry<bool> enabled;
        private static MelonPreferences_Entry<bool> someToggle;
        private static MelonPreferences_Entry<float> somePercentage;
        private static MelonPreferences_Entry<bool> buttonPressed;
        private static MelonPreferences_Entry<int> dropDownValue;
        private static MelonPreferences_Entry<int> multipleChoiceValue;

        private static MenuHelper MenuHelper;

        public override void OnInitializeMelon()
        {
            preferences = MelonPreferences.CreateCategory("example_preferences");
            enabled = preferences.CreateEntry("enabled", true);
            someToggle = preferences.CreateEntry("someToggle", true);
            somePercentage = preferences.CreateEntry("somePercentage", 1f);
            buttonPressed = preferences.CreateEntry("buttonPressed", false);
            dropDownValue = preferences.CreateEntry("dropDownValue", 0);
            multipleChoiceValue = preferences.CreateEntry("multipleChoiceValue", 0);

            MenuHelper = new();
            DeclareMenuTabs(MenuHelper);
        }

        [HarmonyPatch("Il2CppInterop.HarmonySupport.Il2CppDetourMethodPatcher", "ReportException")]
        public static class Patch_Il2CppDetourMethodPatcher
        {
            public static bool Prefix(System.Exception ex)
            {
                MelonLogger.Error("During invoking native->managed trampoline", ex);
                return false;
            }
        }

        private void LogValues()
        {
            Action<string> log = (str) => LoggerInstance.Msg($"\t{str}");
            LoggerInstance.Msg($"preferences:");
            log($"enabled: {enabled.Value}");
            log($"someToggle: {someToggle.Value}");
            log($"somePercentage: {somePercentage.Value}");
            log($"buttonPressed: {buttonPressed.Value}");
            log($"dropDownValue: {dropDownValue.Value}");
            log($"multipleChoiceValue: {multipleChoiceValue.Value}");
        }

        private void DeclareMenuTabs(MenuHelper MenuHelper)
        {
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "resources", "example", "some-icon.png");
            Uri imagePath2 = new("https://github.com/nwfistere/VSMenuModHelper/blob/main/examples/resources/example/game-controller.png");

            MenuHelper.DeclareTab("Config Tab", imagePath);
            MenuHelper.DeclareTab("Empty Tab", imagePath2);

            MenuHelper.AddElementToTab("Config Tab", new TickBox("enabled", () => enabled.Value, (value) => enabled.Value = value));
            MenuHelper.AddElementToTab("Config Tab", new TickBox("someToggle", () => someToggle.Value, (value) => someToggle.Value = value));
            MenuHelper.AddElementToTab("Config Tab", new LabeledButton("LabeledButton", "log values", () => LogValues()));
            MenuHelper.AddElementToTab("Config Tab", new Slider("Slider", () => somePercentage.Value, (value) => somePercentage.Value = value));
            MenuHelper.AddElementToTab("Config Tab", new DropDown("DropDown", new() { "one", "two", "three" }, () => dropDownValue.Value, (value) => dropDownValue.Value = value));
            Action<int> action = (value) => multipleChoiceValue.Value = value;
            MenuHelper.AddElementToTab("Config Tab", new MultipleChoice("DropDown", new() { "one", "two", "three" }, new() { () => action(0), () => action(1), () => action(2) }, () => multipleChoiceValue.Value));
        }

        // Must be added to consume the library. This adds the Tabs to the game.
        [HarmonyPatch(typeof(OptionsController))]
        class Example_OptionsController_Patch
        {
            // Used for advanced GetTabSprite_Postfix
            static Sprite AlterSpriteHelper(Sprite sprite)
            {
                if (sprite.name == "Empty Tab")
                {
                    sprite.texture.filterMode = FilterMode.Point;
                }
                return sprite;
            }

            [HarmonyPatch(nameof(OptionsController.Construct))]
            [HarmonyPrefix]
            static void Construct_Prefix() => MenuHelper.Construct_Prefix();

            [HarmonyPatch(nameof(OptionsController.Initialize))]
            [HarmonyPrefix]
            static void Initialize_Prefix(OptionsController __instance) => MenuHelper.Initialize_Prefix(__instance);

            // Basic usage:
            //[HarmonyPatch(nameof(OptionsController.GetTabSprite))]
            //[HarmonyPostfix]
            //static void GetTabSprite_Postfix(OptionsTabType t, ref Sprite __result) => __result = MenuHelper.OnGetTabSprite(t) ?? __result;

            // Advanced usage to alter the sprite prior to sending to OptionsController
            [HarmonyPatch(nameof(OptionsController.GetTabSprite))]
            [HarmonyPostfix]
            static void GetTabSprite_Postfix(OptionsTabType t, ref Sprite __result) => __result = MenuHelper.OnGetTabSprite(t, AlterSpriteHelper) ?? __result;

            [HarmonyPatch(nameof(OptionsController.BuildPage))]
            [HarmonyPrefix]
            static bool BuildPage_Prefix(OptionsController __instance, OptionsTabType type) => MenuHelper.OnBuildPage(__instance, type);
        }
    }
}
