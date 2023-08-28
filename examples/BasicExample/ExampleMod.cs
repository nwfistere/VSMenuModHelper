using MelonLoader;
using System;
using System.IO;
using UnityEngine;
using VSMenuModHelper;

namespace ExampleMod
{
    public static class ModInfo
    {
        public const string Name = "Example";
        public const string Description = "Example mod for VSMenuModHelper.";
        public const string Author = "Nick";
        public const string Company = "Nick";
        public const string Version = "1.0.0";
        public const string Download = "https://github.com/nwfistere/VSMenuModHelper/tree/main/examples/BasicExample";
    }

    public class ExampleMod : MelonMod
    {
        private MelonPreferences_Category? preferences;
        private static MelonPreferences_Entry<bool> enabled;
        private static MelonPreferences_Entry<bool> someToggle;
        private static MelonPreferences_Entry<float> somePercentage;
        private static MelonPreferences_Entry<bool> buttonPressed;
        private static MelonPreferences_Entry<int> dropDownValue;
        private static MelonPreferences_Entry<int> multipleChoiceValue;

        public override void OnInitializeMelon()
        {
            preferences = MelonPreferences.CreateCategory("example_preferences");
            enabled = preferences.CreateEntry("enabled", true);
            someToggle = preferences.CreateEntry("someToggle", true);
            somePercentage = preferences.CreateEntry("somePercentage", 1f);
            buttonPressed = preferences.CreateEntry("buttonPressed", false);
            dropDownValue = preferences.CreateEntry("dropDownValue", 0);
            multipleChoiceValue = preferences.CreateEntry("multipleChoiceValue", 0);
            
            DeclareMenuTabs();
            
        }

        private void LogValues()
        {
            Action<string> log = (str) => LoggerInstance.Msg($"\t{str}");
            LoggerInstance.Msg($"preferences:");
            log($"enabled: {enabled.Value}");
            log($"someToggle: {someToggle.Value}");
            log($"somePercentage: {somePercentage.Value}");
            log($"buttonPressed: {buttonPressed.Value}");
            log($"dropDownValue: {dropDownValue.Value}");
            log($"multipleChoiceValue: {multipleChoiceValue.Value}");
        }

        private void DeclareMenuTabs()
        {
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "resources", "example", "some-icon.png");
            Uri imageUrl = new("https://github.com/nwfistere/VSMenuModHelper/blob/main/examples/BasicExample/resources/example/game-controller.png?raw=true");

            VSMenuHelper.Instance.DeclareOptionsTab("Config Tab", imagePath);
            VSMenuHelper.Instance.DeclareOptionsTab("Empty Tab", imageUrl);

            VSMenuHelper.Instance.AddTabSpriteModifier("Empty Tab", (sprite) => {
                sprite.texture.filterMode = UnityEngine.FilterMode.Point;
                return sprite;
            });

            VSMenuHelper.Instance.AddTabSpriteModifier("Empty Tab", (sprite) => {
                sprite.texture.ReinitializeWithFormatImpl(40, 40, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_SInt, true);
                return sprite;
            });

            VSMenuHelper.Instance.AddElementToTab("Config Tab", new Title("Config Tab"));
            VSMenuHelper.Instance.AddElementToTab("Config Tab", new TickBox("enabled", () => enabled.Value, (value) => enabled.Value = value));
            VSMenuHelper.Instance.AddElementToTab("Config Tab", new TickBox("someToggle", () => someToggle.Value, (value) => someToggle.Value = value));
            VSMenuHelper.Instance.AddElementToTab("Config Tab", new LabeledButton("LabeledButton", "log values", () => LogValues()));
            VSMenuHelper.Instance.AddElementToTab("Config Tab", new Slider("Slider", () => somePercentage.Value, (value) => somePercentage.Value = value));
            VSMenuHelper.Instance.AddElementToTab("Config Tab", new DropDown("DropDown", new() { "one", "two", "three" }, () => dropDownValue.Value, (value) => dropDownValue.Value = value));
            Action<int> action = (value) => multipleChoiceValue.Value = value;
            VSMenuHelper.Instance.AddElementToTab("Config Tab", new MultipleChoice("MultipleChoice", new() { "one", "two", "three" }, new() { () => action(0), () => action(1), () => action(2) }, () => multipleChoiceValue.Value));
        }
    }
}
