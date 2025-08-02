// 表示一天的任务数据
using System;
using System.Collections.Generic;

public class DayMissionData 
{
    public string Day { get; private set; } // 表示是哪一天的任务数据，2023-10-10这种
    public List<MissionData> Tasks { get; private set; } = new List<MissionData>(); // 任务列表

    public DayMissionData(string day)
    {
        Day = day;
    }

    public void DeleteSpecificMission(MissionData mission) { 
        Tasks.Remove(mission);
    }

    public void AddMission(MissionData mission) {
        Tasks.Add(mission);
    }


    // 获取离当前时间最近的未完成且有deadline的任务，没有的话返回null
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
    // 获取离当前时间最近的前num个未完成且有deadline的任务，没有的话返回null
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

    // TODO从month推迟，不要从day推迟
    public OperationResult pushDDL(int delayTime, int numOfMission) {
        List<MissionData> missionsToDelay = GetFirstUncompletedMissions(numOfMission);
        if (missionsToDelay == null || missionsToDelay.Count == 0) {
            return OperationResult.Fail("没有可以推迟DDL的任务。");
        }
        foreach (MissionData mission in missionsToDelay) {
            bool isCrossDay = mission.pushMission(delayTime);
            if(isCrossDay) {
                
            }
        }
        return OperationResult.Complete();
    }




}
