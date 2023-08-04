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
    public abstract class UIElement
    {
        public string Label { get; set; }
        public bool IsLocalizationTerm { get; set; }
        public UIElement(string label, bool isLocal = false)
        {
            Label = label;
            IsLocalizationTerm = isLocal;
        }

        public abstract Action<OptionsController> GetElement();
    }

    public class LabeledButton : UIElement
    {
        public string ButtonLabel { get; set; }
        public Action Action { get; set; }
        public LabeledButton(string label, string buttonLabel, Action action, bool isLocalizationTerm = false) : base(label, isLocalizationTerm)
        {
            ButtonLabel = buttonLabel;
            Action = action;
        }

        public override Action<OptionsController>  GetElement()
        {
            return (controller) => controller.AddLabeledButton(Label, ButtonLabel, Action, IsLocalizationTerm);
        }
    }

    public class Slider : UIElement
    {
        public float DefaultValue { get; set; }
        public Action<float> Action { get; set; }
        public Slider(string label, float defaultValue, Action<float> action, bool isLocalizationTerm = false) : base(label, isLocalizationTerm)
        {
            DefaultValue = defaultValue;
            Action = action;
        }
        public override Action<OptionsController> GetElement()
        {
            return (controller) => controller.AddSlider(Label, DefaultValue, Action, IsLocalizationTerm);
        }
    }

    public class TickBox : UIElement
    {
        public bool DefaultValue { get; set; }
        public Action<bool> Action { get; set; }
        public TickBox(string label, bool defaultValue, Action<bool> action, bool isLocalizationTerm = false) : base(label, isLocalizationTerm)
        {
            DefaultValue = defaultValue;
            Action = action;
        }
        public override Action<OptionsController> GetElement()
        {
            return (controller) => controller.AddTickBox(Label, DefaultValue, Action, IsLocalizationTerm);
        }
    }

    public class DropDown : UIElement
    {
        public int DefaultValue { get; set; }

        public Action<int> Action { get; set; }

        public List<string> Options { get; set; }

        public int VisibleOptions { get; set; }
        public DropDown(string label, List<string> options, int defaultValue, Action<int> action, int visibleOptions = 4, bool isLocalizationTerm = false) : base(label, isLocalizationTerm)
        {
            DefaultValue = defaultValue;
            Action = action;
            Options = options;
            VisibleOptions = visibleOptions;
        }
        public override Action<OptionsController> GetElement()
        {
            Il2CppSystem.Collections.Generic.List<string> Il2Options = new();
            Options.ForEach((option) => Il2Options.Add(option));
            return (controller) => controller.AddDropDown(Label, Il2Options, DefaultValue, Action, VisibleOptions, IsLocalizationTerm);
        }
    }

    public class MultipleChoice : UIElement
    {
        public List<string> ButtonLabels { get; set; }
        public List<Action> Actions { get; set; }
        public int DefaultValue { get; set; }
        public MultipleChoice(string label, List<string> buttonLabels, List<Action> actions, int defaultValue, bool isLocalizationTerm = false) : base(label, isLocalizationTerm)
        {
            ButtonLabels = buttonLabels;
            Actions = actions;
            DefaultValue = defaultValue;
        }
        public override Action<OptionsController> GetElement()
        {
            Il2CppSystem.Collections.Generic.List<string> Il2Labels = new();
            Il2CppSystem.Collections.Generic.List<Il2CppSystem.Action> Il2Actions = new();
            ButtonLabels.ForEach((label) => Il2Labels.Add(label));

            Actions.ForEach((action) => Il2Actions.Add(action));

            return (controller) => controller.AddMultipleChoice(Label, Il2Labels, Il2Actions, DefaultValue, IsLocalizationTerm);
        }
    }

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
            //UnityEngine.Object.DontDestroyOnLoad(sprite);
            createdTabs.Add(new(identifier, spritePath));
        }

        public void AddElementToTab(string identifier, UIElement element)
        {
            createdTabs.First((tab) => tab.TabName == identifier).AddElement(element);
        }

        //public void OnAddTabs(OptionsController controller)
        //{
        //    // Create the tab objects.
        //    Action<OptionsTabType> selectTab = (OptionsTabType type) => {
        //        if (controller != null)
        //        {
        //            Tab tab = createdTabs.First((tab) => tab.GetTabType() == type);
        //            controller.SelectTab(type);
        //            controller._Title.text = tab.TabName;
        //        }
        //    };

        //    foreach (Tab tab in createdTabs)
        //    {
                
        //        tab.GetElements().ForEach((element) => element.GetElement().Invoke(controller));
        //    }
        //}
        public void OnAddTabs(OptionsController controller)
        {
            Action<OptionsTabType> selectTab = (OptionsTabType type) =>
            {
                if (controller != null)
                {
                    Tab tab = createdTabs.First((tab) => tab.GetTabType() == type);
                    controller.SelectTab(type);
                    controller._Title.text = tab.TabName;
                }
            };

            //createdTabs.ForEach((tab) => tab.CreateTabObject(controller, selectTab));
        }

        public void Initialize_Prefix(OptionsController controller)
        {
            //Action<OptionsTabType> selectTab = (OptionsTabType type) => {
            //    if (controller != null)
            //    {
            //        Tab tab = createdTabs.First((tab) => tab.GetTabType() == type);
            //        controller.SelectTab(type);
            //        controller._Title.text = tab.TabName;
            //    }
            //};

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

        public Sprite? OnGetTabSprite(OptionsTabType type)
        {
            //Sprite sprite = 
                return createdTabs.Where((tab) => tab.GetTabType() == type)
                .Select((tab) => tab.GetSprite()).FirstOrDefault(null as Sprite);
            //if (sprite == null)
            //    return sprite;
            //return createdTabs.Where((tab) => tab.GetTabType() == type)
            //    .Select((tab) => tab.TabButtonSpritePath).FirstOrDefault(null as Sprite);
            //return createdTabs.Where((tab) => tab.GetTabType() == type)
            //    .Select((tab) => tab.TabName)
            //    .Select((name) => tabSprites[name])
            //    .FirstOrDefault(null as Sprite);
        }

        public string? OnGetTabName(OptionsTabType type)
        {
            return createdTabs.Where((tab) => tab.GetTabType() == type)
                .Select((pair) => pair.TabName).FirstOrDefault(null as string);
        }

        public bool OurTab(OptionsTabType type) => createdTabs.Where((tab) => tab.GetTabType() == type).Any();

        public void OnGenerateNavigation(OptionsController controller)
        {
            if (createdTabs.Count > 0) // Don't think we need this.
            {
                List<string> unrenderedTabs = createdTabs.Select((tab) => tab.TabName).ToList();

                foreach (GameObject obj in controller._spawnedTabs)
                {
                    // Just in case there's somehow duplicates...
                    while (unrenderedTabs.Remove(obj.name)) ;
                }

                unrenderedTabs.ForEach((name) => {
                    Tab tab = createdTabs.First((tab) => tab.TabName == name);
                    throw new Exception($"Tab: ${name} is unexpectedly gone!");
                    //tab.CreateTabObject()
                });
            }
        }
    }

    public class Tab {

        private GameObject tabObject;
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

        public void Init(OptionsController optionsController)  //, Action<OptionsTabType> onSelectTab)
        {
            if (alreadyInit)
                throw new Exception("Cannot init twice!");

            // TODO: v Not sure if this is needed or not? v
            optionsController._OptionsConfig.Add(TabType);

            //CreateTabObject(optionsController, onSelectTab);

            alreadyInit = true;
        }

        public Sprite GetSprite()
        {
            return SpriteImporter.LoadSprite(TabButtonSpritePath);
        }

        //public void CreateTabObject(OptionsController optionsController, Action<OptionsTabType> onSelectTab)
        //{
        //    tabObject = UnityEngine.Object.Instantiate(optionsController._TabPrefab);
        //    tabObject.name = TabName;

        //    tabObject.transform.parent = optionsController._TabContainer;

        //    Button tabButton = tabObject.GetComponent<Button>();
        //    tabButton.onClick.AddListener(new Action(() => onSelectTab(TabType)));
        //    tabButton.SetScale(1);

        //    Image tabImage = tabButton.GetComponent<Image>();
        //    tabImage.sprite = SpriteImporter.LoadSprite(TabButtonSpritePath);

        //    tabImage.name = TabName;
        //    tabImage.sprite.name = TabName;
        //    tabImage.sprite.texture.name = TabName;

        //    optionsController._spawnedTabs.Add(tabObject);
        //}
    }
}