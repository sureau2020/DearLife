# 移动端本地通知

## 目录/程序集

Assets/Scripts/Notification、Assets/CoroutineRunner.cs；默认 Assembly-CSharp。

## 命名空间

全局命名空间。

## 最后同步

commit `00d29af1871434383c29e34a13459b3534ee77ee`；2026-10-28；包含工作区未提交文档；业务实现未改。

## 职责

NotificationManager 封装 Android / iOS 权限、渠道、排程和取消。CharacterStatsManager 在 Start 初始化并按角色状态预测通知，进入后台时清理既有排程后重算；同时安排任务截止前提醒。CoroutineRunner 提供常驻协程宿主，供 iOS 等待授权。

## 公开 API

- `Initialize(bool resetOnLaunch = true)`、`StableId(string)`、`ScheduleNotification(string,string,float,string)`（float 为小时）、`ScheduleAt(string,string,DateTime,string)`。
- `CancelByIdentifier(string)`、`CancelAllScheduled()`、`CancelAllDelivered()`、`CancelAll()`。
- `CharacterStatsManager.ScheduleAllNotifications()`；CharacterStat 的值同步、健康同步、到阈值时间计算；StatusDescriptor 格式化文本。
- `CoroutineRunner.Run(IEnumerator)`。

## 公开接口契约

Mobile Notifications 2.4.1；Android 固定 default_channel，稳定 FNV-1a ID；过去/过近排程调整到至少约 5 秒后。iOS 使用原始字符串 identifier 与时间间隔触发器。平台宏外通知方法没有实际排程动作。

CharacterStatsManager 依赖 Stats 配置包含 Full / San / Clean；最多模拟 10 次状态切换。预测使用独立 DecayRatePerHour，不是执行 StateManager 衰减：Crazy / Dirty 倍率当前为 4，与运行时每轮 -3 的逻辑不同，不能保证时间精确一致。任务提醒通过 GetNextMission，距截止不足 10 分钟时改成短延迟。

iOS 授权等待为异步协程，初始化返回时不保证完成；授权完成后 reset 可能清除已经排好的通知，实际顺序待真机验证。无需为文档初始化触发系统权限。

## 依赖

[runtime](runtime.md)、[character](character.md)、[tasks](tasks.md)，Unity Mobile Notifications 及平台权限 API。

## 被依赖

无其他自有业务脚本直接调用本模块公共入口；由场景挂载组件的生命周期驱动。

## 事件

无模块公开 C# 事件；由 Start / OnApplicationPause 触发，不通过角色变化事件实时刷新全部排程。

## 数据所有权

Stats / NotificationRules 是场景序列化配置；模拟字典是副本，不改真实角色。已排程通知由操作系统管理；通知排程不进入主状态存档。

## 文件清单

附属 `.meta` 同归属。
- [Assets/CoroutineRunner.cs](<../../Assets/CoroutineRunner.cs>) — 源码。
- [Assets/Scripts/Notification/CharacterStat.cs](<../../Assets/Scripts/Notification/CharacterStat.cs>) — 源码。
- [Assets/Scripts/Notification/CharacterStatsManager.cs](<../../Assets/Scripts/Notification/CharacterStatsManager.cs>) — 源码。
- [Assets/Scripts/Notification/NotificationManager.cs](<../../Assets/Scripts/Notification/NotificationManager.cs>) — 源码。

## 验证

核对 Android / iOS 分支、依赖版本、任务提醒入口。未申请权限、未发通知；需两端真机验证授权后首次排程、退后台重排、重复标识取消和模型预测偏差。

## 最近改动摘要

- 2026-10-28：初始化通知模块，记录预测和授权时序限制（L3 文档建档）。
