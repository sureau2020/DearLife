using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [Header("��������")]
    public float zoomSpeed = 2f;        // �����ٶ�
    public float minZoom = 2f;          // ��С����
    public float maxZoom = 7f;         // �������

    [Header("�϶�����")]
    public float panSpeed = 10f;        // �϶��ٶ�
    public Vector2 panLimitMin = new Vector2(0, 6); // ������ƶ�����С��Χ
    public Vector2 panLimitMax = new Vector2(3, 8);   // ������ƶ������Χ

    private Camera cam;
    private Vector3 dragOrigin;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        HandleZoom();
        HandlePan();
    }

    bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

#if UNITY_EDITOR || UNITY_STANDALONE
        return EventSystem.current.IsPointerOverGameObject();
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }
        return false;
#else
        return false;
#endif
    }

    void HandleZoom()
    {
        if (IsPointerOverUI()) return;

        // ����������
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
        }

        // �ֻ�˫ָ����
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 prev0 = t0.position - t0.deltaPosition;
            Vector2 prev1 = t1.position - t1.deltaPosition;

            float prevDist = (prev0 - prev1).magnitude;
            float curDist = (t0.position - t1.position).magnitude;

            float delta = curDist - prevDist;
            cam.orthographicSize -= delta * zoomSpeed * 0.01f;
        }

        // �������ŷ�Χ
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }

    void HandlePan()
    {
        if (IsPointerOverUI()) return;

#if UNITY_EDITOR || UNITY_STANDALONE
        // ����϶�
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 diff = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            diff.x *= cam.aspect;
            Vector3 move = new Vector3(-diff.x * panSpeed, -diff.y * panSpeed, 0);

            transform.Translate(move, Space.World);

            dragOrigin = Input.mousePosition;
        }
#elif UNITY_ANDROID || UNITY_IOS
        // ��ָ��ָ�϶�
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                dragOrigin = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector3 diff = Camera.main.ScreenToViewportPoint(touch.position - (Vector3)dragOrigin);
                diff.x *= cam.aspect;
                Vector3 move = new Vector3(-diff.x * panSpeed, -diff.y * panSpeed, 0);

                transform.Translate(move, Space.World);

                dragOrigin = touch.position;
            }
        }
#endif

        // ���������Χ
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, panLimitMin.x, panLimitMax.x);
        pos.y = Mathf.Clamp(pos.y, panLimitMin.y, panLimitMax.y);
        transform.position = pos;
    }
}
