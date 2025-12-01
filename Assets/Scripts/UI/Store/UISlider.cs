using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UISlider : MonoBehaviour
{
    public float animationDuration = 0.3f;
    public float slideDistance = 1200f;
    
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Vector2 offscreenPosition;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
        originalPosition = rectTransform.anchoredPosition;
        
        offscreenPosition = new Vector2(slideDistance, originalPosition.y);
    }
    
    private void OnEnable()
    {
        PlaySlideInAnimation();
    }
    
    public void PlaySlideInAnimation()
    {
        rectTransform.anchoredPosition = offscreenPosition;
        
        rectTransform.DOAnchorPos(originalPosition, animationDuration)
            .SetEase(Ease.OutBack);
    }
    
    /// <summary>
    /// 播放滑出动画并隐藏对象
    /// </summary>
    public void PlaySlideOutAnimation()
    {
        // 先稍微向左移动一点，然后向右滑出
        rectTransform.DOAnchorPosX(originalPosition.x - 50f, animationDuration * 0.2f)
            .SetEase(Ease.InQuad)
            .OnComplete(() => {
                // 然后滑出到右侧
                rectTransform.DOAnchorPos(offscreenPosition, animationDuration * 0.8f)
                    .SetEase(Ease.InBack)
                    .OnComplete(() => {
                        gameObject.SetActive(false);
                    });
            });
    }
    
    /// <summary>
    /// 直接关闭UI（无动画）
    /// </summary>
    public void CloseImmediate()
    {
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 显示UI（带动画）
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
    }
    
    /// <summary>
    /// 隐藏UI（带动画）
    /// </summary>
    public void Hide()
    {
        PlaySlideOutAnimation();
    }
    
    private void OnDisable()
    {
        // 停止所有DOTween动画，避免内存泄漏
        rectTransform.DOKill();
    }
}
