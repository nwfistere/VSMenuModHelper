using Il2CppVampireSurvivors.UI;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Il2CppVampireSurvivors.UI.OptionsController;
using UnityEngine;
using HarmonyLib;
using static MelonLoader.MelonLogger;

namespace VSMenuModHelper
{
    public static class ModInfo
    {
        public const string Name = "VSMenuModHelper";
        public const string Description = "A mod to enable altering Vampire Survivors menus.";
        public const string Author = "Nick";
        public const string Company = "Nick";
        public const string Version = "1.2.0";
        public const string Download = "https://github.com/nwfistere/VSMenuModHelper";
    }
    public class VSMenuHelper : MelonMod
    {
#pragma warning disable CS8618
        public static VSMenuHelper Instance { get; private set; }
#pragma warning restore CS8618
        private readonly OptionsMenuController optionsMenuController;
        private readonly Dictionary<string, List<Func<Sprite, Sprite>>> spriteModifiers;

        private VSMenuHelper() : base()
        {
            optionsMenuController = new();
            spriteModifiers = new();
        }

        public override void OnEarlyInitializeMelon()
        {
            Instance = this;
        }

        public void DeclareOptionsTab(string identifier, string spritePath) => optionsMenuController.DeclareTab(identifier, spritePath);
        public void DeclareOptionsTab(string identifier, Uri spriteUri) => optionsMenuController.DeclareTab(identifier, spriteUri);
        public void AddElementToTab(string identifier, UIElement element) => optionsMenuController.AddElementToTab(identifier, element);

        public void AddTabSpriteModifier(string identifier, Func<Sprite, Sprite> spriteModifier)
        {
            List<Func<Sprite, Sprite>> ModifierList = spriteModifiers.GetValueOrDefault(identifier, new());
            ModifierList.Add(spriteModifier);
        }

        private Func<Sprite, Sprite>? ModifySprite(OptionsTabType type)
        {
            string? identifer = optionsMenuController.getTabNameFromType(type);
            if (identifer != null)
            {
                if (spriteModifiers.ContainsKey(identifer))
                {
                    return (sprite) =>
                    {
                        foreach (Func<Sprite, Sprite> func in spriteModifiers[identifer])
                        {
                            sprite = func(sprite);
                        }
                        return sprite;
                    };
                }
            }
            return null;
        }

        [HarmonyPatch(typeof(OptionsController))]
        class OptionsController_Patch
        {
            [HarmonyPatch(nameof(OptionsController.Construct))]
            [HarmonyPrefix]
            static void Construct_Prefix() => Instance.optionsMenuController.Construct_Prefix();

            [HarmonyPatch(nameof(OptionsController.Initialize))]
            [HarmonyPrefix]
            static void Initialize_Prefix(OptionsController __instance) => Instance.optionsMenuController.Initialize_Prefix(__instance);

            [HarmonyPatch(nameof(OptionsController.GetTabSprite))]
            [HarmonyPostfix]
            static void GetTabSprite_Postfix(OptionsTabType t, ref Sprite __result) => __result = Instance.optionsMenuController.OnGetTabSprite(t, Instance.ModifySprite(t)) ?? __result;

            [HarmonyPatch(nameof(OptionsController.BuildPage))]
            [HarmonyPrefix]
            static bool BuildPage_Prefix(OptionsController __instance, OptionsTabType type) => Instance.optionsMenuController.OnBuildPage(__instance, type);
        }
    }
}
