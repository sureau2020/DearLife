using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [Header("缩放设置")]
    public float zoomSpeed = 2f;        // 缩放速度
    public float minZoom = 2f;          // 最小缩放
    public float maxZoom = 7f;         // 最大缩放

    [Header("拖动设置")]
    public float panSpeed = 10f;        // 拖动速度
    public Vector2 panLimitMin = new Vector2(0, 6); // 相机可移动的最小范围
    public Vector2 panLimitMax = new Vector2(3, 8);   // 相机可移动的最大范围

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

        // 鼠标滚轮缩放
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
        }

        // 手机双指缩放
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

        // 限制缩放范围
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }

    void HandlePan()
    {
        if (IsPointerOverUI()) return;

#if UNITY_EDITOR || UNITY_STANDALONE
        // 鼠标拖动
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
        // 手指单指拖动
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

        // 限制相机范围
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, panLimitMin.x, panLimitMax.x);
        pos.y = Mathf.Clamp(pos.y, panLimitMin.y, panLimitMax.y);
        transform.position = pos;
    }
}
