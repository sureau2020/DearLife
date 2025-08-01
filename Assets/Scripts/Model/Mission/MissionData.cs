using System;



// ��������ڴ洢�������ݣ�����������ײ�model�Ľ���
public class MissionData 
{
    public string Title { get; private set; }
    public DateTime Deadline { get; private set; }
    public float Duration { get; private set; }
    public float Difficulty { get; private set; }
    public bool IsCompleted { get; private set; }


    // REQUIRE: 0 <= duration,dificulty <= 4; deadline > DateTime.Now; title != null
    public MissionData(string title, DateTime deadline, float duration, float difficulty)
    {
        Title = title;
        Deadline = deadline;
        Duration = duration;
        Difficulty = difficulty;
        IsCompleted = false;
    }


    // ������񣬷��ز������
    public OperationResult CompleteMission()
    {
        if (IsCompleted)
        {
            return OperationResult.Fail("��������ɣ��޷��ظ���ɡ�");
        }
        if (DateTime.Now > Deadline)
        {
            return OperationResult.Fail("�����ѹ��ڣ��޷���ɡ�");
        }
        IsCompleted = true;
        return OperationResult.Complete();
    }



}
