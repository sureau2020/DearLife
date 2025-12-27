using UnityEngine;
using DG.Tweening;

public class CharacterTouchAnimation : MonoBehaviour
{
    [SerializeField] private CharacterBreatheComponent breatheComponent;

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
    private Quaternion originRotation;

    private void Awake()
    {
        originScale = transform.localScale;
        originRotation = transform.localRotation;
    }

    public void ShowTouchAnimation()
    {
        transform.DOKill();


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
            if (breatheComponent != null)
                breatheComponent.SetBreathing(true);
        });
    }
}
