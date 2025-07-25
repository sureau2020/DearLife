

using System;


// 这个类用于存储任务数据，处理与任务底层model的交互
public class MissionData 
{
    public string Title { get; private set; }
    public DateTime Deadline { get; private set; }
    public int Salary { get; private set; }
    public bool IsCompleted { get; private set; }

}
