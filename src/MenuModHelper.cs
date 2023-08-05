﻿using Il2CppVampireSurvivors.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Il2CppVampireSurvivors.UI.OptionsController;

namespace VSMenuHelper
{

    public class MenuHelper
    {
        private List<Tab> createdTabs;
        public MenuHelper()
        {
            createdTabs = new();
        }

        public void DeclareTab(string identifier, string spritePath)
        {
            if (createdTabs.Where((tab) => tab.TabName == identifier).Any())
                throw new ArgumentException("Object with that identifier already exists.");
            createdTabs.Add(new(identifier, spritePath));
        }

        public void AddElementToTab(string identifier, UIElement element)
        {
            createdTabs.First((tab) => tab.TabName == identifier).AddElement(element);
        }

        public void Initialize_Prefix(OptionsController controller)
        {
            foreach (Tab tab in createdTabs)
            {
                tab.Init(controller);
            }
        }

        // Returns true if BuildPage should run (We didn't do anything). False if it shouldn't.
        public bool OnBuildPage(OptionsController controller, OptionsTabType type)
        {
            if (!createdTabs.Where((tab) => tab.GetTabType() == type).Any())
                return true;
            Tab tab = createdTabs.First((tab) => tab.GetTabType() == type);
            tab.GetElements().ForEach((element) => element.GetElement().Invoke(controller));
            controller._Title.text = tab.TabName;
            return false;
        }

        public Sprite? OnGetTabSprite(OptionsTabType type) => createdTabs.Where((tab) => tab.GetTabType() == type).Select((tab) => tab.GetSprite()).FirstOrDefault(null as Sprite);

        public string? OnGetTabName(OptionsTabType type) => createdTabs.Where((tab) => tab.GetTabType() == type).Select((pair) => pair.TabName).FirstOrDefault(null as string);

        public bool OurTab(OptionsTabType type) => createdTabs.Where((tab) => tab.GetTabType() == type).Any();
    }

    public class Tab {

        public string TabName { get; set; }
        private static OptionsTabType MinTypeValue = Enum.GetValues<OptionsController.OptionsTabType>().Max() + 1;
        private OptionsTabType TabType;
        private string TabButtonSpritePath;
        private bool alreadyInit = false;
        private List<UIElement> elements;

        public Tab(string name, string spritePath)
        {
            TabName = name;
            TabButtonSpritePath = spritePath;
            elements = new();
            TabType = MinTypeValue++;
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
            return SpriteImporter.LoadSprite(TabButtonSpritePath);
        }
    }
}