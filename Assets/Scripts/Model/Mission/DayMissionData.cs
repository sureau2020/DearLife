// ��ʾһ�����������
using System.Collections.Generic;

public class DayMissionData 
{
    public string Day { get; private set; } // ��ʾ����һ����������ݣ�2023-10-10����
    public List<MissionData> Tasks { get; private set; } // �����б�


    public void DeleteSpecificMission(MissionData mission) { 
        Tasks.Remove(mission);
    }

}
