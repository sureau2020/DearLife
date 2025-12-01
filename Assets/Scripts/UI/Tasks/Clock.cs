using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TimeDial : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform dialCenter;
    [SerializeField] private RectTransform hourHand;
    [SerializeField] private RectTransform minuteHand;
    [SerializeField] private Image hourDial;
    [SerializeField] private Image minuteDial;
    [SerializeField] private TMP_InputField hourText;
    [SerializeField] private TMP_InputField minuteText;
    [SerializeField] private TextMeshProUGUI pm;

    public float innerRadiusScale = 0.2f;
    public float outerRadiusScale = 0.3f;

    public float innerRadius = 62f;   // 小时圈半径
    public float outerRadius = 100f;  // 分钟环半径

    private bool dragging = false;
    private float currentAngle = 0f;
    private bool editingHour = true; // true=小时，false=分钟

    private int hour = 0;
    private int minute = 0;
    private bool isPM = false;

    void Awake()
    {
        // 基于表盘图片的实际大小设置半径
        float dialSize = Mathf.Min(hourDial.rectTransform.rect.width, hourDial.rectTransform.rect.height);
        float minuteDialSize = Mathf.Min(minuteDial.rectTransform.rect.width, minuteDial.rectTransform.rect.height);

        innerRadius = dialSize * innerRadiusScale;
        outerRadius = minuteDialSize * outerRadiusScale;

    }

    void OnEnable()
    { 

        DateTime now = DateTime.Now;
        hour = now.Hour % 12;
        minute = now.Minute;
        hourHand.rotation = Quaternion.Euler(0, 0, -hour * 30f);
        minuteHand.rotation = Quaternion.Euler(0, 0, -minute * 6f);
        currentAngle = hour * 30f;
        editingHour = true;
        isPM = now.Hour >= 12;
        pm.text = isPM ? "下午" : "上午";
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 检测点击到哪个表盘
        if (IsPointerOnOpaquePixel(hourDial, eventData.position))
        {
            editingHour = true;
            dragging = true;
            UpdatePointer(eventData.position);
        }
        else if (IsPointerOnOpaquePixel(minuteDial, eventData.position))
        {
            editingHour = false;
            dragging = true;
            UpdatePointer(eventData.position);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragging)
        {
            UpdatePointer(eventData.position);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
        SnapToNearest();
    }

    void UpdatePointer(Vector2 pointerPos)
    {
        // 计算角度
        Vector2 dir = pointerPos - (Vector2)dialCenter.position;
        float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;
        
        currentAngle = angle;
        ApplyRotation();
    }

    private bool IsPointerOnOpaquePixel(Image img, Vector2 screenPos)
    {
        if (img == null || img.sprite == null || img.sprite.texture == null)
            return false;

        // 1. 屏幕坐标转本地坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            img.rectTransform, screenPos, null, out Vector2 localPos);

        Rect rect = img.rectTransform.rect;
        // 2. 检查是否在图片范围内
        if (!rect.Contains(localPos))
            return false;

        // 3. 本地坐标转归一化坐标
        float xNorm = (localPos.x - rect.x) / rect.width;
        float yNorm = (localPos.y - rect.y) / rect.height;

        // 4. 映射到像素坐标
        Sprite sprite = img.sprite;
        int texX = Mathf.RoundToInt(sprite.rect.x + sprite.rect.width * xNorm);
        int texY = Mathf.RoundToInt(sprite.rect.y + sprite.rect.height * yNorm);

        // 5. 边界检查
        if (texX < 0 || texX >= sprite.texture.width || texY < 0 || texY >= sprite.texture.height)
            return false;

        // 6. 取像素alpha值
        Color color = sprite.texture.GetPixel(texX, texY);
        return color.a > 0.1f; // 透明阈值
    }

    void ApplyRotation()
    {
        if (editingHour)
        {
            hourHand.rotation = Quaternion.Euler(0, 0, -currentAngle);
        }
        else
        {
            minuteHand.rotation = Quaternion.Euler(0, 0, -currentAngle);
        }
    }

    void SnapToNearest()
    {
        if (editingHour)
        {
            // 每30°一个小时
            hour = Mathf.RoundToInt(currentAngle / 30f) % 12;
            currentAngle = hour * 30f;
            hourHand.rotation = Quaternion.Euler(0, 0, -currentAngle);
            if (isPM) {
                hourText.text = Math.Min((hour+12),23).ToString("D2");
            }
            else
            {
                hourText.text = hour.ToString("D2");
            }
            Debug.Log("选中小时: " + hour);
        }
        else
        {
            // 每6°一分钟
            minute = Mathf.RoundToInt(currentAngle / 6f) % 60;
            currentAngle = minute * 6f;
            minuteHand.rotation = Quaternion.Euler(0, 0, -currentAngle);
            minuteText.text = minute.ToString("D2");
            Debug.Log("选中分钟: " + minute.ToString("D2"));
        }
    }

    public void ChangePm() {
        if (isPM) {
            isPM = false;
            pm.text = "上午";
            hourText.text = hour.ToString("D2");
        }
        else { 
            isPM = true;
            pm.text = "下午";
            hourText.text = Math.Min((hour+12),23).ToString("D2");
        }
    }
}
