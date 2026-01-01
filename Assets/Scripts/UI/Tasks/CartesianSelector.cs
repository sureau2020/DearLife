using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CartesianSelector : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField] private RectTransform point;
    [SerializeField] private RectTransform area;

    public float Duration { get; private set; } = 2f;
    public float Difficulty { get; private set; } = 2f;

    private Vector2 min = Vector2.zero;
    private Vector2 max = new Vector2(4, 4);

    private void Start()
    {
        SetPointByValue(Duration, Difficulty);
    }

    public void OnPointerDown(PointerEventData eventData) => OnDrag(eventData);

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(area, eventData.position, eventData.pressEventCamera, out localPoint);
        Vector2 size = area.rect.size;
        Vector2 normalized = new Vector2(
            Mathf.Clamp01((localPoint.x / size.x) + 0.5f),
            Mathf.Clamp01((localPoint.y / size.y) + 0.5f)
        );
        Duration = Mathf.Lerp(min.x, max.x, normalized.x);
        Difficulty = Mathf.Lerp(min.y, max.y, normalized.y);
        SetPointByValue(Duration, Difficulty);
    }

    public void SetPointByValue(float duration, float difficulty)
    {
        Vector2 size = area.rect.size;
        Vector2 normalized = new Vector2(
            Mathf.InverseLerp(min.x, max.x, duration),
            Mathf.InverseLerp(min.y, max.y, difficulty)
        );
        point.anchoredPosition = new Vector2(
            (normalized.x - 0.5f) * size.x,
            (normalized.y - 0.5f) * size.y
        );
        this.Duration = duration;
        this.Difficulty = difficulty;
    }

}
