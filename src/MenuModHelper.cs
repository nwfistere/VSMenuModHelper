using Il2CppVampireSurvivors.App.Tools;
using Il2CppVampireSurvivors.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Il2CppVampireSurvivors.UI.OptionsController;

namespace VSMenuHelper
{
    public class VSMenuHelper
    {

    }

    public class Tab {

        //private readonly OptionsController optionsController;
        private static Dictionary<OptionsTabType, Tab> createdTabs = new();
        private GameObject tabObject;
        public string TabName { get; set; }
        private OptionsTabType tabType = (OptionsTabType)(-1);
        private Sprite tabButtonSprite;
        private bool alreadyInit = false;
        private static OptionsController StaticOptionsController;
        private List<Action> buildPageActions = new();


        public Tab(string name, Sprite sprite)
        {
            TabName = name;
            tabButtonSprite = sprite;
        }

         
        private static Action<OptionsTabType> selectTab = (OptionsTabType type) => {
            if (StaticOptionsController != null)
            { 
                StaticOptionsController.SelectTab(type);
                StaticOptionsController._Title.text = createdTabs[type].TabName;
            }
        };

        // Should be called at OptionsController.AddTabs Postfix.
        public void OnTabCreation(OptionsController optionsController)
        {
            if (alreadyInit)
            {
                throw new Exception("Cannot init twice!");
            }

            if (createdTabs == null || createdTabs.Select((pair) => pair.Value.TabName == TabName).Any())
            {
                return;
            }

            StaticOptionsController = optionsController;
            tabType = (OptionsTabType)optionsController._spawnedTabs.Count;

            optionsController._OptionsConfig.Add(tabType);

            CreateTabObject();

            alreadyInit = true;
        }

        private void CreateTabObject()
        {
            tabObject = UnityEngine.Object.Instantiate(StaticOptionsController._TabPrefab);
            tabObject.name = TabName;

            tabObject.transform.parent = StaticOptionsController._TabContainer;

            Button tabButton = tabObject.GetComponent<Button>();
            tabButton.onClick.AddListener(new Action(delegate { selectTab(tabType); }));
            tabButton.SetScale(1);

            Image tabImage = tabButton.GetComponent<Image>();
            tabImage.sprite = tabButtonSprite;
            tabImage.overrideSprite = tabButtonSprite;
            tabImage.name = TabName;

            createdTabs[tabType] = this;

            StaticOptionsController._spawnedTabs.Add(tabObject);
        }

        public static Sprite? OnGetTabSprite(OptionsTabType type)
        {
            return createdTabs.Where((pair) => pair.Key == type)
                .Select((pair) => pair.Value.tabObject.GetComponent<Image>().sprite).FirstOrDefault(null as Sprite);
        }

        public static string? OnGetTabName(OptionsTabType type)
        {
            return createdTabs.Where((pair) => pair.Key == type)
                .Select((pair) => pair.Value.TabName).FirstOrDefault((string)null);
        }

        public static bool OurTab(OptionsTabType type)
        {
            return createdTabs.Where((pair) => pair.Key == type).Any();
        }

        public void OnBuildPage(Action action)
        {
            buildPageActions.Add(action);
        }

        public static bool OnBuildPage(OptionsTabType type)
        {
            if (!createdTabs.ContainsKey(type))
                return false;
            Tab tab = createdTabs[type];
            foreach (Action action in tab.buildPageActions)
            {
                action.Invoke();
            }
            return true;
        }

        public static void OnGenerateNavigation()
        {
            if (StaticOptionsController != null && createdTabs.Count > 0)
            {
                foreach(KeyValuePair<OptionsTabType, Tab> pair in createdTabs)
                {
                    bool contains = false;
                    foreach (GameObject tab in StaticOptionsController._spawnedTabs)
                    {
                        if (tab != null && tab.name == pair.Value.TabName)
                        {
                            contains = true;
                            break;
                        }
                    }

                    if (!contains)
                    {
                        pair.Value.CreateTabObject();
                    }
                }
            } 
        }
    }
}