

public class GameSettings 
{
    public float MaxSalaryFactor { get; private set; } = 1.2f; //����нˮ������������
    public float MinSalaryFactor { get; private set; } = 0.8f; //����нˮ����С�������

    public int HourlyWage { get; private set; } = 15;//Сʱ����

    public int DifficultyBonus { get; private set; } = 3; //�����Ѷ�ÿ1��нˮ���Ӷ���

    public int CountdownWagePerHour { get; private set; } = 3; //����ʱģʽ��ÿСʱ�ͱ�

    public OperationResult Validate()
    {
        if (MinSalaryFactor < 0)
        {
            return OperationResult.Fail("нˮ����������޲���С��0");
        }

        if (MaxSalaryFactor < MinSalaryFactor)
        {
            return OperationResult.Fail("нˮ����������� ���ܴ��� нˮ�����������");
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




}
