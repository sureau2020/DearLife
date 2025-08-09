using System;



// ��������ڴ洢�������ݣ�����������ײ�model�Ľ���
public class MissionData 
{
    public string Title { get; private set; }
    public DateTime Deadline { get; private set; }
    public float Duration { get; private set; }
    public float Difficulty { get; private set; }
    public bool IsCompleted { get; private set; }

    public bool HasDeadline { get; private set; }


    // REQUIRE: 0 <= duration,dificulty <= 4; ����deadline > DateTime.Now;��û��deadline�ʹ���DateTime.MinValue�� title != null
    public MissionData(string title, DateTime deadline, float duration, float difficulty)
    {
        Title = title;
        Deadline = deadline;
        if (Deadline == DateTime.MinValue)
        {
            HasDeadline = false;
        }
        else { 
            HasDeadline = true;
        }
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
        if (HasDeadline && DateTime.Now > Deadline)
        {
            return OperationResult.Fail("�����ѹ��ڣ��޷���ɡ�");
        }
        IsCompleted = true;
        return OperationResult.Complete();
    }

    // �Ƴ������DDL�����������Ҳ���£�������false����
    public bool pushMission(int delayMinutes)
    {
        DateTime newDeadline = Deadline.AddMinutes(delayMinutes);
        bool isCrossDay = newDeadline.Date != Deadline.Date;
        Deadline = newDeadline;
        return !isCrossDay;
    }


}
