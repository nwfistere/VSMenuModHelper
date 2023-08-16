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
using UnityEngine.UIElements;
using Il2CppInterop.Runtime;
using UnityEngine.UI;
using Il2CppVampireSurvivors;
using System.Runtime.ExceptionServices;

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
            mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
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

        static int mainThreadId;
        public static bool IsMainThread
        {
            get { return System.Threading.Thread.CurrentThread.ManagedThreadId == mainThreadId; }
        }

        [HarmonyPatch(typeof(PrefabReassembler))]
        static class PrefabReassembler_Patch
        {

            //public static List<PrefabReassembler> instances = new();

            [HarmonyPatch(nameof(PrefabReassembler.Start))]
            [HarmonyPrefix]
            static void Start_Prefix(PrefabReassembler __instance) => Melon<VSMenuHelper>.Logger.Msg($"Start_Prefix");

            [HarmonyPatch(nameof(PrefabReassembler.Start))]
            [HarmonyPostfix]
            static void Start_Postfix(PrefabReassembler __instance) => Melon<VSMenuHelper>.Logger.Msg($"Start_Postfix");

            //[HarmonyPatch(nameof(PrefabReassembler.Update))]
            //[HarmonyPostfix]
            //static void Update_Postfix(PrefabReassembler __instance)
            //{
            //    if (IsMainThread)
            //    {
            //        //if (__instance._PrefabComponents != null && __instance._PrefabComponents.Count > 0)
            //        //    if (__instance._PrefabComponents[^1] != null && __instance._PrefabComponents[^1].name != null)
            //        //        Melon<VSMenuHelper>.Logger.Msg($"prefab: {__instance._PrefabComponents[^1].name}");
            //    }
            //}



            //[HarmonyPatch(nameof(PrefabReassembler.Update))]
            //[HarmonyPrefix]
            //static void Update_Prefix(PrefabReassembler __instance) => instances.Add(__instance);

            [HarmonyPatch(nameof(PrefabReassembler.SpawnRoutine))]
            [HarmonyPrefix]
            static void SpawnRoutine_Prefix(PrefabReassembler __instance) => Melon<VSMenuHelper>.Logger.Msg($"SpawnRoutine_Prefix");

            [HarmonyPatch(nameof(PrefabReassembler.SpawnRoutine))]
            [HarmonyPostfix]
            static void SpawnRoutine_Postfix(PrefabReassembler __instance) => Melon<VSMenuHelper>.Logger.Msg($"SpawnRoutine_Postfix");
        }

        [HarmonyPatch(typeof(OptionsController))]
        class OptionsController_Patch2
        {

            static GameObject _content;

            [HarmonyPatch(nameof(OptionsController.Initialize))]
            [HarmonyPrefix]
            static void Initialize_Prefix2(OptionsController __instance)
            {
                Texture2D[] textures = Resources.LoadAll<Texture2D>("");
                List<string> names = textures.Select(t => t.name).ToList();
                Component[] prefabs = Resources.LoadAll<Component>("");
                List<string> prefabsnames = prefabs.Select(t => t.name).ToList();
                Sprite[] uiSprites = Resources.LoadAll<Sprite>("spritesheets/UI");

                GameObject TabScroll = new GameObject("TabScroll", Il2CppType.Of<RectTransform>());
                TabScroll.transform.SetParent(__instance._TabContainer, false);

                List<string> gameObjects = Resources.LoadAll<GameObject>("").Select((g) => g.name).ToList();

                prefabs = Resources.LoadAll<Component>("PrefabInstance");
                prefabsnames = prefabs.Select(t => t.name).ToList();

                GameObject viewport = new GameObject("Viewport", Il2CppType.Of<RectTransform>());
                viewport.transform.SetParent(TabScroll.transform, false);


                GameObject content = new GameObject("Content", Il2CppType.Of<RectTransform>());
                content.transform.SetParent(viewport.transform, false);

                //UnityEngine.Object.Instantiate()
                GameObject Scrollbar = new GameObject("Scrollbar Vertical", Il2CppType.Of<Scrollbar>());
                Scrollbar.transform.SetParent(TabScroll.transform);

                GameObject Slider = new GameObject("Slider", Il2CppType.Of<Scrollbar>());

                ScrollEnhancer enhancer = TabScroll.AddComponent<ScrollEnhancer>();
                //Scrollbar scrollbar = enhancer.gameObject.AddComponent<Scrollbar>();
                UnityEngine.UI.Slider slider = enhancer.gameObject.AddComponent<UnityEngine.UI.Slider>();
                enhancer.Initialize(1, viewport.GetComponent<RectTransform>(), Scrollbar.GetComponent<Scrollbar>(), slider, 0.1f);

                _content = content;
            }

            [HarmonyPatch(nameof(OptionsController.BuildPage))]
            [HarmonyPostfix]
            static void BuildPage_Postfix(OptionsController __instance, OptionsTabType type)
            {
                Melon<VSMenuHelper>.Logger.Msg($"BuildPage_Postfix");
                List<Transform> children = new();
                for (int i = 0; i < __instance._TabContainer.childCount; i++)
                {
                    if (__instance._TabContainer.GetChild(i).name != "TabScroll")
                    {
                        children.Add(__instance._TabContainer.GetChild(i).transform);
                    }
                }
                children.ForEach((child) => child.parent = _content.transform);
            }

            [HarmonyPatch(nameof(OptionsController.GenerateNavigation))]
            [HarmonyPostfix]
            static void GenerateNavigation_Postfix(OptionsController __instance)
            {
                Melon<VSMenuHelper>.Logger.Msg($"GenerateNavigation_Postfix");
            }
        }
    }
}
