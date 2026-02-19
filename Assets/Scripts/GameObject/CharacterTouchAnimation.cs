using UnityEngine;
using DG.Tweening;
using System;

public class CharacterTouchAnimation : MonoBehaviour
{
    [SerializeField] private CharacterBreatheComponent breatheComponent;
    [SerializeField] private SpriteRenderer leftEyeRenderer;
    [SerializeField] private SpriteRenderer rightEyeRenderer;
    [SerializeField] private SpriteRenderer leftEyeBlancRenderer;
    [SerializeField] private SpriteRenderer rightEyeBlancRenderer;

    private Sprite leftCloseEye;
    private Sprite rightCloseEye;
    private Sprite leftOpenEye;
    private Sprite rightOpenEye;
    private bool leftEyeBlancVisible = false;
    private bool rightEyeBlancVisible = false;

    private float squashYScale = 0.8f;   
    private float stretchXScale = 1.1f; 
    private float squashDuration = 0.5f;
    private Ease squashEase = Ease.OutQuad;   
    private float rotateAngle = 15f;
    private float rotateDuration = 0.1f;
    private int rotateLoops = 8;
    private float recoverDuration = 0.5f;
    private Ease recoverEase = Ease.OutBack;
    private float rotateRecoverDuration = 0.1f;

    private Vector3 originScale;
    private Quaternion originRotation = new Quaternion(0, 0, 0, 0);

    void Awake()
    {
        originScale = transform.localScale;
        //originRotation = transform.localRotation;
        Character.recordOpenEyes += RecordEye;
    }

    void OnDestroy()
    {
        Character.recordOpenEyes -= RecordEye;
    }

    void Start()
    {
        leftCloseEye = AppearanceAtlasManager.Instance.GetPartSprite("LeftEye", 5);
        rightCloseEye = AppearanceAtlasManager.Instance.GetPartSprite("RightEye", 5);
    }

    public void RecordEye()
    {
        if (leftEyeRenderer != null)
            leftOpenEye = leftEyeRenderer.sprite;
        if (rightEyeRenderer != null)
            rightOpenEye = rightEyeRenderer.sprite;
        if (rightEyeBlancRenderer != null && rightEyeBlancRenderer.enabled) {
            rightEyeBlancVisible = true;
        }else {
            rightEyeBlancVisible = false;
        }
        if (leftEyeBlancRenderer != null && leftEyeBlancRenderer.enabled)
        {
            leftEyeBlancVisible = true;
        }
        else
        {
            leftEyeBlancVisible = false;
        }
    }


    public void ShowTouchAnimation()
    {
        transform.DOKill();

        SoundManager.Instance.PlaySfx("ShortShake");
        CloseEye();

        if (transform.localScale.x > 0)
        {
            originScale.x = Mathf.Abs(originScale.x);
        }
        else
        { 
            originScale.x = -Mathf.Abs(originScale.x);
        }
        transform.localScale = originScale;
        transform.localRotation = originRotation;

        if (breatheComponent != null)
            breatheComponent.SetBreathing(false);

        Vector3 squashScale = new Vector3(
            originScale.x * stretchXScale,
            originScale.y * squashYScale,
            originScale.z
        );

        Sequence seq = DOTween.Sequence();

        // 1. 挤压
        seq.Append(
            transform.DOScale(squashScale, squashDuration)
                .SetEase(squashEase)
        );

        // 2. 左右摇（+angle ↔ -angle）
        seq.Join(
            transform.DORotate(
                new Vector3(0, 0, rotateAngle),
                rotateDuration
            )
            .From(new Vector3(0, 0, -rotateAngle))
            .SetLoops(rotateLoops, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
        );
        // 3. 回弹
        seq.Append(
            transform.DOScale(originScale, recoverDuration)
                .SetEase(recoverEase)
        );

        // 4. 旋转回正
        seq.Join(
            transform.DORotateQuaternion(originRotation, rotateRecoverDuration)
        );

        seq.OnComplete(() =>
        {
            OpenEye();
            if (breatheComponent != null)
                breatheComponent.SetBreathing(true);
        });
    }

    private void CloseEye()
    {
        if (leftEyeRenderer != null)
            leftEyeRenderer.sprite = leftCloseEye;
        if (rightEyeRenderer != null)
            rightEyeRenderer.sprite = rightCloseEye;
        if (leftEyeBlancRenderer != null)
            leftEyeBlancRenderer.enabled = false;
        if (rightEyeBlancRenderer != null)
            rightEyeBlancRenderer.enabled = false;
    }

    private void OpenEye()
    {
        if (leftEyeRenderer != null)
            leftEyeRenderer.sprite = leftOpenEye;
        if (rightEyeRenderer != null)
            rightEyeRenderer.sprite = rightOpenEye;
        if (leftEyeBlancVisible)
            leftEyeBlancRenderer.enabled = true;
        if (rightEyeBlancVisible)
            rightEyeBlancRenderer.enabled = true;

    }
}
