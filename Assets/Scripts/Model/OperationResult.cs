


// ��������Ա�ʾ��Ϸ��ϸ΢�����Ľ�����繺����Ʒ�ɹ�û�У�������ɹ������ɹ���ԭ����ʲô
public class OperationResult 
{
    public bool Success { get; private set; }   // �����Ƿ�ɹ�
    public string Message { get; private set; } // ������Ϣ���ɹ�ʱΪ�գ�

    // �ɹ�
    public static OperationResult Complete()
    {
        return new OperationResult { 
            Success = true , 
            Message = "�ɹ���"};
    }

    // ʧ��
    public static OperationResult Fail(string message)
    {
        return new OperationResult{
            Success = false,
            Message = message,
        };
    }

}
