using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Standalone UI: knows image coordinates only, never room/world coordinates.
public class CustomFurnishPreview : MaskableGraphic, IPointerClickHandler, IDragHandler
{
    [SerializeField] private RawImage picture;
    private Texture2D texture;
    private CustomFurnishKind kind;
    private Vector2Int anchor;
    private readonly HashSet<Vector2Int> blocked = new();
    public bool EditAnchor { get; set; }
    public Vector2Int Anchor => anchor;
    public event Action Changed;

    public void Configure(RawImage image) { picture = image; }

    public void SetImage(Texture2D image, CustomFurnishKind category)
    {
        texture = image;
        kind = category;
        anchor = Vector2Int.zero;
        blocked.Clear();
        picture.texture = image;
        picture.enabled = image != null;
        FitPicture();
        SetVerticesDirty();
        Changed?.Invoke();
    }

    public List<CustomFurnishCell> GetBlockedCells()
    {
        var result = new List<CustomFurnishCell>();
        foreach (var cell in blocked) result.Add(new CustomFurnishCell(cell.x, cell.y));
        result.Sort((a, b) => a.y != b.y ? a.y.CompareTo(b.y) : a.x.CompareTo(b.x));
        return result;
    }

    private Rect ImageRect()
    {
        Rect area = rectTransform.rect;
        if (texture == null) return area;
        float scale = Mathf.Min((area.width - 20) / texture.width, (area.height - 20) / texture.height);
        Vector2 size = new(texture.width * scale, texture.height * scale);
        return new Rect(area.center - size * .5f, size);
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        FitPicture();
    }

    private void FitPicture()
    {
        if (picture == null || texture == null) return;
        picture.rectTransform.anchorMin = picture.rectTransform.anchorMax = new Vector2(.5f, .5f);
        picture.rectTransform.pivot = new Vector2(.5f, .5f);
        picture.rectTransform.anchoredPosition = Vector2.zero;
        picture.rectTransform.sizeDelta = ImageRect().size;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        if (texture == null) return;
        Rect rect = ImageRect();
        float scale = rect.width / texture.width;
        Vector2 origin = rect.min + (Vector2)anchor * scale;
        float step = CustomFurnishData.PixelsPerCell * scale;
        if (kind != CustomFurnishKind.Decor)
        {
            int columns = Mathf.CeilToInt((texture.width - anchor.x) / 16f);
            int rows = Mathf.CeilToInt((texture.height - anchor.y) / 16f);
            for (int x = 0; x < columns; x++)
                for (int y = 0; y < rows; y++)
                {
                    Vector2 min = origin + new Vector2(x, y) * step;
                    Vector2 max = Vector2.Min(min + Vector2.one * step, rect.max);
                    bool red = blocked.Contains(new Vector2Int(x, y));
                    Quad(vh, min, max, red ? new Color(1, .15f, .15f, .4f) : new Color(1, 1, 1, .12f));
                    Color line = red ? new Color(1, .25f, .25f, .95f) : new Color(1, 1, 1, .8f);
                    Quad(vh, min, new Vector2(max.x, min.y + 1), line);
                    Quad(vh, min, new Vector2(min.x + 1, max.y), line);
                    Quad(vh, new Vector2(min.x, max.y - 1), max, line);
                    Quad(vh, new Vector2(max.x - 1, min.y), max, line);
                }
        }
        Quad(vh, origin - new Vector2(15, 3), origin + new Vector2(15, 3), Color.black);
        Quad(vh, origin - new Vector2(3, 15), origin + new Vector2(3, 15), Color.black);
        Color cross = EditAnchor ? new Color(1, .8f, .15f) : Color.cyan;
        Quad(vh, origin - new Vector2(14, 1.5f), origin + new Vector2(14, 1.5f), cross);
        Quad(vh, origin - new Vector2(1.5f, 14), origin + new Vector2(1.5f, 14), cross);
    }

    private static void Quad(VertexHelper vh, Vector2 min, Vector2 max, Color color)
    {
        int start = vh.currentVertCount;
        vh.AddVert(new Vector3(min.x, min.y), color, Vector2.zero);
        vh.AddVert(new Vector3(min.x, max.y), color, Vector2.zero);
        vh.AddVert(new Vector3(max.x, max.y), color, Vector2.zero);
        vh.AddVert(new Vector3(max.x, min.y), color, Vector2.zero);
        vh.AddTriangle(start, start + 1, start + 2);
        vh.AddTriangle(start + 2, start + 3, start);
    }

    public void OnPointerClick(PointerEventData e) { if (!e.dragging) ApplyPointer(e); }
    public void OnDrag(PointerEventData e) { if (EditAnchor) ApplyPointer(e); }

    private void ApplyPointer(PointerEventData e)
    {
        if (texture == null || e.button != PointerEventData.InputButton.Left) return;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, e.position, e.pressEventCamera, out var local)) return;
        Rect rect = ImageRect();
        if (!rect.Contains(local)) return;
        Vector2 pixel = (local - rect.min) * (texture.width / rect.width);
        if (EditAnchor)
        {
            var next = new Vector2Int(Mathf.Clamp(Mathf.RoundToInt(pixel.x), 0, texture.width - 1),
                Mathf.Clamp(Mathf.RoundToInt(pixel.y), 0, texture.height - 1));
            if (next == anchor) return;
            anchor = next;
            blocked.Clear();
        }
        else if (kind != CustomFurnishKind.Decor && pixel.x >= anchor.x && pixel.y >= anchor.y)
        {
            var cell = new Vector2Int(Mathf.FloorToInt((pixel.x - anchor.x) / 16), Mathf.FloorToInt((pixel.y - anchor.y) / 16));
            if (!blocked.Add(cell)) blocked.Remove(cell);
        }
        SetVerticesDirty();
        Changed?.Invoke();
    }
}
