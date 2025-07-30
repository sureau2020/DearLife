


// 这个类用以表示游戏中细微操作的结果，如购买物品成功没有，如果不成功，不成功的原因是什么
public class OperationResult 
{
    public bool Success { get; private set; }   // 操作是否成功
    public string Message { get; private set; } // 错误信息（成功时为空）

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
