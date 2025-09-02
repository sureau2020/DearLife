using UnityEngine;

public class CharacterBreatheComponent : MonoBehaviour
{
    private float breatheSpeed = 2f;   // 呼吸速度
    private float breatheScale = 0.01f;// 呼吸幅度

    private Vector3 originalScale;  // 原始缩放（0.38, 0.38, 0.38）
    private bool isBreathing = true;
    private int flipDirection = 1;  // 1 或 -1，表示翻转方向

    void Start()
    {
        originalScale = transform.localScale;
        // 保存原始缩放的绝对值
        originalScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
    }

    void Update()
    {
        if (isBreathing)
        {
            float breathe = Mathf.Sin(Time.time * breatheSpeed) * breatheScale;
            // 应用翻转方向 + 原始缩放 + 呼吸动画
            Vector3 targetScale = new Vector3(
                originalScale.x * flipDirection,  // x 轴考虑翻转
                originalScale.y + breathe,        // y 轴添加呼吸效果
                originalScale.z
            );
            transform.localScale = targetScale;
        }
    }

    /// <summary>
    /// 设置呼吸状态
    /// </summary>
    /// <param name="breathing">是否启用呼吸</param>
    public void SetBreathing(bool breathing)
    {
        isBreathing = breathing;
        if (!breathing)
        {
            // 停止呼吸时，保持翻转但去掉呼吸效果
            transform.localScale = new Vector3(
                originalScale.x * flipDirection,
                originalScale.y,
                originalScale.z
            );
        }
    }

    /// <summary>
    /// 设置呼吸参数
    /// </summary>
    /// <param name="speed">呼吸速度</param>
    /// <param name="scale">呼吸幅度</param>
    public void SetBreatheParameters(float speed, float scale)
    {
        breatheSpeed = speed;
        breatheScale = scale;
    }

    /// <summary>
    /// 设置翻转方向（由父物体的移动脚本调用）
    /// </summary>
    /// <param name="direction">1 表示正常方向，-1 表示翻转</param>
    public void SetFlipDirection(int direction)
    {
        flipDirection = direction;
    }
}