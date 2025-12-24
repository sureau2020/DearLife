using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationWhenDisable : MonoBehaviour
{
    [SerializeField] private RectTransform panelRect;

    [SerializeField] private bool isXdirection = true;
    [SerializeField] private float originalPosition = 0f;
    [SerializeField] private float endPostion = 0f;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease showEase = Ease.OutCubic;
    [SerializeField] private Ease hideEase = Ease.OutCubic;

    private void Awake()
    {
        if (panelRect == null)
            panelRect = GetComponent<RectTransform>();
    }

    public void HideAndDisable()
    {
        if (panelRect == null)
        {
            gameObject.SetActive(false);
            return;
        }

        panelRect.DOKill();

        if (isXdirection)
        {
            Vector2 start = panelRect.anchoredPosition;
            panelRect.anchoredPosition = new Vector2(start.x, start.y);

            panelRect.DOAnchorPosX(endPostion, duration)
                     .SetEase(hideEase)
                     .SetTarget(panelRect)
                     .OnComplete(() =>
                     {
                         panelRect.gameObject.SetActive(false);
                     });
        }
        else
        {
            Vector2 start = panelRect.anchoredPosition;
            panelRect.anchoredPosition = new Vector2(start.x, start.y);

            panelRect.DOAnchorPosY(endPostion, duration)
                     .SetEase(hideEase)
                     .SetTarget(panelRect)
                     .OnComplete(() =>
                     {
                         panelRect.gameObject.SetActive(false);
                     });
        }
    }


    private void OnDisable()
    {
        if (panelRect != null)
            panelRect.DOKill();
    }
}
