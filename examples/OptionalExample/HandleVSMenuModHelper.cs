using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VSMenuModHelper;

namespace OptionalExample
{
    internal static class CategoryExtension
    {
        internal static T Get<T>(this MelonPreferences_Category category, string entry)
        {
            return category.GetEntry<T>(entry).Value;
        }

        internal static T Set<T>(this MelonPreferences_Category category, string entry, T value)
        {
            return category.GetEntry<T>(entry).Value = value;
        }
    }

    internal class HandleVSMenuModHelper
    {

        private VSMenuHelper vsMenuHelper;
        private MelonPreferences_Category category;
        public static HandleVSMenuModHelper Instance { get; private set; }
        public HandleVSMenuModHelper(MelonPreferences_Category category)
        {
            Instance = this;
            this.category = category;
            this.vsMenuHelper = VSMenuHelper.Instance;
            DeclareMenuTabs();
        }

        private void LogValues()
        {
            Action<string> log = (str) => Melon<OptionalExample>.Logger.Msg($"\t{str}");
            Melon<OptionalExample>.Logger.Msg($"preferences:");
            log($"enabled: {category.Get<bool>("enabled")}");
            log($"someToggle: {category.Get<bool>("someToggle")}");
            log($"somePercentage: {category.Get<float>("somePercentage")}");
            log($"dropDownValue: {category.Get<int>("dropDownValue")}");
            log($"multipleChoiceValue: {category.Get<int>("multipleChoiceValue")}");
        }

        private void DeclareMenuTabs()
        {
            bool enabled = category.GetEntry<bool>("enabled").Value;

            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "resources", "example", "some-icon.png");

            Action<int> action = (value) => category.Set("multipleChoiceValue", value);

            vsMenuHelper.DeclareOptionsTab("Optional Config Tab", imagePath);

            vsMenuHelper.AddElementToTab("Optional Config Tab", new Title("Optional Config Tab"));
            vsMenuHelper.AddElementToTab("Optional Config Tab", new TickBox("enabled", () => category.Get<bool>("enabled"), (value) => category.Set("enabled", value)));
            vsMenuHelper.AddElementToTab("Optional Config Tab", new TickBox("someToggle", () => category.Get<bool>("someToggle"), (value) => category.Set("someToggle", value)));
            vsMenuHelper.AddElementToTab("Optional Config Tab", new LabeledButton("LabeledButton", "log values", () => LogValues()));
            vsMenuHelper.AddElementToTab("Optional Config Tab", new Slider("Slider", () => category.Get<float>("somePercentage"), (value) => category.Set("somePercentage", value)));
            vsMenuHelper.AddElementToTab("Optional Config Tab", new DropDown("DropDown", new() { "one", "two", "three" }, () => category.Get<int>("dropDownValue"), (value) => category.Set("dropDownValue", value)));
            vsMenuHelper.AddElementToTab("Optional Config Tab", new MultipleChoice("MultipleChoice", new() { "one", "two", "three" }, new() { () => action(0), () => action(1), () => action(2) }, () => category.Get<int>("multipleChoiceValue")));
            vsMenuHelper.AddElementToTab("Optional Config Tab", new LabeledButton("", "save", () => category.SaveToFile()));

        }
    }
}
