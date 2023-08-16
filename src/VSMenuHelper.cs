using HarmonyLib;
using Il2CppVampireSurvivors.UI;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Il2CppTMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static Il2CppVampireSurvivors.UI.OptionsController;

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

#if DEBUG
        [HarmonyPatch("Il2CppInterop.HarmonySupport.Il2CppDetourMethodPatcher", "ReportException")]
        public static class Patch_Il2CppDetourMethodPatcher
        {
            public static bool Prefix(Exception ex)
            {
                MelonLogger.Error("During invoking native->managed trampoline", ex);
                return false;
            }
        }

        public override void OnLateUpdate()
        {
            base.OnLateUpdate();
        }
#endif

        [HarmonyPatch(typeof(OptionsController))]
        class OptionsController_Patch2
        {

            public static RectTransform _content;
            public static GameObject tabObject;

            static RectTransform CreateScrollViewContent(RectTransform viewportRectTransform)
            {
                GameObject contentObject = new GameObject("Content");
                contentObject.AddComponent<RectTransform>(); // Required for layout control.
                contentObject.AddComponent<VerticalLayoutGroup>();
                contentObject.AddComponent<ContentSizeFitter>();

                VerticalLayoutGroup layoutGroup = contentObject.GetComponent<VerticalLayoutGroup>();
                layoutGroup.childControlHeight = false;
                layoutGroup.childControlWidth = false;
                layoutGroup.spacing = 55;

                RectTransform contentRectTransform = contentObject.GetComponent<RectTransform>();
                contentRectTransform.SetParent(viewportRectTransform, false);
                contentRectTransform.anchorMin = Vector2.zero;
                contentRectTransform.anchorMax = Vector2.one;
                contentRectTransform.pivot = Vector2.up;
                contentRectTransform.anchoredPosition = new Vector2(30, 0);

                ContentSizeFitter contentSizeFitter = contentObject.GetComponent<ContentSizeFitter>();
                contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                return contentRectTransform;
            }

            static RectTransform CreateScrollViewViewport(GameObject scrollViewObject)
            {
                Material material = new Material(Shader.Find("UI/Default"));
                Material unmaterial = new Material(Shader.Find("UI/Default"));
                GameObject viewportObject = new GameObject("Viewport");
                viewportObject.AddComponent<RectTransform>(); // Required for layout control.

                RectTransform viewportRectTransform = viewportObject.GetComponent<RectTransform>();
                viewportRectTransform.SetParent(scrollViewObject.GetComponent<RectTransform>(), false);

                viewportRectTransform.anchorMin = new Vector2(0, 0.02f);
                viewportRectTransform.anchorMax = Vector2.one;
                viewportRectTransform.sizeDelta = Vector2.zero;
                viewportRectTransform.anchoredPosition = new Vector2(0, -10);

                Sprite[] allSprites = Resources.FindObjectsOfTypeAll<Sprite>();

                Sprite sprite = allSprites.Where(s => s.name == "UIMask").FirstOrDefault(null as Sprite);

                Mask mask = viewportObject.AddComponent<Mask>();
                mask.showMaskGraphic = false;

                Image image = viewportObject.AddComponent<Image>();
                image.m_Type = Image.Type.Sliced;
                image.material = material;
                //image.isMaskingGraphic = true;
                image.sprite = sprite;

                mask.m_Graphic = image;
                mask.m_MaskMaterial = material;
                mask.m_UnmaskMaterial = unmaterial;


                //image.sprite = sprite;
                //mask.m_MaskMaterial = material;
                //mask.m_UnmaskMaterial = material;

                return viewportRectTransform;
            }

            static Scrollbar CreateScrollbar(ScrollRect scrollRect, Scrollbar.Direction direction)
            {
                GameObject scrollbarObject = new GameObject("Scrollbar");
                scrollbarObject.AddComponent<RectTransform>(); // Required for layout control.

                RectTransform scrollbarRectTransform = scrollbarObject.GetComponent<RectTransform>();
                scrollbarRectTransform.SetParent(scrollRect.transform, false);

                Scrollbar scrollbar = scrollbarObject.AddComponent<Scrollbar>();
                scrollbar.direction = direction;

                GameObject handleObject = new GameObject("Handle");
                handleObject.AddComponent<RectTransform>(); // Required for layout control.

                RectTransform handleRectTransform = handleObject.GetComponent<RectTransform>();
                handleRectTransform.SetParent(scrollbarRectTransform, false);

                scrollbar.handleRect = handleRectTransform;

                return scrollbar;
            }

            [HarmonyPatch(nameof(OptionsController.Initialize))]
            [HarmonyPrefix]
            static void Initialize_Prefix2(OptionsController __instance)
            {
                //tabObject = __instance._TabContainer.gameObject;
                //if (__instance._TabContainer.FindChild("ScrollView") != null)
                //{
                //    List<Transform> children = new();
                //    for (int i = 0; i < _content.childCount; i++)
                //    {
                //        children.Add(_content.GetChild(i).transform);
                //    }
                //    children.ForEach((child) => child.SetParent(__instance._TabContainer, false));
                //    GameObject.DestroyImmediate(__instance._TabContainer.FindChild("ScrollView").gameObject);
                //}

                //if (__instance._TabContainer.FindChild("ScrollView") == null)
                //{
                //    GameObject scrollViewObject = new GameObject("ScrollView");
                //    scrollViewObject.transform.SetParent(tabObject.transform, false);

                //    ScrollRect scrollRect = scrollViewObject.AddComponent<ScrollRect>();
                //    //scrollRect.verticalScrollbar = CreateScrollbar(scrollRect, Scrollbar.Direction.BottomToTop);
                //    scrollRect.horizontal = false;
                //    scrollRect.scrollSensitivity = 100;
                //    scrollRect.vertical = true;
                //    scrollRect.viewport = CreateScrollViewViewport(scrollViewObject);
                //    scrollRect.content = CreateScrollViewContent(scrollRect.viewport);

                //    RectTransform rectTransform = scrollViewObject.GetComponent<RectTransform>();
                //    rectTransform.anchorMin = Vector2.zero;
                //    rectTransform.anchorMax = Vector2.one;
                //    rectTransform.sizeDelta = Vector2.zero;
                //    rectTransform.anchoredPosition = Vector2.zero;

                //    _content = scrollRect.content;
                //}
            }

            [HarmonyPatch(nameof(OptionsController.BuildPage))]
            [HarmonyPostfix]
            static void BuildPage_Postfix(OptionsController __instance, OptionsTabType type)
            {
                List<Transform> children = new();

                tabObject = __instance._TabContainer.gameObject;
                if (__instance._TabContainer.FindChild("ScrollView") != null)
                {
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

                    ScrollRect scrollRect = scrollViewObject.AddComponent<ScrollRect>();
                    //scrollRect.verticalScrollbar = CreateScrollbar(scrollRect, Scrollbar.Direction.BottomToTop);
                    scrollRect.horizontal = false;
                    scrollRect.scrollSensitivity = 100;
                    scrollRect.vertical = true;
                    scrollRect.viewport = CreateScrollViewViewport(scrollViewObject);
                    scrollRect.content = CreateScrollViewContent(scrollRect.viewport);

                    RectTransform rectTransform = scrollViewObject.GetComponent<RectTransform>();
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.one;
                    rectTransform.sizeDelta = Vector2.zero;
                    rectTransform.anchoredPosition = Vector2.zero;

                    _content = scrollRect.content;
                }

                children = new();
                for (int i = 0; i < __instance._TabContainer.childCount; i++)
                {
                    if (__instance._TabContainer.GetChild(i).name != "ScrollView")
                    {
                        children.Add(__instance._TabContainer.GetChild(i).transform);
                    }
                }
                children.ForEach((child) => child.SetParent(_content, false));

                if (__instance._TabContainer.GetComponent<VerticalLayoutGroup>() != null)
                {
                    GameObject.DestroyImmediate(__instance._TabContainer.GetComponent<VerticalLayoutGroup>());
                }
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
