using UnityEngine;
using UnityEngine.EventSystems;

// ������Ļ����ʾ�Ľ�ɫ��������ʾ������߼�
public class Character : MonoBehaviour
{
    [SerializeField] private GameObject characterUI;
    [SerializeField] private CameraFocus cameraFocus;

    void Update()
    {
        CheckClick();
        CheckTouch();
    }

    // ��굥��
    private void CheckClick()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    ChangeShowingOfCharacterUI();
                }
            }
        }
    }

    // ��������
    private void CheckTouch()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended)
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    return;

                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform == transform)
                    {
                        ChangeShowingOfCharacterUI();
                    }
                }
            }
        }
    }

    
    private void ChangeShowingOfCharacterUI()
    {
        characterUI.SetActive(!characterUI.activeSelf);
        if (characterUI.activeSelf) {
            cameraFocus.FocusOnTarget();
        }
    }
}
