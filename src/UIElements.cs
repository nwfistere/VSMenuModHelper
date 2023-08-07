using Il2CppVampireSurvivors.UI;
using System;
using System.Collections.Generic;

namespace VSMenuModHelper
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

        public override Action<OptionsController> GetElement()
        {
            return (controller) => controller.AddLabeledButton(Label, ButtonLabel, Action, IsLocalizationTerm);
        }
    }

    public class Slider : UIElement
    {
        public Func<float> GetterDelegate { get; set; }
        public Action<float> Action { get; set; }

        public Slider(string label, Func<float> getterDelegate, Action<float> action, bool isLocalizationTerm = false) : base(label, isLocalizationTerm)
        {
            GetterDelegate = getterDelegate;
            Action = action;
        }
        public override Action<OptionsController> GetElement()
        {
            return (controller) => controller.AddSlider(Label, GetterDelegate(), Action, IsLocalizationTerm);
        }
    }

    public class TickBox : UIElement
    {
        public Func<bool> GetterDelegate { get; set; }
        public Action<bool> Action { get; set; }
        public TickBox(string label, Func<bool> getterDelegate, Action<bool> action, bool isLocalizationTerm = false) : base(label, isLocalizationTerm)
        {
            GetterDelegate = getterDelegate;
            Action = action;
        }
        public override Action<OptionsController> GetElement()
        {
            return (controller) => controller.AddTickBox(Label, GetterDelegate(), Action, IsLocalizationTerm);
        }
    }

    public class DropDown : UIElement
    {
        public Func<int> GetterDelegate { get; set; }

        public Action<int> Action { get; set; }

        public List<string> Options { get; set; }

        public int VisibleOptions { get; set; }
        public DropDown(string label, List<string> options, Func<int> getterDelegate, Action<int> action, int visibleOptions = 4, bool isLocalizationTerm = false) : base(label, isLocalizationTerm)
        {
            GetterDelegate = getterDelegate;
            Action = action;
            Options = options;
            VisibleOptions = visibleOptions;
        }
        public override Action<OptionsController> GetElement()
        {
            Il2CppSystem.Collections.Generic.List<string> Il2Options = new();
            Options.ForEach((option) => Il2Options.Add(option));
            return (controller) => controller.AddDropDown(Label, Il2Options, GetterDelegate(), Action, VisibleOptions, IsLocalizationTerm);
        }
    }

    public class MultipleChoice : UIElement
    {
        public List<string> ButtonLabels { get; set; }
        public List<Action> Actions { get; set; }
        public Func<int> GetterDelegate { get; set; }
        public MultipleChoice(string label, List<string> buttonLabels, List<Action> actions, Func<int> getterDelegate, bool isLocalizationTerm = false) : base(label, isLocalizationTerm)
        {
            ButtonLabels = buttonLabels;
            Actions = actions;
            GetterDelegate = getterDelegate;
        }
        public override Action<OptionsController> GetElement()
        {
            Il2CppSystem.Collections.Generic.List<string> Il2Labels = new();
            Il2CppSystem.Collections.Generic.List<Il2CppSystem.Action> Il2Actions = new();
            ButtonLabels.ForEach((label) => Il2Labels.Add(label));

            Actions.ForEach((action) => Il2Actions.Add(action));

            return (controller) => controller.AddMultipleChoice(Label, Il2Labels, Il2Actions, GetterDelegate(), IsLocalizationTerm);
        }
    }

    public class Title : UIElement
    {
        public Title(string title) : base(title, false) { }

        public override Action<OptionsController> GetElement() => (controller) => controller._Title.text = this.Label;
    }
}
