using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationWhenEnable : MonoBehaviour
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

    void OnEnable()
    {
        if (panelRect == null) return;

        panelRect.DOKill();

        if (!isXdirection)
        {
            panelRect.position = new Vector2(panelRect.position.x, originalPosition);
            panelRect.gameObject.SetActive(true);
            panelRect.DOAnchorPosY(endPostion, duration)
                     .SetEase(showEase)
                     .SetTarget(panelRect);
        }
        else {
            panelRect.position = new Vector2(originalPosition, panelRect.position.y);
            panelRect.gameObject.SetActive(true);
            panelRect.DOAnchorPosX(endPostion, duration)
                     .SetEase(showEase)
                     .SetTarget(panelRect);
        }
        
    }


}
