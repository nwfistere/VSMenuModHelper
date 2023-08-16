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
using Il2CppVampireSurvivors.App.Tools;

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

        [HarmonyPatch(typeof(OptionsController))]
        class OptionsController_Patch2
        {

            static RectTransform _content;

            //[HarmonyPatch(nameof(OptionsController.Initialize))]
            //[HarmonyPrefix]
            //static void Initialize_Prefix2(OptionsController __instance)
            //{
            //    //Texture2D[] textures = Resources.LoadAll<Texture2D>("");
            //    //List<string> names = textures.Select(t => t.name).ToList();
            //    //Component[] prefabs = Resources.LoadAll<Component>("");
            //    //List<string> prefabsnames = prefabs.Select(t => t.name).ToList();
            //    //Sprite[] uiSprites = Resources.LoadAll<Sprite>("spritesheets/UI");

            //    GameObject TabScroll = new GameObject("TabScroll", Il2CppType.Of<RectTransform>());
            //    TabScroll.transform.SetParent(__instance._TabContainer, false);

            //    List<string> gameObjects = Resources.LoadAll<GameObject>("").Select((g) => g.name).ToList();

            //    //prefabs = Resources.LoadAll<Component>("PrefabInstance");
            //    //prefabsnames = prefabs.Select(t => t.name).ToList();

            //    GameObject viewport = new GameObject("Viewport", Il2CppType.Of<RectTransform>());
            //    viewport.transform.SetParent(TabScroll.transform, false);


            //    GameObject content = new GameObject("Content", Il2CppType.Of<RectTransform>());
            //    content.transform.SetParent(viewport.transform, false);

            //    //UnityEngine.Object.Instantiate()
            //    GameObject Scrollbar = new GameObject("Scrollbar Vertical", Il2CppType.Of<Scrollbar>());
            //    Scrollbar.transform.SetParent(TabScroll.transform);

            //    GameObject Slider = new GameObject("Slider", Il2CppType.Of<Scrollbar>());

            //    ScrollEnhancer enhancer = TabScroll.AddComponent<ScrollEnhancer>();
            //    //Scrollbar scrollbar = enhancer.gameObject.AddComponent<Scrollbar>();
            //    UnityEngine.UI.Slider slider = enhancer.gameObject.AddComponent<UnityEngine.UI.Slider>();
            //    enhancer.Initialize(1, viewport.GetComponent<RectTransform>(), Scrollbar.GetComponent<Scrollbar>(), slider, 0.1f);

            //    _content = content;
            //}

            static RectTransform CreateScrollViewContent(ScrollRect scrollRect)
            {
                // Create a Content GameObject and set its properties.
                GameObject contentObject = new GameObject("Content");
                contentObject.AddComponent<RectTransform>(); // Required for layout control.
                contentObject.AddComponent<VerticalLayoutGroup>();
                contentObject.AddComponent<ContentSizeFitter>();

                VerticalLayoutGroup layoutGroup = contentObject.GetComponent<VerticalLayoutGroup>();
                layoutGroup.childControlHeight = false;
                layoutGroup.childControlWidth = false;
                layoutGroup.spacing = 55;

                // Set the Content's RectTransform properties.
                RectTransform contentRectTransform = contentObject.GetComponent<RectTransform>();
                contentRectTransform.SetParent(scrollRect.transform, false);
                //contentRectTransform.SetParent(contentObject.transform, false);
                contentRectTransform.anchorMin = Vector2.up;
                contentRectTransform.anchorMax = Vector2.one;
                contentRectTransform.pivot = Vector2.up;

                // Add your UI elements (Text, Images, etc.) to the contentGameObject.

                layoutGroup.SetDirty();

                return contentRectTransform;
            }

            static RectTransform CreateScrollViewViewport(GameObject scrollViewObject)
            {
                // Create a Viewport GameObject and set its properties.
                GameObject viewportObject = new GameObject("Viewport");
                viewportObject.AddComponent<RectTransform>(); // Required for layout control.

                // Set the Viewport's RectTransform properties.
                RectTransform viewportRectTransform = viewportObject.GetComponent<RectTransform>();
                viewportRectTransform.SetParent(scrollViewObject.transform, false);

                // Create a Mask component and add it to the Viewport GameObject.
                Mask mask = viewportObject.AddComponent<Mask>();
                mask.showMaskGraphic = true; // Set to true if you want to see the masked area.
                mask.m_RectTransform = viewportRectTransform;

                return viewportRectTransform;
            }

            static Scrollbar CreateScrollbar(ScrollRect scrollRect, Scrollbar.Direction direction)
            {
                // Create a Scrollbar GameObject and set its properties.
                GameObject scrollbarObject = new GameObject("Scrollbar");
                scrollbarObject.AddComponent<RectTransform>(); // Required for layout control.

                // Set the Scrollbar's RectTransform properties.
                RectTransform scrollbarRectTransform = scrollbarObject.GetComponent<RectTransform>();
                scrollbarRectTransform.SetParent(scrollRect.transform, false);

                // Add the Scrollbar component.
                Scrollbar scrollbar = scrollbarObject.AddComponent<Scrollbar>();
                scrollbar.direction = direction;

                // Create a Scrollbar handle and set its properties.
                GameObject handleObject = new GameObject("Handle");
                handleObject.AddComponent<RectTransform>(); // Required for layout control.

                // Set the Handle's RectTransform properties.
                RectTransform handleRectTransform = handleObject.GetComponent<RectTransform>();
                handleRectTransform.SetParent(scrollbarRectTransform, false);

                scrollbar.handleRect = handleRectTransform;

                return scrollbar;
            }

            [HarmonyPatch(nameof(OptionsController.Initialize))]
            [HarmonyPrefix]
            static void Initialize_Prefix2(OptionsController __instance)
            {
                GameObject tabObject = __instance._TabContainer.gameObject;
                if (__instance._TabContainer.FindChild("ScrollView") != null)
                {
                    List<Transform> children = new();
                    for (int i = 0; i < _content.childCount; i++)
                    {
                        children.Add(_content.GetChild(i).transform);
                    }
                    children.ForEach((child) => child.SetParent(__instance._TabContainer, false));
                    GameObject.DestroyImmediate(__instance._TabContainer.FindChild("ScrollView").gameObject);
                }

                if (__instance._TabContainer.FindChild("ScrollView") == null)
                {
                    GameObject scrollViewObject = new GameObject("ScrollView");
                    scrollViewObject.transform.SetParent(tabObject.transform, false);

                    // Add a ScrollRect component to the ScrollView GameObject.
                    ScrollRect scrollRect = scrollViewObject.AddComponent<ScrollRect>();
                    scrollRect.content = CreateScrollViewContent(scrollRect);

                    _content = scrollRect.content;

                    // Create and set the viewport and scrollbar properties.
                    scrollRect.viewport = CreateScrollViewViewport(scrollViewObject);
                    scrollRect.verticalScrollbar = CreateScrollbar(scrollRect, Scrollbar.Direction.BottomToTop);
                    scrollRect.horizontal = false;
                    //scrollRect.horizontalNormalizedPosition = 0.3f;
                    scrollRect.scrollSensitivity = 2;
                }
            }

            [HarmonyPatch(nameof(OptionsController.BuildPage))]
            [HarmonyPostfix]
            static void BuildPage_Postfix(OptionsController __instance, OptionsTabType type)
            {
                Melon<VSMenuHelper>.Logger.Msg($"BuildPage_Postfix");
                List<Transform> children = new();
                for (int i = 0; i < __instance._TabContainer.childCount; i++)
                {
                    if (__instance._TabContainer.GetChild(i).name != "ScrollView")
                    {
                        children.Add(__instance._TabContainer.GetChild(i).transform);
                    }
                }
                children.ForEach((child) => child.SetParent(_content, false));
                __instance._TabContainer.GetComponentInChildren<VerticalLayoutGroup>().SetDirty();
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
