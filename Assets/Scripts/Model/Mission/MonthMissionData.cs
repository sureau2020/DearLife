//��ʾһ���µ���������
using System.Collections.Generic;

public class MonthMissionData
{
    public string Month { get; private set; } // ��ʾ����һ���µ��������ݣ�2023-10���ָ�ʽ
    private Dictionary<string, DayMissionData> dayMap = new();
}
