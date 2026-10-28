using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Inject storage and completion callbacks; no FurnishManager, RoomView or GameManager dependency.
public class CustomFurnishEditor : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_InputField itemName;
    [SerializeField] private TMP_Text status;
    [SerializeField] private TMP_Text modeLabel;
    [SerializeField] private Button pickButton;
    [SerializeField] private Button modeButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private CustomFurnishPreview preview;
    private CustomFurnishLibrary library;
    private CustomFurnishKind kind;
    private Action saved, closed;
    private Texture2D image;
    private int requestVersion;
    private bool picking, saving, completed;

    public void Configure(TMP_Text heading, TMP_InputField nameInput, TMP_Text hint, TMP_Text anchorLabel,
        Button pick, Button mode, Button save, Button cancel, CustomFurnishPreview canvas)
    {
        title = heading; itemName = nameInput; status = hint; modeLabel = anchorLabel;
        pickButton = pick; modeButton = mode; saveButton = save; cancelButton = cancel; preview = canvas;
    }

    private void Awake()
    {
        pickButton.onClick.AddListener(PickImage);
        modeButton.onClick.AddListener(ToggleAnchor);
        saveButton.onClick.AddListener(Save);
        cancelButton.onClick.AddListener(Close);
        preview.Changed += Refresh;
    }

    public void Initialize(CustomFurnishKind category, CustomFurnishLibrary storage, Action onSaved, Action onClosed)
    {
        kind = category; library = storage; saved = onSaved; closed = onClosed;
        string label = category == CustomFurnishKind.Furniture ? "家具" : category == CustomFurnishKind.Decor ? "装饰" : "房间背景";
        title.text = "导入" + label;
        itemName.text = "我的" + label;
        preview.EditAnchor = true;
        Refresh();
    }

    private void PickImage()
    {
        if (picking || saving || NativeGallery.IsMediaPickerBusy()) return;
        picking = true;
        int version = ++requestVersion;
        Refresh();
        try
        {
            NativeGallery.GetImageFromGallery(path =>
            {
                if (this == null || !isActiveAndEnabled || version != requestVersion) return;
                picking = false;
                if (string.IsNullOrEmpty(path)) { Refresh(); return; }
                Texture2D candidate = null;
                try
                {
                    var properties = NativeGallery.GetImageProperties(path);
                    if (properties.width > 500 || properties.height > 500)
                        throw new InvalidOperationException($"图片为 {properties.width}×{properties.height}，宽和高都不能超过 500 像素");
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
                    if (properties.width <= 0 || properties.height <= 0)
                        throw new InvalidOperationException("无法确认图片尺寸，请选择 PNG 或 JPG 图片");
#endif
                    // Check original dimensions first. Do not pass 500 here: it would silently resize.
                    candidate = NativeGallery.LoadImageAtPath(path, -1, false, false);
                    if (candidate == null) throw new InvalidOperationException("无法读取图片");
                    if (candidate.width > 500 || candidate.height > 500)
                        throw new InvalidOperationException($"图片为 {candidate.width}×{candidate.height}，宽和高都不能超过 500 像素");
                    candidate.filterMode = FilterMode.Point;
                    candidate.wrapMode = TextureWrapMode.Clamp;
                    if (image != null) Destroy(image);
                    image = candidate;
                    candidate = null;
                    preview.EditAnchor = true;
                    preview.SetImage(image, kind);
                    Refresh();
                }
                catch (Exception e) { Refresh(); status.text = e.Message; }
                finally { if (candidate != null) Destroy(candidate); }
            }, "选择图片（宽、高均不超过500像素）", "image/*");
        }
        catch (Exception e) { picking = false; Refresh(); status.text = "打开相册失败：" + e.Message; }
    }

    private void ToggleAnchor()
    {
        preview.EditAnchor = !preview.EditAnchor;
        preview.SetVerticesDirty();
        Refresh();
    }

    private void Refresh()
    {
        pickButton.interactable = !picking && !saving;
        modeButton.interactable = image != null && !picking && !saving;
        saveButton.interactable = image != null && !picking && !saving;
        cancelButton.interactable = !saving;
        modeLabel.text = preview.EditAnchor ? "修改锚点：开" : "修改锚点：关";
        if (image == null) { status.text = "请选择图片：宽、高均不超过 500 像素"; return; }
        string mode = preview.EditAnchor ? "点击或拖动设置锚点；移动锚点将清除红格"
            : kind == CustomFurnishKind.Decor ? "装饰没有碰撞格子，可以保存"
            : "点击格子切换：白色可走，红色阻挡";
        status.text = $"{image.width}×{image.height} 像素 · 16像素/格 · 锚点({preview.Anchor.x}, {preview.Anchor.y})\n{mode}";
    }

    private void Save()
    {
        if (saving || picking || image == null || library == null) return;
        var draft = new CustomFurnishData {
            displayName = itemName.text.Trim(), kind = kind, width = image.width, height = image.height,
            anchorX = preview.Anchor.x, anchorY = preview.Anchor.y, blockedCells = preview.GetBlockedCells()
        };
        saving = true;
        Refresh();
        var result = library.Save(draft, image);
        saving = false;
        if (!result.Success) { Refresh(); status.text = result.Message; return; }
        try { saved?.Invoke(); }
        finally { Close(); }
    }

    public void Close()
    {
        if (saving || completed) return;
        completed = true;
        requestVersion++;
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        requestVersion++;
        picking = false;
        var callback = closed;
        closed = null;
        callback?.Invoke();
    }

    private void OnDestroy()
    {
        preview.Changed -= Refresh;
        if (image != null) Destroy(image);
    }
}
