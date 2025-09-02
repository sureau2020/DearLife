//��ʾһ���µ���������
using System.Collections.Generic;

public class MonthMissionData
{
    public string Month { get; set; } // ��ʾ����һ���µ��������ݣ�2023-10���ָ�ʽ
    public Dictionary<string, DayMissionData> Days { get => dayMap; set => dayMap = value; } // �������л�
    private Dictionary<string, DayMissionData> dayMap = new();


    //string month ��2023-10���ָ�ʽ
    public MonthMissionData(string month)
    {
       Month = month;
    }


    // string day ��2023-10-10���ָ�ʽ
    public DayMissionData GetDayMissionData(string day)
    {
        if (!dayMap.ContainsKey(day))
        {
            DayMissionData newDate = new DayMissionData(day);
            dayMap[day] = newDate;
        }
        return dayMap[day];
    }


}
