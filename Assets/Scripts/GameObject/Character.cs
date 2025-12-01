using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

// 代表屏幕上显示的角色，处理显示的相关逻辑
public class Character : MonoBehaviour
{
    [SerializeField] private GameObject characterUI;
    [SerializeField] private CameraFocus cameraFocus;
    [SerializeField] private CharacterMoveAI characterMoveAI;
    [SerializeField] private Closet closet;
    [SerializeField] private GameObject clickEffectPrefab;

    private float z=0.3f; 

    private bool touchHandled = false; 
    private float lastToggleTime = -999f;
    private const float ToggleCooldown = 0.2f;

    // 手势判定
    private Vector2 touchStartPos;
    private float touchStartTime;
    private int activeFingerId = -1;
    private bool isDragging = false;
    private float dragThresholdPixels = 20f;   // 根据dpi在Awake里自适应
    private const float TapMaxDuration = 0.3f; // 最长点击时长

    void Awake()
    {
        // 禁止触摸模拟鼠标，避免手机一次触摸触发鼠标事件
        Input.simulateMouseWithTouches = false;

        // 自适应像素阈值（约0.15英寸），低dpi时至少20px
        if (Screen.dpi > 0)
        {
            dragThresholdPixels = Mathf.Max(20f, Screen.dpi * 0.15f);
        }
    }

    void Update() {

#if UNITY_ANDROID || UNITY_IOS
        if (!closet.isActiveAndEnabled)
        {
            CheckTouch();
        }
#else
        //桌面端：处理鼠标与触摸
        if (!closet.isActiveAndEnabled) {
            CheckClick();
            //        CheckTouch();
        }

#endif
    }

    private void CheckClick()
    {
        // 如果当前设备支持触摸，则不走鼠标路径，防止重复
        if (Application.isMobilePlatform || Input.touchSupported)
            return;

        if (Input.GetMouseButtonUp(0))
        {
            // 严格的 UI 命中检测：有任何 UI 命中就返回
            if (IsPointerOverUIStrict(Input.mousePosition))
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // 1. 先检测角色本身
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    ChangeShowingOfCharacterUI();
                    return; // 已处理点击角色，直接返回
                }
            }

            // 2. 再检测地面
            if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Floor")))
            {
                Vector3 pos = Input.mousePosition;
                pos.z = this.z; // 相机到角色的距离
                
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(pos);

                // 生成特效
                GameObject effect = Instantiate(clickEffectPrefab, worldPos,Quaternion.identity);

                // 自动销毁（粒子播放完就消失）
                Destroy(effect, 1f);
                // 点击到地面，尝试让角色移动
                if (characterMoveAI != null)
                    characterMoveAI.MoveToIfValid(hit.point);
            }
        }
    }

    // 触屏单击（带手势区分）
    private void CheckTouch()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                activeFingerId = touch.fingerId;
                touchHandled = false;
                isDragging = false;
                touchStartPos = touch.position;
                touchStartTime = Time.unscaledTime;
            }
            else if (touch.fingerId == activeFingerId)
            {
                if (touch.phase == TouchPhase.Moved && !isDragging)
                {
                    // 超过阈值判定为拖动
                    if (Vector2.Distance(touchStartPos, touch.position) > dragThresholdPixels)
                    {
                        isDragging = true; // 标记为滑动，交给相机去处理
                    }
                }
                else if (touch.phase == TouchPhase.Ended && !touchHandled)
                {
                    touchHandled = true;

                    // 严格的 UI 命中检测：有任何 UI 命中就返回（不允许穿透）
                    if (IsPointerOverUIStrict(touch.position))
                    {
                        activeFingerId = -1;
                        return;
                    }

                    // 只有短时且位移小才算点击
                    bool isTap = !isDragging &&
                                 (Time.unscaledTime - touchStartTime <= TapMaxDuration) &&
                                 (Vector2.Distance(touchStartPos, touch.position) <= dragThresholdPixels);

                    if (!isTap)
                    {
                        activeFingerId = -1;
                        return; // 滑动/长按一律不触发点击
                    }

                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        if (hit.transform == transform)
                        {
                            ChangeShowingOfCharacterUI();
                            return; // 已处理点击角色，直接返回
                        }
                    }

                    // 2. 再检测地面
                    if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Floor")))
                    {
                        Vector3 pos = Input.mousePosition;
                        pos.z = this.z; 
                        Vector3 worldPos = Camera.main.ScreenToWorldPoint(pos);
                        GameObject effect = Instantiate(clickEffectPrefab, worldPos, Quaternion.identity);
                        Destroy(effect, 1f);

                        // 点击到地面，尝试让角色移动
                        if (characterMoveAI != null)
                            characterMoveAI.MoveToIfValid(hit.point);
                    }

                    activeFingerId = -1;
                }
            }
        }
        else
        {
            // 非单指，重置状态
            activeFingerId = -1;
            touchHandled = false;
            isDragging = false;
        }
    }

    // 使用 UI 图形射线，严格判定“是否被任意UI遮挡"
    private bool IsPointerOverUIStrict(Vector2 screenPosition)
    {
        if (EventSystem.current == null) return false;

        var eventData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition
        };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    public void ChangeShowingOfCharacterUI()
    {
        // 去抖，避免极短时间内重复触发
        if (Time.unscaledTime - lastToggleTime < ToggleCooldown)
            return;
        lastToggleTime = Time.unscaledTime;

        SoundManager.Instance.PlaySfx("Type");
        if (!characterUI.activeSelf)
        {
            characterUI.SetActive(true);
            cameraFocus.FocusOnTarget();
        }
        else
        {
            characterUI.SetActive(false);
            cameraFocus.ResetCamera();
        }
    }


}
