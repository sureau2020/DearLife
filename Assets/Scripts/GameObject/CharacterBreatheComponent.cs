using UnityEngine;

public class CharacterBreatheComponent : MonoBehaviour
{
    private float breatheSpeed = 2f;   // �����ٶ�
    private float breatheScale = 0.01f;// ��������

    private Vector3 originalScale;  // ԭʼ���ţ�0.38, 0.38, 0.38��
    private bool isBreathing = true;
    private int flipDirection = 1;  // 1 �� -1����ʾ��ת����

    void Start()
    {
        originalScale = transform.localScale;
        // ����ԭʼ���ŵľ���ֵ
        originalScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
    }

    void Update()
    {
        if (isBreathing)
        {
            float breathe = Mathf.Sin(Time.time * breatheSpeed) * breatheScale;
            // Ӧ�÷�ת���� + ԭʼ���� + ��������
            Vector3 targetScale = new Vector3(
                originalScale.x * flipDirection,  // x �ῼ�Ƿ�ת
                originalScale.y + breathe,        // y ����Ӻ���Ч��
                originalScale.z
            );
            transform.localScale = targetScale;
        }
    }

    /// <summary>
    /// ���ú���״̬
    /// </summary>
    /// <param name="breathing">�Ƿ����ú���</param>
    public void SetBreathing(bool breathing)
    {
        isBreathing = breathing;
        if (!breathing)
        {
            // ֹͣ����ʱ�����ַ�ת��ȥ������Ч��
            transform.localScale = new Vector3(
                originalScale.x * flipDirection,
                originalScale.y,
                originalScale.z
            );
        }
    }

    /// <summary>
    /// ���ú�������
    /// </summary>
    /// <param name="speed">�����ٶ�</param>
    /// <param name="scale">��������</param>
    public void SetBreatheParameters(float speed, float scale)
    {
        breatheSpeed = speed;
        breatheScale = scale;
    }

    /// <summary>
    /// ���÷�ת�����ɸ�������ƶ��ű����ã�
    /// </summary>
    /// <param name="direction">1 ��ʾ��������-1 ��ʾ��ת</param>
    public void SetFlipDirection(int direction)
    {
        flipDirection = direction;
    }
}