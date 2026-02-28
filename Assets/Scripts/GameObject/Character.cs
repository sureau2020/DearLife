using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;


public class Character : MonoBehaviour
{
    [SerializeField] private GameObject characterUI;
    [SerializeField] private CameraFocus cameraFocus;
    //[SerializeField] private CharacterMoveAI characterMoveAI;
    [SerializeField] private CharacterTileMoveAI characterTileMoveAI;
    [SerializeField] private CharacterTouchAnimation touchAnimation;
    [SerializeField] private Closet closet;
    [SerializeField] private GameObject clickEffectPrefab;
    public static Action recordOpenEyes;

    private readonly List<RaycastResult> _uiRaycastResults = new List<RaycastResult>();
    private PointerEventData _tempPointerData;
    private bool _isStartedOnUI = false;

    private float z = 0.3f;

    private bool touchHandled = false;
    private float lastToggleTime = -999f;
    private const float ToggleCooldown = 0.2f;

    private Vector2 touchStartPos;
    private float touchStartTime;
    private int activeFingerId = -1;
    private bool isDragging = false;
    private float dragThresholdPixels = 20f;  
    private const float TapMaxDuration = 0.3f;
    private Plane xzPlane = new Plane(Vector3.up, Vector3.zero);

    private bool isFurnishing = false;


    void Awake()
    {
        Input.simulateMouseWithTouches = false;

        if (Screen.dpi > 0)
        {
            dragThresholdPixels = Mathf.Max(20f, Screen.dpi * 0.15f);
        }
        FurnishManager.onEnterFurnishMode += (bool mode) => isFurnishing = mode;
    }

    void OnDestroy()
    {
        FurnishManager.onEnterFurnishMode -= (bool mode) => isFurnishing = mode;
    }

    void Update()
    {

#if UNITY_ANDROID || UNITY_IOS
        if (!closet.isActiveAndEnabled && !isFurnishing)
        {
            CheckTouch();
        }
        #else
        if (!closet.isActiveAndEnabled && !isFurnishing) {
            CheckClick();
            //        CheckTouch();
        }

#endif
    }

    private void CheckClick()
    {
        if (Application.isMobilePlatform || Input.touchSupported)
            return;

        if (Input.GetMouseButtonUp(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return; 
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    ChangeShowingOfCharacterUI();
                    return;
                }
            }
          
            // 3. 计算射线与平面的交点
            float distance; // 射线到平面的距离
            if (xzPlane.Raycast(ray, out distance))
            {
                Vector3 hitPoint = ray.GetPoint(distance);

                if (characterTileMoveAI != null)
                {
                    if (characterTileMoveAI.IsWalkable(hitPoint))
                    {
                        SoundManager.Instance.PlaySfx("Pop");
                        Vector3 pos = Input.mousePosition;
                        pos.z = this.z;

                        GameObject effect = Instantiate(clickEffectPrefab, hitPoint, Quaternion.identity);

                        Destroy(effect, 1f);

                        characterTileMoveAI.MoveToPosition(hitPoint);
                    }
                }
            }

            

            //    if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Floor")))
            //{
            //    SoundManager.Instance.PlaySfx("Pop");
            //    Vector3 pos = Input.mousePosition;
            //    pos.z = this.z; 

            //    Vector3 worldPos = Camera.main.ScreenToWorldPoint(pos);

            //    GameObject effect = Instantiate(clickEffectPrefab, worldPos, Quaternion.identity);

            //    Destroy(effect, 1f);
            //    if (characterTileMoveAI != null)
            //        characterTileMoveAI.MoveToPosition(hit.point);
            //}
        }
    }

    private void CheckTouch()
    {
        if (Input.touchCount <= 0)
        {
            activeFingerId = -1;
            touchHandled = false;
            isDragging = false;
            _isStartedOnUI = false;
            return;
        }

        Touch touch = Input.GetTouch(0);

        // 1. 手指按下阶段
        if (touch.phase == TouchPhase.Began)
        {
            activeFingerId = touch.fingerId;
            touchHandled = false;
            isDragging = false;
            touchStartPos = touch.position;
            touchStartTime = Time.unscaledTime;

            // 【关键优化】按下时就检测 UI 遮挡
            _isStartedOnUI = IsPointerOverUIStrict(touch.position);
        }
        // 2. 手指移动/保持阶段
        else if (touch.fingerId == activeFingerId)
        {
            // 如果按下时就在 UI 上，后续逻辑直接拦截
            if (_isStartedOnUI) return;

            if (touch.phase == TouchPhase.Moved && !isDragging)
            {
                if (Vector2.Distance(touchStartPos, touch.position) > dragThresholdPixels)
                {
                    isDragging = true;
                }
            }
            // 3. 手指抬起阶段
            else if (touch.phase == TouchPhase.Ended && !touchHandled)
            {
                touchHandled = true;

                // 判定是否为合法点击（非长距离拖拽，非长按）
                bool isTap = !isDragging &&
                             (Time.unscaledTime - touchStartTime <= TapMaxDuration) &&
                             (Vector2.Distance(touchStartPos, touch.position) <= dragThresholdPixels);

                if (isTap)
                {
                    HandleTapLogic(touch.position);
                }

                activeFingerId = -1;
                _isStartedOnUI = false;
            }
        }
    }

    // 将具体业务逻辑抽离，保持代码整洁
    private void HandleTapLogic(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        // 优先检测角色点击
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                ChangeShowingOfCharacterUI();
                return;
            }
        }

        // 地面移动检测
        if (xzPlane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            if (characterTileMoveAI != null && characterTileMoveAI.IsWalkable(hitPoint))
            {
                SoundManager.Instance.PlaySfx("Pop");

                // 生成特效
                GameObject effect = Instantiate(clickEffectPrefab, hitPoint, Quaternion.identity);
                Destroy(effect, 1f);

                characterTileMoveAI.MoveToPosition(hitPoint);
            }
        }
    }

    // 优化后的射线检测，零 GC
    private bool IsPointerOverUIStrict(Vector2 screenPosition)
    {
        if (EventSystem.current == null) return false;

        if (_tempPointerData == null) _tempPointerData = new PointerEventData(EventSystem.current);
        _tempPointerData.position = screenPosition;

        _uiRaycastResults.Clear();
        EventSystem.current.RaycastAll(_tempPointerData, _uiRaycastResults);
        return _uiRaycastResults.Count > 0;
    }

    public void ChangeShowingOfCharacterUI()
    {
        if (Time.unscaledTime - lastToggleTime < ToggleCooldown)
            return;
        lastToggleTime = Time.unscaledTime;

        SoundManager.Instance.PlaySfx("Type");
        if (!characterUI.activeSelf)
        {
            OpenCharacterUI();
        }
        else
        {
            TouchCharacter();
        }
    }

    private void OpenCharacterUI()
    {
        recordOpenEyes?.Invoke();
        characterUI.SetActive(true);
        cameraFocus.FocusOnTarget();
    }

    public void TouchCharacter() { 
        touchAnimation.ShowTouchAnimation();
    }

    public void CloseCharacterUI()
    {
        characterUI.SetActive(false);
        cameraFocus.ResetCamera();
    }
}