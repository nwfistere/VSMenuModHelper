using Il2CppInterop.Runtime;
using Il2CppTMPro;
using Il2CppVampireSurvivors.App.Tools;
using Il2CppVampireSurvivors.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.TextCore.Text;

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

        public virtual void Destroy() { /* Do nothing by default. */ }
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

    public class TextInput : UIElement
    {
        public Func<string> GetterDelegate { get; set; }
        public Action<bool> Action { get; set; }

        public Func<string> PlaceHolderDelegate { get; set; }

        public LabeledInputUI LabeledInputUI { get; set; }

        private TextMeshProUGUI textMesh { get; set; }

        public TextInput(string label, Func<string> getterDelegate, Func<string> placeholderDelegate, bool isLocalizationTerm = false) : base(label, isLocalizationTerm)
        {
            GetterDelegate = getterDelegate;
            PlaceHolderDelegate = placeholderDelegate;
        }

        public override Action<OptionsController> GetElement()
        {
            return (controller) => LabeledInputUI = controller.AddLabeledInput(Label, GetterDelegate(), PlaceHolderDelegate(), IsLocalizationTerm);
        }

        public string getValue()
        {
            if (LabeledInputUI)
            {
                if (!textMesh)
                {
                    foreach (TextMeshProUGUI tmpug in LabeledInputUI.GetComponentsInChildren<TextMeshProUGUI>())
                    {
                        if (tmpug.name == "Text")
                        {
                            textMesh = tmpug;
                        }
                    }
                }

                // Seems as though there's overflow happening with this text, do a substring on the size of text.
                return textMesh.text.Substring(0, textMesh.m_InternalTextProcessingArraySize - 1);
            }
            return "";
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
        public static List<TMP_Sprite> sprites = new();

        public override Action<OptionsController> GetElement()
        {
            return (controller) =>
            {
                TickBoxUI tickBoxUI = controller.AddTickBox(Label, GetterDelegate(), Action, IsLocalizationTerm);

                Transform childLabel = tickBoxUI.GetGameObject().transform.FindChild("Label");
                TextMeshProUGUI label = childLabel.GetComponent<TextMeshProUGUI>();

                Sprite s = SpriteImporter.LoadSprite("C:\\Users\\Nick\\Pictures\\00051-1768801398-smol.png", new Rect(0, 0, 30, 36), new Vector2(0f, 0f));
                s.name = "test";

                TMP_SpriteAsset spriteAsset = new()
                {
                    spriteSheet = s.texture,
                    name = Path.GetFileNameWithoutExtension("C:\\Users\\Nick\\Pictures\\00051-1768801398-smol.png") ?? "",                    
                };
                spriteAsset.hashCode = TMP_TextUtilities.GetSimpleHashCode(spriteAsset.name);

                TMP_Sprite sprite = new();
                sprite.id = i++;
                sprite.name = Path.GetFileNameWithoutExtension("C:\\Users\\Nick\\Pictures\\00051-1768801398-smol.png") ?? "";
                sprite.hashCode = TMP_TextUtilities.GetSimpleHashCode(sprite.name);

                int unicode = TMP_TextUtilities.StringHexToInt(sprite.name);
                sprite.unicode = unicode;
                sprite.x = 0;
                sprite.y = 0;
                sprite.width = s.texture.width;
                sprite.height = s.texture.height;
                sprite.pivot = new Vector2(0, 0);
                sprite.xAdvance = sprite.width;
                sprite.scale = 1.0f;
                sprite.xOffset = 0 - (sprite.width * sprite.pivot.x);
                sprite.yOffset = sprite.height - (sprite.height * sprite.pivot.y);
                sprites.Add(sprite);

                TMP_SpriteGlyph spriteGlyph = new();
                spriteGlyph.index = (uint)(i - 1);
                spriteGlyph.sprite = sprite.sprite;
                spriteGlyph.metrics = new(sprite.width, sprite.height, ((-sprite.width) / 2f) + sprite.pivot.x, sprite.height / 2f + sprite.pivot.y, sprite.xAdvance);
                spriteGlyph.glyphRect = new((int)sprite.x, (int)sprite.y, (int)sprite.width, (int)sprite.height);
                spriteGlyph.scale = 1.0f;
                spriteGlyph.atlasIndex = 0;

                spriteAsset.spriteGlyphTable.Add(spriteGlyph);

                spriteAsset.spriteInfoList = new();
                spriteAsset.spriteInfoList.Add(sprite);

                TMP_SpriteCharacter spriteCharacter = new((uint)sprite.unicode, spriteGlyph);
                spriteCharacter.name = sprite.name;
                spriteCharacter.scale = sprite.scale;

                spriteAsset.spriteCharacterTable.Add(spriteCharacter);

                spriteAsset.UpdateLookupTables();

                Shader shader = Shader.Find("TextMeshPro/Sprite");
                Material material = new Material(shader);
                material.SetTexture(ShaderUtilities.ID_MainTex, spriteAsset.spriteSheet);

                spriteAsset.material = material;
                material.hideFlags = HideFlags.HideInHierarchy;

                label.text = "<voffset=0.5em><sprite=0 name=\"00051-1768801398-smol\"></voffset>" + label.text;

                //spriteAsset.material = label.material;
                label.spriteAsset = spriteAsset;
                
                spriteAssets.Add(spriteAsset);

            };
        }
    }

    public class HorizontalRule : UIElement
    {

        private GameObject Label { get; set; }

        public HorizontalRule() : base("", false) {}
        public override Action<OptionsController> GetElement()
        {
            return (controller) => //{ };
            {
                RectTransform Content = controller._Content;
                Label = GameObject.Instantiate(controller._ButtonPrefab.transform.Find("Label").gameObject, Content);
                Label.name = "Options_Horizontal_Rule";
                TextMeshProUGUI textMesh = Label.GetComponent<TextMeshProUGUI>();
                textMesh.text = "___________________________________________";
            };
        }

        public override void Destroy()
        {
            GameObject.DestroyImmediate(Label);
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


    public class ColorDropDown : UIElement
    {
        public Func<int> GetterDelegate { get; set; }

        public Action<int> Action { get; set; }

        public List<Color> Options { get; set; }

        public int VisibleOptions { get; set; }

        public ColorDropDown(string label, List<Color> options, Func<int> getterDelegate, Action<int> action, int visibleOptions = 4, bool isLocalizationTerm = false) : base(label, isLocalizationTerm)
        {
            GetterDelegate = getterDelegate;
            Action = action;
            Options = options;
            VisibleOptions = visibleOptions;
        }
        public override Action<OptionsController> GetElement()
        {
            Il2CppSystem.Collections.Generic.List<Color> Il2Options = new();
            Options.ForEach((option) => Il2Options.Add(option));
            return (controller) => controller.AddColourDropDown(Label, Il2Options, GetterDelegate(), Action, VisibleOptions, IsLocalizationTerm);
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
