// ��ʾһ�����������
using System;
using System.Collections.Generic;

public class DayMissionData 
{
    public string Day { get; private set; } // ��ʾ����һ����������ݣ�2023-10-10����
    public List<MissionData> Tasks { get; private set; } = new List<MissionData>(); // �����б�

    public DayMissionData(string day)
    {
        Day = day;
    }

    public void DeleteSpecificMission(MissionData mission) { 
        Tasks.Remove(mission);
    }

    // �������¼�Զ�����������������ddl�����񣬾ͷ������
    public void AddMission(MissionData mission)
    {
        if (mission == null) return;
        if (!mission.HasDeadline)
        {
            Tasks.Add(mission);
            return;
        }
        for (int i = 0; i < Tasks.Count; i++)
        {
            var t = Tasks[i];
            if (!t.HasDeadline) 
            {
                Tasks.Insert(i, mission);
                return;
            }
            if (mission.Deadline < t.Deadline)
            {
                Tasks.Insert(i, mission);
                return;
            }
        }
        Tasks.Add(mission);
    }


    // ��ȡ�뵱ǰʱ�������δ�������deadline������û�еĻ�����null
    public MissionData GetFirstUncompletedMission() {
        if (Tasks == null || Tasks.Count == 0) {
            return null;
        }
        foreach (MissionData mission in Tasks) {
            if (!mission.IsCompleted && mission.HasDeadline && mission.Deadline > DateTime.Now) {
                return mission;
            }
        }
        return null;

    }


    // REQUIRE: num > 0
    // ��ȡ�뵱ǰʱ�������ǰnum��δ�������deadline������û�еĻ�����null
    public List<MissionData> GetFirstUncompletedMissions(int num) {
        if (Tasks == null || Tasks.Count == 0) {
            return null;
        }
        List<MissionData> result = new List<MissionData>();
        foreach (MissionData mission in Tasks) {
            if (!mission.IsCompleted && mission.HasDeadline && mission.Deadline > DateTime.Now) {
                result.Add(mission);
                if (result.Count >= num) {
                    break;
                }
            }
        }
        return result.Count > 0 ? result : null;
    }

    // TODO��month�Ƴ٣���Ҫ��day�Ƴ�
    public OperationResult pushDDL(int delayTime, int numOfMission) {
        List<MissionData> missionsToDelay = GetFirstUncompletedMissions(numOfMission);
        if (missionsToDelay == null || missionsToDelay.Count == 0) {
            return OperationResult.Fail("û�п����Ƴ�DDL������");
        }
        foreach (MissionData mission in missionsToDelay) {
            bool isCrossDay = mission.pushMission(delayTime);
            if(isCrossDay) {
                
            }
        }
        return OperationResult.Complete();
    }




}
