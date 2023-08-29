using Il2CppTMPro;
using Il2CppVampireSurvivors.Graphics;
using Il2CppVampireSurvivors.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static Il2CppVampireSurvivors.UI.OptionsController;

namespace VSMenuModHelper
{
    internal class OptionsMenuController
    {
        private readonly List<Tab> createdTabs;
        public OptionsMenuController()
        {
            createdTabs = new();
        }

        public void DeclareTab(string identifier, string spritePath)
        {
            if (createdTabs.Where((tab) => tab.TabName == identifier).Any())
                throw new ArgumentException("Object with that identifier already exists.");
            createdTabs.Add(new(identifier, spritePath));
        }

        public void DeclareTab(string identifier, Uri spriteUri)
        {
            if (createdTabs.Where((tab) => tab.TabName == identifier).Any())
                throw new ArgumentException("Object with that identifier already exists.");
            createdTabs.Add(new(identifier, spriteUri));
        }

        public void DeclareTab(string identifier, Sprite sprite)
        {
            if (createdTabs.Where((tab) => tab.TabName == identifier).Any())
                throw new ArgumentException("Object with that identifier already exists.");
            createdTabs.Add(new(identifier, sprite));
        }

        public void AddElementToTab(string identifier, UIElement element)
        {
            createdTabs.First((tab) => tab.TabName == identifier).AddElement(element);
        }

        public void Construct_Prefix() => createdTabs.ForEach((tab) => tab.Reset());

        public void Initialize_Prefix(OptionsController controller) => createdTabs.ForEach((tab) => tab.Init(controller));

        // Returns true if BuildPage should run (We didn't do anything). False if it shouldn't.
        public bool OnBuildPage(OptionsController controller, OptionsTabType type)
        {
            if (!createdTabs.Where((tab) => tab.GetTabType() == type).Any())
                return true;
            Tab tab = createdTabs.First((tab) => tab.GetTabType() == type);
            tab.GetElements().ForEach((element) => element.GetElement().Invoke(controller));

            List<Il2CppTMPro.TextMeshProUGUI> objects = UnityEngine.Object.FindObjectsOfTypeAll(Il2CppInterop.Runtime.Il2CppType.Of<Il2CppTMPro.TextMeshProUGUI>()).ToList().Where((o) => o.name == "Label").Select((o) => o.Cast<Il2CppTMPro.TextMeshProUGUI>()).Where((o) => o.transform.parent.name == "Options_TickBox(Clone)").ToList();
            TextMeshProUGUI text = objects[0];
            //text.mater


            return false;
        }

        public Sprite? OnGetTabSprite(OptionsTabType type)
        {
            Sprite? sprite = createdTabs.Where((tab) => tab.GetTabType() == type).Select((tab) => tab.GetSprite()).FirstOrDefault(null as Sprite);
            return sprite;
        }

        internal string? getTabNameFromType(OptionsTabType type)
        {
            return createdTabs.Where((tab) => tab.GetTabType() == type).Select((tab) => tab.TabName).FirstOrDefault(null as string);
        }

        public string? OnGetTabName(OptionsTabType type) => createdTabs.Where((tab) => tab.GetTabType() == type).Select((pair) => pair.TabName).FirstOrDefault(null as string);

        public bool OurTab(OptionsTabType type) => createdTabs.Where((tab) => tab.GetTabType() == type).Any();

        public void AddSpriteModifier(string identifier, Func<Sprite, Sprite> spriteModifier)
        {
            createdTabs.First((tab) => tab.TabName == identifier).spriteModifiers.Add(spriteModifier);
        }
    }

    internal class Tab {

        public string TabName { get; set; }
        private static OptionsTabType MinTypeValue = Enum.GetValues<OptionsController.OptionsTabType>().Max() + 1;
        private readonly OptionsTabType TabType;
        private readonly string? TabButtonSpritePath; // Using a filepath
        private readonly string? TabButtonSpriteName; // Using a sprite from resource manager
        private readonly Uri? TabButtonSpriteUri; // Using a sprite from internet.
        private bool alreadyInit = false;
        private readonly List<UIElement> elements;
        public List<Func<Sprite, Sprite>> spriteModifiers { get; set; } = new();

        private Tab(string name)
        {
            TabName = name;
            elements = new();
            TabType = MinTypeValue++;
        }

        public Tab(string name, string spritePath) : this(name)
        {
            // If file doesn't exist, try loading directly from SpriteManager later.
            if (File.Exists(spritePath))
            {
                TabButtonSpritePath = spritePath;
            } else
            {
                TabButtonSpriteName = spritePath;
            }
        }

        public Tab(string name, Uri spriteUri) : this(name)
        {
            TabButtonSpriteUri = spriteUri;
        }

        public Tab(string name, Sprite sprite) : this(name)
        {
            TabButtonSpriteName = sprite.name;

            SpriteManager.RegisterSprite(sprite);
        }

        public OptionsTabType GetTabType() => TabType;
        public void AddElement(UIElement element) => elements.Add(element);
        public List<UIElement> GetElements() => elements;

        public void Init(OptionsController optionsController)
        {
            if (!alreadyInit)
                optionsController._OptionsConfig.Add(TabType);
            alreadyInit = true;
        }

        public Sprite GetSprite()
        {
            if (TabButtonSpritePath != null)
            {
                Sprite sprite =  SpriteImporter.LoadSprite(TabButtonSpritePath);
                sprite.name = TabName;
                spriteModifiers.ForEach(mod => sprite = mod.Invoke(sprite));
                return sprite;
            }
            else if (TabButtonSpriteUri != null)
            {
                Sprite sprite = SpriteImporter.LoadSprite(TabButtonSpriteUri);
                sprite.name = TabName;
                spriteModifiers.ForEach(mod => sprite = mod.Invoke(sprite));
                return sprite;
            } else if (TabButtonSpriteName != null)
            {
                Sprite sprite = SpriteManager.GetSprite(TabButtonSpriteName, false);
                spriteModifiers.ForEach(mod => sprite = mod.Invoke(sprite));
                return sprite;
            }
            throw new InvalidOperationException("Sprite path and uri are both null, one must be set.");
        }

        public void Reset() => alreadyInit = false;
    }
}