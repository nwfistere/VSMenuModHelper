using Il2CppTMPro;
using Il2CppVampireSurvivors.App.Tools;
using Il2CppVampireSurvivors.UI;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.TextCore;

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

    public class TickBoxPro : UIElement
    {
        public Func<bool> GetterDelegate { get; set; }
        public Action<bool> Action { get; set; }
        public TickBoxPro(string label, Func<bool> getterDelegate, Action<bool> action, bool isLocalizationTerm = false) : base(label, isLocalizationTerm)
        {
            GetterDelegate = getterDelegate;
            Action = action;
        }
        public static int i = 0;

        public static List<TMP_SpriteAsset> spriteAssets = new();
        public override Action<OptionsController> GetElement()
        {
            //return (controller) => controller.AddTickBox(Label, GetterDelegate(), Action, IsLocalizationTerm);
            return (controller) =>
            {
                TickBoxUI tickBoxUI = controller.AddTickBox(Label, GetterDelegate(), Action, IsLocalizationTerm);

                Transform childLabel = tickBoxUI.GetGameObject().transform.FindChild("Label");
                TextMeshProUGUI label = childLabel.GetComponent<TextMeshProUGUI>();

                TMP_SpriteAsset spriteAsset = new();


                Sprite sprite = SpriteImporter.LoadSprite("C:\\Users\\Nick\\Pictures\\00051-1768801398-smol.png", new Rect(0, 0, 30, 36), new Vector2(0f, 0f));
                sprite.name = "test";

                spriteAsset = new()
                {
                    spriteSheet = sprite.texture,
                    name = "test"
                };

                //label.spriteAsset = spriteAsset;
                //label.m_currentSpriteAsset = spriteAsset;
                //label.m_defaultSpriteAsset = spriteAsset;

                GlyphMetrics metrics = new GlyphMetrics(sprite.texture.width, sprite.texture.height, -(sprite.texture.height), 0, sprite.texture.width);
                GlyphRect rect = new(0, 0, Convert.ToInt16(metrics.width), Convert.ToInt16(metrics.height));
                // GlyphRect rect = new(0, 0, 1, 1);

                TMP_SpriteGlyph spriteGlyph = new(0, metrics, rect, 1f, 0, sprite);
                TMP_SpriteCharacter newSprite = new(0, spriteAsset, spriteGlyph);
                newSprite.name = "test";
                TMP_Sprite tmp_sprite = new()
                {
                    sprite = sprite,
                    id = 0,
                    unicode = (int)newSprite.unicode,
                    name = "test",
                    x = 0,
                    y = 0,
                    pivot = sprite.pivot,
                    xAdvance = sprite.texture.width,
                    width = sprite.texture.width,
                    height = sprite.texture.height,
                    scale = 1,
                    xOffset = sprite.texture.width / 2,
                    yOffset = 0,
                    hashCode = newSprite.hashCode
                };

                    


                //spriteAsset.spriteInfoList = new();

                //spriteAsset.spriteInfoList.Add(tmp_sprite);
                spriteAsset.spriteCharacterTable.Add(newSprite);
                spriteAsset.spriteCharacterLookupTable.Add((uint)tmp_sprite.id, newSprite);
                spriteAsset.spriteGlyphTable.Add(spriteGlyph);
                spriteAsset.material = label.material;
                spriteAsset.UpdateLookupTables();
                //spriteAsset.SetDirty();
                //label.textInfo.characterInfo[0].spriteAsset = label.spriteAsset;
                //label.textInfo.characterInfo[0].spriteIndex = 0;

                label.text = "<sprite=0 name=\"test\"> <sprite name=\"test\"> <sprite=0>";

                spriteAssets.Add(spriteAsset);

            };
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
