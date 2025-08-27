using System.Collections;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{
    private Camera cam;           
    [SerializeField] private Transform target;     
    public Vector3 offset = new Vector3(0, 0, -3.5f);
    public float moveSpeed = 5f;    
    public float zoomSpeed = 3f;    
    public float targetSize = 2f;   
    void Awake()
    {
        cam = GetComponent<Camera>(); 
    }

    // 按钮点击调用
    public void FocusOnTarget()
    {
        if (target != null && cam != null)
            StartCoroutine(FocusCoroutine());
    }

    IEnumerator FocusCoroutine()
    {
        Vector3 startPos = cam.transform.position;
        Vector3 endPos = target.position + offset;
        float startSize = cam.orthographicSize;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            cam.transform.position = Vector3.Lerp(startPos, endPos, t);
            cam.orthographicSize = Mathf.Lerp(startSize, targetSize, t * zoomSpeed);

            yield return null;
        }

        // 最后矫正一下
        cam.transform.position = endPos;
        cam.orthographicSize = targetSize;
    }
}

