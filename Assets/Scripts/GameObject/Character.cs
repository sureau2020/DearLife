using UnityEngine;
using UnityEngine.EventSystems; 

// ������Ļ����ʾ�Ľ�ɫ��������ʾ������߼�
public class Character : MonoBehaviour
{
    [SerializeField] private GameObject characterUI; 
    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 0.5f; // ˫����������룩



    void Update()
    {
        CheckDoubleClick();
        CheckDoubleTouch();
    }


    //�ֻ���˫�����������������ر������
    private void CheckDoubleTouch()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended)
            {
                // ����Ƿ����UI��
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
            // ����Ƿ����UI��
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
