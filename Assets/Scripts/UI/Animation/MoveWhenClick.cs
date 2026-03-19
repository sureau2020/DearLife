using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MoveWhenClick : MonoBehaviour
{
    // 将变量名改为复数，明确这是一个列表
    public List<MoveWhenClickData> animationDatas = new List<MoveWhenClickData>();
    public Button Button;
    private bool isOrigin = true;

    void Start()
    {
        FurnishManager.onEnterFurnishMode += OnEnterFurnishMode;
        Closet.onClosetToggled += OnEnterFurnishMode; 
    }

    void OnDestroy()
    {
        FurnishManager.onEnterFurnishMode -= OnEnterFurnishMode;
        Closet.onClosetToggled -= OnEnterFurnishMode;
    }

    private void OnEnterFurnishMode(bool isInFurnishMode)
    {
        if (isInFurnishMode)
        {
            Button.interactable = false;
        }
        else
        {
            Button.interactable = true;
        }
    }

    public void Move()
    {
        // 每次执行前，先清理掉已经不存在的无效引用（防止 Inspector 里残留 Missing 对象）
        animationDatas.RemoveAll(d => d == null || d.target == null);

        if (isOrigin)
        {
            MoveToEnd();
        }
        else
        {
            MoveToOrigin();
        }
    }

    private void MoveToEnd()
    {
        isOrigin = false;
        foreach (var data in animationDatas)
        {
            // 1. 停止该物体上正在播放的同类动画，防止叠加冲突
            data.target.DOKill();

            // 2. 设置初始位置
            data.target.anchoredPosition = data.originalPosition;

            // 3. 播放动画并绑定生命周期
            data.target.DOAnchorPos(data.endPosition, data.duration)
                       .SetEase(Ease.OutQuad) // 建议加上缓动效果，更丝滑
                       .SetLink(data.target.gameObject); // 核心：物体销毁时自动销毁动画
        }
    }

    private void MoveToOrigin()
    {
        isOrigin = true;
        foreach (var data in animationDatas)
        {
            data.target.DOKill();

            data.target.anchoredPosition = data.endPosition;

            data.target.DOAnchorPos(data.originalPosition, data.duration)
                       .SetEase(Ease.OutQuad)
                       .SetLink(data.target.gameObject);
        }
    }
}

[System.Serializable]
public class MoveWhenClickData
{
    public RectTransform target;
    public Vector2 originalPosition;
    public Vector2 endPosition;
    public float duration = 0.5f; // 给个默认时间
}