using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class CustomFurnishEditorBuilder
{
    public const string PrefabPath = "Assets/Resources/UI/CustomFurnishEditor.prefab";
    private static TMP_FontAsset font;

    [MenuItem("Tools/DearLife/Build Custom Furnish Editor")]
    public static void Build()
    {
        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath("8fd456a17277d1c42a549978ca4cbffb"));
        if (font == null) throw new System.InvalidOperationException("Project UI font is missing");
        var root = Rect("CustomFurnishEditor", null, Vector2.zero, Vector2.one);
        root.gameObject.SetActive(false);
        var background = root.gameObject.AddComponent<Image>();
        background.color = new Color(.08f, .11f, .16f, .98f);
        var canvas = root.gameObject.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 200;
        root.gameObject.AddComponent<GraphicRaycaster>();
        var editor = root.gameObject.AddComponent<CustomFurnishEditor>();

        var title = Label("Title", root, "导入家具", new Vector2(.08f, .90f), new Vector2(.92f, .96f), 44);
        var nameRect = Rect("Name", root, new Vector2(.08f, .83f), new Vector2(.68f, .89f));
        nameRect.gameObject.AddComponent<Image>().color = new Color(.19f, .23f, .30f);
        var input = nameRect.gameObject.AddComponent<TMP_InputField>();
        var textArea = Rect("TextArea", nameRect, new Vector2(.04f, .08f), new Vector2(.96f, .92f));
        textArea.gameObject.AddComponent<RectMask2D>();
        var nameText = Label("Text", textArea, "", Vector2.zero, Vector2.one, 30);
        nameText.alignment = TextAlignmentOptions.MidlineLeft;
        var placeholder = Label("Placeholder", textArea, "素材名称", Vector2.zero, Vector2.one, 30);
        placeholder.color = new Color(.7f, .75f, .8f);
        placeholder.alignment = TextAlignmentOptions.MidlineLeft;
        input.textViewport = textArea;
        input.textComponent = nameText;
        input.placeholder = placeholder;
        input.characterLimit = 40;
        input.lineType = TMP_InputField.LineType.SingleLine;
        var pick = Button("Pick", root, "选择图片", new Vector2(.70f, .83f), new Vector2(.92f, .89f));

        var work = Rect("WorkArea", root, new Vector2(.06f, .29f), new Vector2(.94f, .80f));
        work.gameObject.AddComponent<Image>().color = new Color(.23f, .27f, .33f);
        var picture = Rect("Picture", work, new Vector2(.5f, .5f), new Vector2(.5f, .5f)).gameObject.AddComponent<RawImage>();
        picture.raycastTarget = false;
        picture.enabled = false;
        var preview = Rect("GridAndAnchor", work, Vector2.zero, Vector2.one).gameObject.AddComponent<CustomFurnishPreview>();
        preview.Configure(picture);
        preview.raycastTarget = true;

        var status = Label("Status", root, "请选择图片", new Vector2(.07f, .18f), new Vector2(.93f, .27f), 28);
        var mode = Button("AnchorMode", root, "修改锚点：开", new Vector2(.07f, .11f), new Vector2(.53f, .17f));
        Label("Limit", root, "宽、高 ≤ 500\n固定16像素/格", new Vector2(.56f, .11f), new Vector2(.93f, .17f), 26);
        var cancel = Button("Cancel", root, "取消", new Vector2(.07f, .035f), new Vector2(.47f, .095f));
        var save = Button("Save", root, "保存到素材库", new Vector2(.53f, .035f), new Vector2(.93f, .095f));
        save.GetComponent<Image>().color = new Color(.18f, .48f, .39f);
        editor.Configure(title, input, status, mode.GetComponentInChildren<TMP_Text>(), pick, mode, save, cancel, preview);
        root.gameObject.SetActive(true);
        Directory.CreateDirectory(Path.GetDirectoryName(PrefabPath));
        PrefabUtility.SaveAsPrefabAsset(root.gameObject, PrefabPath);
        Object.DestroyImmediate(root.gameObject);
        AssetDatabase.SaveAssets();
        Debug.Log("CUSTOM_FURNISH_PREFAB_CREATED: " + PrefabPath);
    }

    private static RectTransform Rect(string name, Transform parent, Vector2 min, Vector2 max)
    {
        var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        if (parent != null) rect.SetParent(parent, false);
        rect.anchorMin = min; rect.anchorMax = max;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        return rect;
    }

    private static TextMeshProUGUI Label(string name, Transform parent, string value, Vector2 min, Vector2 max, float size)
    {
        var text = Rect(name, parent, min, max).gameObject.AddComponent<TextMeshProUGUI>();
        text.font = font; text.text = value; text.fontSize = size;
        text.color = new Color(.93f, .95f, 1);
        text.alignment = TextAlignmentOptions.Center;
        text.enableAutoSizing = true;
        text.fontSizeMin = size * .65f; text.fontSizeMax = size;
        text.raycastTarget = false;
        return text;
    }

    private static Button Button(string name, Transform parent, string label, Vector2 min, Vector2 max)
    {
        var rect = Rect(name, parent, min, max);
        var image = rect.gameObject.AddComponent<Image>();
        image.color = new Color(.24f, .32f, .43f);
        var button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        Label("Label", rect, label, new Vector2(.04f, .08f), new Vector2(.96f, .92f), 30);
        return button;
    }
}
