using UnityEngine;
using UnityEngine.EventSystems; 

// 代表屏幕上显示的角色，处理显示的相关逻辑
public class Character : MonoBehaviour
{
    [SerializeField] private GameObject characterUI; 
    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 0.5f; // 双击最大间隔（秒）



    void Update()
    {
        CheckDoubleClick();
        CheckDoubleTouch();
    }


    //手机，双击调出背包面板或隐藏背包面板
    private void CheckDoubleTouch()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended)
            {
                // 检查是否点在UI上
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    return;

                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform == transform)
                    {
                        float currentTime = Time.time;
                        if (currentTime - lastClickTime <= doubleClickThreshold)
                        {
                            ChangeShowingOfCharacterUI();
                            lastClickTime = 0f;
                        }
                        else
                        {
                            lastClickTime = currentTime;
                        }
                    }
                }
            }
        }
    }

    private void CheckDoubleClick()
    {
        if (Input.GetMouseButtonUp(0))
        {
            // 检查是否点在UI上
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    float currentTime = Time.time;
                    if (currentTime - lastClickTime <= doubleClickThreshold)
                    {
                        ChangeShowingOfCharacterUI();
                        lastClickTime = 0f;
                    }
                    else
                    {
                        lastClickTime = currentTime;
                    }
                }
            }
        }
    }

    private void ChangeShowingOfCharacterUI()
    {
        if (characterUI != null)
            characterUI.SetActive(!characterUI.activeSelf);
    }
}
