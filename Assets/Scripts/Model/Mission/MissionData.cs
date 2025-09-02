using System;



// ��������ڴ洢�������ݣ�����������ײ�model�Ľ���
public class MissionData 
{
    public string Title { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime BelongsToDate { get; set; } //�����������
    public float Duration { get; set; }
    public float Difficulty { get; set; }
    public bool IsCompleted { get; set; }
    public bool HasDeadline { get; set; }

    // REQUIRE: 0 <= duration,dificulty <= 4; ����deadline > DateTime.Now;��û��deadline�ʹ���DateTime.MinValue�� title != null
    // �޸Ĺ��캯��
    public MissionData(string title, DateTime deadline, float duration, float difficulty, DateTime belongsToDate)
    {
        Title = title;
        Deadline = deadline;
        if (Deadline == DateTime.MinValue)
        {
            HasDeadline = false;
            // ���û��ָ���������ڣ�Ĭ��Ϊ����
            BelongsToDate = belongsToDate;
        }
        else { 
            HasDeadline = true;
            // �н�ֹ���ڵ����񣬹������ھ��ǽ�ֹ����
            BelongsToDate = deadline.Date;
        }
        Duration = duration;
        Difficulty = difficulty;
        IsCompleted = false;
    }


    // ������񣬷��ز������
    // �޸��������ı����߼�
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
        _ = TaskManagerModel.Instance.SaveMonthAsync(BelongsToDate.ToString("yyyy-MM"));
        return OperationResult.Complete();
    }

    // �Ƴ������DDL�����������Ҳ���£�������false����
    public bool pushMission(int delayMinutes)
    {
        if (!HasDeadline) return false; // �� DDL �������Ƴ�
        
        DateTime newDeadline = Deadline.AddMinutes(delayMinutes);
        bool isCrossDay = newDeadline.Date != Deadline.Date;
        Deadline = newDeadline;
        BelongsToDate = newDeadline.Date; // ���¹�������
        return !isCrossDay;
    }
}
