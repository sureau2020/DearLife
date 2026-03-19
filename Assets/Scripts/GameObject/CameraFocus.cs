using System.Collections;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{
    private Camera cam;
    private Coroutine currentRoutine;
    //[SerializeField] private CharacterMoveAI characterMoveAI;
    [SerializeField] private CharacterTileMoveAI characterTileMoveAI;

    [Header("聚焦参数")]
    [SerializeField] private Transform target;
    public Vector3 offset = new Vector3(0, 0, -4f);
    public float moveSpeed = 5f;
    public float zoomSpeed = 3f;
    public float targetSize = 2f;

    [Header("初始位置参数")]
    private Vector3 initialPos;
    private float initialSize;

    void Awake()
    {
        cam = GetComponent<Camera>();
        initialPos = cam.transform.position;
        initialSize = cam.orthographicSize;
    }

    public void FocusOnTarget()
    {
        if (target == null || cam == null) return;

        // 停掉之前的动画，避免跳动
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        characterTileMoveAI.SetFocus(true);
        Vector3 endPos = target.position + offset;
        currentRoutine = StartCoroutine(MoveCamera(endPos, targetSize));
    }

    public void FocusOnTarget(Vector3 worldPosition)
    {
        if (cam == null) return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        characterTileMoveAI.SetFocus(true);
        Vector3 endPos = worldPosition + offset;
        currentRoutine = StartCoroutine(MoveCamera(endPos, targetSize));
    }

    public void ResetCamera()
    {
        if (cam == null) return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        characterTileMoveAI.SetFocus(false);
        currentRoutine = StartCoroutine(MoveCamera(initialPos, initialSize));
    }

    IEnumerator MoveCamera(Vector3 endPos, float endSize)
    {
        Vector3 startPos = cam.transform.position;
        float startSize = cam.orthographicSize;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            cam.transform.position = Vector3.Lerp(startPos, endPos, t);
            cam.orthographicSize = Mathf.Lerp(startSize, endSize, t);
            yield return null;
        }

        cam.transform.position = endPos;
        cam.orthographicSize = endSize;
        currentRoutine = null;
    }


}
