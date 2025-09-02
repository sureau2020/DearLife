// ��������Ա�ʾ��Ϸ��ϸ΢�����Ľ�����繺����Ʒ�ɹ�û�У�������ɹ������ɹ���ԭ����ʲô
public class OperationResult 
{
    public bool Success { get; protected set; }   
    public string Message { get; protected set; } 

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

public class OperationResult<T> : OperationResult
{
    public T Data { get; private set; }

    public static OperationResult<T> Complete(T data)
    {
        return new OperationResult<T>
        {
            Success = true,
            Data = data,
            Message = "�ɹ���"
        };
    }

    public static new OperationResult<T> Fail(string message)
    {
        return new OperationResult<T>
        {
            Success = false,
            Data = default(T),
            Message = message
        };
    }

    // ɾ���ظ�������������ֱ�Ӽ̳л���ļ���
}
