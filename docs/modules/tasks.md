# 任务与日历

## 目录/程序集

Model/Mission、Manager/TaskManager.cs、UI/Tasks；默认 Assembly-CSharp。

## 命名空间

全局命名空间；UI/Tasks/Task.cs 中 Task 是本项目 UI 类，标准异步 Task 常使用完整限定名/别名以避开冲突。

## 最后同步

commit `00d29af1871434383c29e34a13459b3534ee77ee`；2026-10-28；包含工作区未提交文档；业务实现未改。

## 职责

TaskManagerModel 懒加载月份与例行任务；MonthMissionData → DayMissionData → MissionData 组织已归档任务。TaskManager 按选中日期合并日任务与例行投影并排序，日历/输入面板/列表负责交互。

## 公开 API

- TaskManagerModel：`GetMonth(string)`、`GetRecurringMissionDatas()`、例行任务增删/按 ID 查询、`SaveMonth/SaveMonthAsync`、`SaveAllMonths/SaveAllMonthsAsync`、`SaveRecurringMissionDatas()`、`pushDDL(MissionData,int)`。
- MissionData：`CompleteMission()`、`pushMission(int)`；RecurringMissionData：`IsOccurringOnDate(DateTime)`、`GetRecurrenceDaysStringInChinese()`。
- MonthMissionData：`GetDayMissionData(string)`；DayMissionData：`AddMission`、`DeleteSpecificMission`、`TransferRecurringMissionsToNormalMissions`、`GetFirstUncompletedMission(s)`、`pushDDL`。
- TaskManager：`OnDaySelected(DateTime)`、`OnMonthDaySelected(DateTime)`、`GenerateRecurringTasksOnSpecificDate(DateTime,HashSet<string>)`、`GetArchievedTasksIDMap(List<MissionData>)`、`GetNextMission()`、月历开关。

## 公开接口契约

月份/日期格式见 [conventions](../conventions.md)。无截止用 DateTime.MinValue；MissionData 带 HasDeadline、BelongsToDate、SourceRecurringId。例行投影按 SourceRecurringId 去重，完成后归档。列表排序只比较一天内时刻，有 DDL 排在无 DDL 前。

逾期完成会标记完成并返回失败结果，StateManager 将此特殊结果用于半额奖励；不能把所有失败都当作未修改数据。`pushMission` 当前返回 `!isCrossDay`，而 TaskManagerModel 调用方将该 bool 当“跨日”判断，存在语义不一致；本次仅记录，未修复。任务持久化路径见 [storage](storage.md)。

## 依赖

[runtime](runtime.md)、[storage](storage.md)、[character](character.md)、[presentation](presentation.md)。

## 被依赖

[runtime](runtime.md)、[storage](storage.md)、[notifications](notifications.md)；通知使用 GetNextMission 获取当天首个未完成的已归档任务，不能视作全量未来日程检索。

## 事件

`TaskManager.event Action<List<MissionData>> OnTaskListUpdated` → TaskList；列表为当前合并结果，元素仍为可变模型。`DayCell.Action<int> OnCellClicked` → WeekCalendar，参数为周格索引；日历/输入 UI 的 Unity 按钮绑定见各文件和场景。

## 数据所有权

TaskManagerModel 静态单例拥有 monthMap 和例行任务列表；TaskManager 拥有选中日期与展示集合。GetAllLoadedMonths 仅复制字典容器；GetRecurringMissionDatas 返回内部列表。

## 文件清单

附属 `.meta` 同归属；任务/日期 prefab 由 [project](project.md) 记录。
- [Assets/Scripts/Manager/TaskManager.cs](<../../Assets/Scripts/Manager/TaskManager.cs>) — 源码。
- [Assets/Scripts/Model/Mission/DayMissionData.cs](<../../Assets/Scripts/Model/Mission/DayMissionData.cs>) — 源码。
- [Assets/Scripts/Model/Mission/MissionData.cs](<../../Assets/Scripts/Model/Mission/MissionData.cs>) — 源码。
- [Assets/Scripts/Model/Mission/MonthMissionData.cs](<../../Assets/Scripts/Model/Mission/MonthMissionData.cs>) — 源码。
- [Assets/Scripts/Model/Mission/RecurringMissionData.cs](<../../Assets/Scripts/Model/Mission/RecurringMissionData.cs>) — 源码。
- [Assets/Scripts/Model/Mission/TaskManagerModel.cs](<../../Assets/Scripts/Model/Mission/TaskManagerModel.cs>) — 源码。
- [Assets/Scripts/UI/Tasks/AddTaskButton.cs](<../../Assets/Scripts/UI/Tasks/AddTaskButton.cs>) — 源码。
- [Assets/Scripts/UI/Tasks/CartesianSelector.cs](<../../Assets/Scripts/UI/Tasks/CartesianSelector.cs>) — 源码。
- [Assets/Scripts/UI/Tasks/Clock.cs](<../../Assets/Scripts/UI/Tasks/Clock.cs>) — 源码。
- [Assets/Scripts/UI/Tasks/DayCell.cs](<../../Assets/Scripts/UI/Tasks/DayCell.cs>) — 源码。
- [Assets/Scripts/UI/Tasks/InputRecurringTaskPanel.cs](<../../Assets/Scripts/UI/Tasks/InputRecurringTaskPanel.cs>) — 源码。
- [Assets/Scripts/UI/Tasks/InputTaskPanel.cs](<../../Assets/Scripts/UI/Tasks/InputTaskPanel.cs>) — 源码。
- [Assets/Scripts/UI/Tasks/MonthCalendar.cs](<../../Assets/Scripts/UI/Tasks/MonthCalendar.cs>) — 源码。
- [Assets/Scripts/UI/Tasks/Task.cs](<../../Assets/Scripts/UI/Tasks/Task.cs>) — 源码。
- [Assets/Scripts/UI/Tasks/TaskList.cs](<../../Assets/Scripts/UI/Tasks/TaskList.cs>) — 源码。
- [Assets/Scripts/UI/Tasks/WeekCalendar.cs](<../../Assets/Scripts/UI/Tasks/WeekCalendar.cs>) — 源码。

## 验证

静态核对月/日键、例行生成、奖励/通知调用点。未执行交互或日期边界测试；维护延期逻辑时需特别验证跨日/跨月、无 DDL、例行任务归档及重复完成。

## 最近改动摘要

- 2026-10-28：初始化任务边界与契约，记录延期返回值不一致（L3 文档建档）；未改业务。
