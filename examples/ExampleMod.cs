using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Runtime.Remoting.Messaging;
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
        private MelonPreferences_Category preferences;
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
            string imagePath2 = Path.Combine(Directory.GetCurrentDirectory(), "resources", "example", "n-icon.png");

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

        [HarmonyPatch(typeof(OptionsController))]
        class Example_OptionsController_Patch2
        {

            [HarmonyPatch(nameof(OptionsController.Initialize))]
            [HarmonyPrefix]
            static void Initialize_Prefix(OptionsController __instance) => MenuHelper.Initialize_Prefix(__instance);

            [HarmonyPatch(nameof(OptionsController.GetTabSprite))]
            [HarmonyPostfix]
            static void GetTabSprite_Postfix(OptionsTabType t, ref Sprite __result) => __result = MenuHelper.OnGetTabSprite(t) ?? __result;

            [HarmonyPatch(nameof(OptionsController.BuildPage))]
            [HarmonyPrefix]
            static bool BuildPage_Prefix(OptionsController __instance, OptionsTabType type) => MenuHelper.OnBuildPage(__instance, type);

        }

        [HarmonyPatch(typeof(OptionsController))]
        class Example_OptionsController_Patch
        {
            [HarmonyPatch(nameof(OptionsController.Construct))]
            [HarmonyPrefix]
            static void Construct_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"Construct_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.Initialize))]
            [HarmonyPrefix]
            static void Initialize_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"Initialize_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.AddTabs))]
            [HarmonyPrefix]
            static void AddTabs_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddTabs_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.GetTabSprite))]
            [HarmonyPrefix]
            static void GetTabSprite_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"GetTabSprite_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.GetTabName))]
            [HarmonyPrefix]
            static void GetTabName_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"GetTabName_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.SelectTab))]
            [HarmonyPrefix]
            static void SelectTab_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"SelectTab_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.ClearPage))]
            [HarmonyPrefix]
            static void ClearPage_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"ClearPage_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.GetFirstTab))]
            [HarmonyPrefix]
            static void GetFirstTab_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"GetFirstTab_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.GetFirstElement))]
            [HarmonyPrefix]
            static void GetFirstElement_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"GetFirstElement_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.GetFirstSelectable))]
            [HarmonyPrefix]
            static void GetFirstSelectable_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"GetFirstSelectable_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.GetLastSelectable))]
            [HarmonyPrefix]
            static void GetLastSelectable_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"GetLastSelectable_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.ClearAll))]
            [HarmonyPrefix]
            static void ClearAll_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"ClearAll_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.BuildPage))]
            [HarmonyPrefix]
            static void BuildPage_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"BuildPage_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.GenerateNavigation))]
            [HarmonyPrefix]
            static void GenerateNavigation_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"GenerateNavigation_Prefix");
            }

            [HarmonyPatch(nameof(OptionsController.SetMaximumDownNavigation))]
            [HarmonyPrefix]
            static void SetMaximumDownNavigation_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"SetMaximumDownNavigation_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.BuildQuickAccessPage))]
            [HarmonyPrefix]
            static void BuildQuickAccessPage_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"BuildQuickAccessPage_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.BuildIngamePage))]
            [HarmonyPrefix]
            static void BuildIngamePage_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"BuildIngamePage_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.BuildDisplayPage))]
            [HarmonyPrefix]
            static void BuildDisplayPage_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"BuildDisplayPage_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.BuildSoundPage))]
            [HarmonyPrefix]
            static void BuildSoundPage_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"BuildSoundPage_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.BuildGameplayPage))]
            [HarmonyPrefix]
            static void BuildGameplayPage_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"BuildGameplayPage_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.BuildUserPage))]
            [HarmonyPrefix]
            static void BuildUserPage_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"BuildUserPage_Prefix");
            }


            //[HarmonyPatch(nameof(OptionsController.BuildAboutPage))]
            //[HarmonyPrefix]
            //static bool BuildAboutPage_Prefix(OptionsController __instance)
            //{
            //    Melon<ExampleMod>.Logger.Msg($"BuildAboutPage_Prefix");
            //    return false;
            //}


            [HarmonyPatch(nameof(OptionsController.BuildCheatsPage))]
            [HarmonyPrefix]
            static void BuildCheatsPage_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"BuildCheatsPage_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.AddLanguageButton))]
            [HarmonyPrefix]
            static void AddLanguageButton_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddLanguageButton_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.AddVisuallyInvertStages))]
            [HarmonyPrefix]
            static void AddVisuallyInvertStages_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddVisuallyInvertStages_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.AddDisableMovingBackground))]
            [HarmonyPrefix]
            static void AddDisableMovingBackground_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddDisableMovingBackground_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.AddFullScreen))]
            [HarmonyPrefix]
            static void AddFullScreen_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddFullScreen_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.AddJoystickTypes))]
            [HarmonyPrefix]
            static void AddJoystickTypes_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddJoystickTypes_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.AddOrientations))]
            [HarmonyPrefix]
            static void AddOrientations_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddOrientations_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.AddResolutions))]
            [HarmonyPrefix]
            static void AddResolutions_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddResolutions_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.AddWindowTypes))]
            [HarmonyPrefix]
            static void AddWindowTypes_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddWindowTypes_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.AddVisibleJoysticks))]
            [HarmonyPrefix]
            static void AddVisibleJoysticks_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddVisibleJoysticks_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.AddSoundEffectTypes))]
            [HarmonyPrefix]
            static void AddSoundEffectTypes_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddSoundEffectTypes_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.AddSlider))]
            [HarmonyPrefix]
            static void AddSlider_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddSlider_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.AddDropDown))]
            [HarmonyPrefix]
            static void AddDropDown_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddDropDown_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.AddTickBox))]
            [HarmonyPrefix]
            static void AddTickBox_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddTickBox_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.AddLabeledButton))]
            [HarmonyPrefix]
            static void AddLabeledButton_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddLabeledButton_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.AddMultipleChoice))]
            [HarmonyPrefix]
            static void AddMultipleChoice_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddMultipleChoice_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.Translate))]
            [HarmonyPrefix]
            static void Translate_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"Translate_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.ToggleVisualInvert))]
            [HarmonyPrefix]
            static void ToggleVisualInvert_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"ToggleVisualInvert_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.SetJoystickDefault))]
            [HarmonyPrefix]
            static void SetJoystickDefault_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"SetJoystickDefault_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.SetJoystickLegacy))]
            [HarmonyPrefix]
            static void SetJoystickLegacy_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"SetJoystickLegacy_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.SetOrientation))]
            [HarmonyPrefix]
            static void SetOrientation_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"SetOrientation_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.ApplySelectedOrientation))]
            [HarmonyPrefix]
            static void ApplySelectedOrientation_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"ApplySelectedOrientation_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.SetMusic))]
            [HarmonyPrefix]
            static void SetMusic_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"SetMusic_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.SetSounds))]
            [HarmonyPrefix]
            static void SetSounds_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"SetSounds_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.SetClassicMusic))]
            [HarmonyPrefix]
            static void SetClassicMusic_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"SetClassicMusic_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.SetBlastProcessedMusic))]
            [HarmonyPrefix]
            static void SetBlastProcessedMusic_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"SetBlastProcessedMusic_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.FlashingVFX))]
            [HarmonyPrefix]
            static void FlashingVFX_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"FlashingVFX_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.ScreenShake))]
            [HarmonyPrefix]
            static void ScreenShake_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"ScreenShake_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.VisibleJoystick))]
            [HarmonyPrefix]
            static void VisibleJoystick_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"VisibleJoystick_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.DamageNumbers))]
            [HarmonyPrefix]
            static void DamageNumbers_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"DamageNumbers_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.SetFullscreen))]
            [HarmonyPrefix]
            static void SetFullscreen_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"SetFullscreen_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.ToggleMovingBackground))]
            [HarmonyPrefix]
            static void ToggleMovingBackground_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"ToggleMovingBackground_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.ToggleStageProgression))]
            [HarmonyPrefix]
            static void ToggleStageProgression_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"ToggleStageProgression_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.ViewPonclePrivacyPolicy))]
            [HarmonyPrefix]
            static void ViewPonclePrivacyPolicy_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"ViewPonclePrivacyPolicy_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.ViewUnityPrivacyPolicy))]
            [HarmonyPrefix]
            static void ViewUnityPrivacyPolicy_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"ViewUnityPrivacyPolicy_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.AnalyticsOptOut))]
            [HarmonyPrefix]
            static void AnalyticsOptOut_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AnalyticsOptOut_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.OpenLanguagesPage))]
            [HarmonyPrefix]
            static void OpenLanguagesPage_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"OpenLanguagesPage_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.SetResolution))]
            [HarmonyPrefix]
            static void SetResolution_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"SetResolution_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.SetWindowMode))]
            [HarmonyPrefix]
            static void SetWindowMode_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"SetWindowMode_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.ApplyGraphicsSettings))]
            [HarmonyPrefix]
            static void ApplyGraphicsSettings_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"ApplyGraphicsSettings_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.RestoreBackup))]
            [HarmonyPrefix]
            static void RestoreBackup_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"RestoreBackup_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.DeleteSave))]
            [HarmonyPrefix]
            static void DeleteSave_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"DeleteSave_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.WaitAndReselect))]
            [HarmonyPrefix]
            static void WaitAndReselect_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"WaitAndReselect_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.HideAdsButtons))]
            [HarmonyPrefix]
            static void HideAdsButtons_Prefix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"HideAdsButtons_Prefix");
            }


            [HarmonyPatch(nameof(OptionsController.Construct))]
            [HarmonyPostfix]
            static void Construct_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"Construct_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.Initialize))]
            [HarmonyPostfix]
            static void Initialize_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"Initialize_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.AddTabs))]
            [HarmonyPostfix]
            static void AddTabs_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddTabs_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.GetTabSprite))]
            [HarmonyPostfix]
            static void GetTabSprite_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"GetTabSprite_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.GetTabName))]
            [HarmonyPostfix]
            static void GetTabName_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"GetTabName_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.SelectTab))]
            [HarmonyPostfix]
            static void SelectTab_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"SelectTab_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.ClearPage))]
            [HarmonyPostfix]
            static void ClearPage_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"ClearPage_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.GetFirstTab))]
            [HarmonyPostfix]
            static void GetFirstTab_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"GetFirstTab_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.GetFirstElement))]
            [HarmonyPostfix]
            static void GetFirstElement_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"GetFirstElement_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.GetFirstSelectable))]
            [HarmonyPostfix]
            static void GetFirstSelectable_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"GetFirstSelectable_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.GetLastSelectable))]
            [HarmonyPostfix]
            static void GetLastSelectable_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"GetLastSelectable_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.ClearAll))]
            [HarmonyPostfix]
            static void ClearAll_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"ClearAll_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.BuildPage))]
            [HarmonyPostfix]
            static void BuildPage_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"BuildPage_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.GenerateNavigation))]
            [HarmonyPostfix]
            static void GenerateNavigation_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"GenerateNavigation_Postfix");
            }


            //[HarmonyPatch(nameof(OptionsController.SetMaximumUpNavigation))]
            //[HarmonyPostfix]
            //static void SetMaximumUpNavigation_Postfix(OptionsController __instance)
            //{
            //    Melon<ExampleMod>.Logger.Msg($"SetMaximumUpNavigation_Postfix");
            //}


            [HarmonyPatch(nameof(OptionsController.SetMaximumDownNavigation))]
            [HarmonyPostfix]
            static void SetMaximumDownNavigation_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"SetMaximumDownNavigation_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.BuildQuickAccessPage))]
            [HarmonyPostfix]
            static void BuildQuickAccessPage_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"BuildQuickAccessPage_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.BuildIngamePage))]
            [HarmonyPostfix]
            static void BuildIngamePage_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"BuildIngamePage_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.BuildDisplayPage))]
            [HarmonyPostfix]
            static void BuildDisplayPage_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"BuildDisplayPage_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.BuildSoundPage))]
            [HarmonyPostfix]
            static void BuildSoundPage_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"BuildSoundPage_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.BuildGameplayPage))]
            [HarmonyPostfix]
            static void BuildGameplayPage_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"BuildGameplayPage_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.BuildUserPage))]
            [HarmonyPostfix]
            static void BuildUserPage_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"BuildUserPage_Postfix");
            }


            //[HarmonyPatch(nameof(OptionsController.BuildAboutPage))]
            //[HarmonyPostfix]
            //static void BuildAboutPage_Postfix(OptionsController __instance)
            //{
            //    Melon<ExampleMod>.Logger.Msg($"BuildAboutPage_Postfix");
            //}


            [HarmonyPatch(nameof(OptionsController.BuildCheatsPage))]
            [HarmonyPostfix]
            static void BuildCheatsPage_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"BuildCheatsPage_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.AddLanguageButton))]
            [HarmonyPostfix]
            static void AddLanguageButton_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddLanguageButton_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.AddVisuallyInvertStages))]
            [HarmonyPostfix]
            static void AddVisuallyInvertStages_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddVisuallyInvertStages_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.AddDisableMovingBackground))]
            [HarmonyPostfix]
            static void AddDisableMovingBackground_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddDisableMovingBackground_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.AddFullScreen))]
            [HarmonyPostfix]
            static void AddFullScreen_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddFullScreen_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.AddJoystickTypes))]
            [HarmonyPostfix]
            static void AddJoystickTypes_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddJoystickTypes_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.AddOrientations))]
            [HarmonyPostfix]
            static void AddOrientations_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddOrientations_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.AddResolutions))]
            [HarmonyPostfix]
            static void AddResolutions_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddResolutions_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.AddWindowTypes))]
            [HarmonyPostfix]
            static void AddWindowTypes_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddWindowTypes_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.AddVisibleJoysticks))]
            [HarmonyPostfix]
            static void AddVisibleJoysticks_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddVisibleJoysticks_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.AddSoundEffectTypes))]
            [HarmonyPostfix]
            static void AddSoundEffectTypes_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddSoundEffectTypes_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.AddSlider))]
            [HarmonyPostfix]
            static void AddSlider_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddSlider_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.AddDropDown))]
            [HarmonyPostfix]
            static void AddDropDown_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddDropDown_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.AddTickBox))]
            [HarmonyPostfix]
            static void AddTickBox_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddTickBox_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.AddLabeledButton))]
            [HarmonyPostfix]
            static void AddLabeledButton_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddLabeledButton_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.AddMultipleChoice))]
            [HarmonyPostfix]
            static void AddMultipleChoice_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AddMultipleChoice_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.Translate))]
            [HarmonyPostfix]
            static void Translate_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"Translate_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.ToggleVisualInvert))]
            [HarmonyPostfix]
            static void ToggleVisualInvert_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"ToggleVisualInvert_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.SetJoystickDefault))]
            [HarmonyPostfix]
            static void SetJoystickDefault_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"SetJoystickDefault_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.SetJoystickLegacy))]
            [HarmonyPostfix]
            static void SetJoystickLegacy_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"SetJoystickLegacy_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.SetOrientation))]
            [HarmonyPostfix]
            static void SetOrientation_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"SetOrientation_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.ApplySelectedOrientation))]
            [HarmonyPostfix]
            static void ApplySelectedOrientation_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"ApplySelectedOrientation_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.SetMusic))]
            [HarmonyPostfix]
            static void SetMusic_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"SetMusic_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.SetSounds))]
            [HarmonyPostfix]
            static void SetSounds_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"SetSounds_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.SetClassicMusic))]
            [HarmonyPostfix]
            static void SetClassicMusic_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"SetClassicMusic_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.SetBlastProcessedMusic))]
            [HarmonyPostfix]
            static void SetBlastProcessedMusic_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"SetBlastProcessedMusic_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.FlashingVFX))]
            [HarmonyPostfix]
            static void FlashingVFX_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"FlashingVFX_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.ScreenShake))]
            [HarmonyPostfix]
            static void ScreenShake_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"ScreenShake_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.VisibleJoystick))]
            [HarmonyPostfix]
            static void VisibleJoystick_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"VisibleJoystick_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.DamageNumbers))]
            [HarmonyPostfix]
            static void DamageNumbers_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"DamageNumbers_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.SetFullscreen))]
            [HarmonyPostfix]
            static void SetFullscreen_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"SetFullscreen_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.ToggleMovingBackground))]
            [HarmonyPostfix]
            static void ToggleMovingBackground_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"ToggleMovingBackground_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.ToggleStageProgression))]
            [HarmonyPostfix]
            static void ToggleStageProgression_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"ToggleStageProgression_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.ViewPonclePrivacyPolicy))]
            [HarmonyPostfix]
            static void ViewPonclePrivacyPolicy_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"ViewPonclePrivacyPolicy_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.ViewUnityPrivacyPolicy))]
            [HarmonyPostfix]
            static void ViewUnityPrivacyPolicy_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"ViewUnityPrivacyPolicy_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.AnalyticsOptOut))]
            [HarmonyPostfix]
            static void AnalyticsOptOut_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"AnalyticsOptOut_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.OpenLanguagesPage))]
            [HarmonyPostfix]
            static void OpenLanguagesPage_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"OpenLanguagesPage_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.SetResolution))]
            [HarmonyPostfix]
            static void SetResolution_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"SetResolution_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.SetWindowMode))]
            [HarmonyPostfix]
            static void SetWindowMode_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"SetWindowMode_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.ApplyGraphicsSettings))]
            [HarmonyPostfix]
            static void ApplyGraphicsSettings_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"ApplyGraphicsSettings_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.RestoreBackup))]
            [HarmonyPostfix]
            static void RestoreBackup_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"RestoreBackup_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.DeleteSave))]
            [HarmonyPostfix]
            static void DeleteSave_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"DeleteSave_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.WaitAndReselect))]
            [HarmonyPostfix]
            static void WaitAndReselect_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"WaitAndReselect_Postfix");
            }


            [HarmonyPatch(nameof(OptionsController.HideAdsButtons))]
            [HarmonyPostfix]
            static void HideAdsButtons_Postfix(OptionsController __instance)
            {
                Melon<ExampleMod>.Logger.Msg($"HideAdsButtons_Postfix");
            }

        }
    }
}
