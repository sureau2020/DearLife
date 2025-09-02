// 这个类用以表示游戏中细微操作的结果，如购买物品成功没有，如果不成功，不成功的原因是什么
public class OperationResult 
{
    public bool Success { get; protected set; }   
    public string Message { get; protected set; } 

    // 成功
    public static OperationResult Complete()
    {
        return new OperationResult { 
            Success = true , 
            Message = "成功。"};
    }

    // 失败
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
            Message = "成功。"
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

    // 删除重复的属性声明，直接继承基类的即可
}
