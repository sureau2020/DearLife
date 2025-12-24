using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InfoInputAnimator : MonoBehaviour
{
    [SerializeField] private RectTransform panelRect;

    private float offscreenTop = -1920f;
    private float visibleTop = 0f;
    private float duration = 0.5f;
    private Ease showEase = Ease.OutCubic;
    private Ease hideEase = Ease.OutCubic;

    private void Awake()
    {
        if (panelRect == null)
            panelRect = GetComponent<RectTransform>();
    }

    public void SlideIn()
    {
        if (panelRect == null) return;

        panelRect.DOKill();

        panelRect.position = new Vector2(panelRect.position.x, offscreenTop);
        panelRect.gameObject.SetActive(true);

        panelRect.DOAnchorPosY(visibleTop, duration)
                 .SetEase(showEase)
                 .SetTarget(panelRect);
    }

 
    public void SlideOut()
    {
        if (panelRect == null) return;

        panelRect.DOKill();

        Vector2 position = panelRect.position;
        if (position.y != visibleTop) {
            panelRect.position = new Vector2(position.x, visibleTop);
        }
        

        panelRect.DOAnchorPosY(offscreenTop, duration)
                 .SetEase(hideEase)
                 .SetTarget(panelRect)
                 .OnComplete(() =>
                 {
                     panelRect.gameObject.SetActive(false);
                     panelRect.position = new Vector2(position.x, visibleTop);
                 });
    }

    private void OnDisable()
    {
        if (panelRect != null)
            panelRect.DOKill();
    }
}
