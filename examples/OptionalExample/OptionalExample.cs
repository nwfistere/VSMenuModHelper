using MelonLoader;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace OptionalExample
{
    public static class ModInfo
    {
        public const string Name = "Optional Example";
        public const string Description = "Example mod that makes VSMenuModHelper optional.";
        public const string Author = "Nick";
        public const string Company = "Nick";
        public const string Version = "1.0.0";
        public const string Download = "https://github.com/nwfistere/VSMenuModHelper/tree/main/examples";
    }
    public class OptionalExample : MelonMod
    {
        private MelonPreferences_Category? preferences;
        private static MelonPreferences_Entry<bool> enabled;
        private static MelonPreferences_Entry<bool> someToggle;
        private static MelonPreferences_Entry<float> somePercentage;
        private static MelonPreferences_Entry<bool> buttonPressed;
        private static MelonPreferences_Entry<int> dropDownValue;
        private static MelonPreferences_Entry<int> multipleChoiceValue;

        public static bool FoundVSMenuModHelper = false;
        public override void OnInitializeMelon()
        {
            bool WeShouldLookForVSMenuHelper = true;

            preferences = MelonPreferences.CreateCategory(ModInfo.Name);
            enabled = preferences.CreateEntry("enabled", true);
            someToggle = preferences.CreateEntry("someToggle", true);
            somePercentage = preferences.CreateEntry("somePercentage", 1f);
            buttonPressed = preferences.CreateEntry("buttonPressed", false);
            dropDownValue = preferences.CreateEntry("dropDownValue", 0);
            multipleChoiceValue = preferences.CreateEntry("multipleChoiceValue", 0);

            if (WeShouldLookForVSMenuHelper)
            {
                string FullPathOfVSMenuModHelperDll = Path.Combine(Directory.GetCurrentDirectory(), "Mods", "VSMenuModHelper.dll");
                MelonAssembly VSMenuModHelperAssembly = MelonAssembly.LoadMelonAssembly(FullPathOfVSMenuModHelperDll);
                if (VSMenuModHelperAssembly != null)
                {
                    LoggerInstance.Msg("Found - VSMenuModHelperAssembly!");
                    FoundVSMenuModHelper = true;
                }
                else
                    LoggerInstance.Warning("Not Found - VSMenuModHelperAssembly");
            }
        }

        public override void OnLateInitializeMelon()
        {
            if (FoundVSMenuModHelper && preferences != null)
            {
                HandleVSMenuModHelper menuHelper = new(preferences);
            }

            buttonPressed.Value = true;
            preferences.SaveToFile();
        }
    }
}