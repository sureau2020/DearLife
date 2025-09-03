

public class GameSettings 
{
    public float MaxSalaryFactor { get; private set; } = 1.2f; //����нˮ������������

    public int HourlyWage { get; private set; } = 10;//Сʱ����

    public int DifficultyBonus { get; private set; } = 2; //�����Ѷ�ÿ1��нˮ���Ӷ���

    public int CountdownWagePerHour { get; private set; } = 3; //����ʱģʽ��ÿСʱ�ͱ�
    //TODO����û������wage�Ƿ�valid�����ۺ������������涼û��

    public int DelayCost { get; private set; } = 5; //����ʱ��ʱ����Ǯ��

    public int DelayMissionNum { get; private set; } = 3; //����ʱģʽ��ʱ������

    //0-5�����֣�����Խ��ظ�����Խ�ߣ�0���ظ���5�϶��ظ�
    public int ReplyChance { get; private set; } = 2;


    public OperationResult Validate()
    {

        if (MaxSalaryFactor <= 1)
        {
            return OperationResult.Fail("нˮ����������� ���� С��1");
        }

        if(HourlyWage <= 0)
        {
            return OperationResult.Fail("ʱн�������0����Ҫ��Ǯ�ϰ���");
        }

        if (DifficultyBonus < 0)
        {
            return OperationResult.Fail("�����ѶȽ�������С��0����Ҫ��Ǯ���ѻ���");
        }

        return OperationResult.Complete();
    }


    public OperationResult ChangeMaxRandomFactor(float factor)
    {
        if (factor <= 1)
        {
            return OperationResult.Fail("���нˮ������� ���� С�ڵ���1");
        }
        if(factor >= 2) { 
            return OperationResult.Fail("���нˮ������� ���� ���ڵ���2");
        }
        MaxSalaryFactor = factor;
        _ = GameManager.Instance.StateManager.SaveStateAsync();
        return OperationResult.Complete();
    }

    public OperationResult ChangeHourlyWage(int wage)
    {
        if (wage <= 0)
        {
            return OperationResult.Fail("ʱн�������0����Ҫ��Ǯ�ϰ���");
        }
        HourlyWage = wage;
        _ = GameManager.Instance.StateManager.SaveStateAsync();
        return OperationResult.Complete();
    }

    public OperationResult ChangeDifficultyBonus(int bonus)
    {
        if (bonus < 0)
        {
            return OperationResult.Fail("�����ѶȽ�������С��0����Ҫ��Ǯ���ѻ���");
        }
        DifficultyBonus = bonus;
        _ = GameManager.Instance.StateManager.SaveStateAsync();
        return OperationResult.Complete();
    }

    public OperationResult ChangeReplyChance(int chance)
    {
        if (chance < 0 || chance > 5)
        {
            return OperationResult.Fail("�ظ����ʱ�����0-5֮��");
        }
        ReplyChance = chance;
        _ = GameManager.Instance.StateManager.SaveStateAsync();
        return OperationResult.Complete();
    }


}
