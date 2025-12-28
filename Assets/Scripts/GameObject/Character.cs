using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;


public class Character : MonoBehaviour
{
    [SerializeField] private GameObject characterUI;
    [SerializeField] private CameraFocus cameraFocus;
    [SerializeField] private CharacterMoveAI characterMoveAI;
    [SerializeField] private CharacterTouchAnimation touchAnimation;
    [SerializeField] private Closet closet;
    [SerializeField] private GameObject clickEffectPrefab;
    public static Action recordOpenEyes;

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


    void Awake()
    {
        Input.simulateMouseWithTouches = false;

        if (Screen.dpi > 0)
        {
            dragThresholdPixels = Mathf.Max(20f, Screen.dpi * 0.15f);
        }
    }

    void Update()
    {

#if UNITY_ANDROID || UNITY_IOS
        if (!closet.isActiveAndEnabled)
        {
            CheckTouch();
        }
#else
        if (!closet.isActiveAndEnabled) {
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
            if (IsPointerOverUIStrict(Input.mousePosition))
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    ChangeShowingOfCharacterUI();
                    return;
                }
            }

            if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Floor")))
            {
                Vector3 pos = Input.mousePosition;
                pos.z = this.z; 

                Vector3 worldPos = Camera.main.ScreenToWorldPoint(pos);

                GameObject effect = Instantiate(clickEffectPrefab, worldPos, Quaternion.identity);

                Destroy(effect, 1f);
                if (characterMoveAI != null)
                    characterMoveAI.MoveToIfValid(hit.point);
            }
        }
    }

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
                    if (Vector2.Distance(touchStartPos, touch.position) > dragThresholdPixels)
                    {
                        isDragging = true; 
                    }
                }
                else if (touch.phase == TouchPhase.Ended && !touchHandled)
                {
                    touchHandled = true;

                    if (IsPointerOverUIStrict(touch.position))
                    {
                        activeFingerId = -1;
                        return;
                    }

                    bool isTap = !isDragging &&
                                 (Time.unscaledTime - touchStartTime <= TapMaxDuration) &&
                                 (Vector2.Distance(touchStartPos, touch.position) <= dragThresholdPixels);

                    if (!isTap)
                    {
                        activeFingerId = -1;
                        return;
                    }

                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        if (hit.transform == transform)
                        {
                            ChangeShowingOfCharacterUI();
                            return; 
                        }
                    }

                    if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Floor")))
                    {
                        Vector3 pos = Input.mousePosition;
                        pos.z = this.z;
                        Vector3 worldPos = Camera.main.ScreenToWorldPoint(pos);
                        GameObject effect = Instantiate(clickEffectPrefab, worldPos, Quaternion.identity);
                        Destroy(effect, 1f);

                        if (characterMoveAI != null)
                            characterMoveAI.MoveToIfValid(hit.point);
                    }

                    activeFingerId = -1;
                }
            }
        }
        else
        {
            activeFingerId = -1;
            touchHandled = false;
            isDragging = false;
        }
    }

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