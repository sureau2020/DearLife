# 启动与状态协调

## 目录/程序集

归属路径见 [索引](../INDEX.md)：GameManager、BootScene、TimeManager、StateManager；默认 Assembly-CSharp。

## 命名空间

全局命名空间。

## 最后同步

commit `00d29af1871434383c29e34a13459b3534ee77ee`；2026-10-28；包含工作区未提交文档；业务源码无改动。

## 职责

BootSceneManager 在 Awake 常驻，设 30 FPS，检查主存档：成功进入 Room，否则进入 EditCharacter。建角确认生成 CharacterData 并进入 Room；从主界面编辑时把当前角色拷入编辑状态。

GameManager.Awake 创建家具、Tile、地图数据库，读取 StateManager（优先采用 Boot 保留的角色），计算离线衰减；获取对话/房间组件，协程顺序加载事件、衣柜、物品，关闭加载 UI 后应用外观。RoomManager 有自己的 Awake，并非等待 LoadAllData 后才初始化。

StateManager 协调玩家、角色、设置、自定义数值、任务完成奖励和延期消费；GameManager 提供 UI / 对话用门面。暂停或失焦时同步保存主状态。

## 公开 API

- BootSceneManager：`ConfirmCharacter()`、`EnterEditModeFromMain()`、`SetAppearance(CharacterAppearance)`、`ApplyAppearancePart(string,int)`，角色信息与性格标签读写。
- GameManager：`UseItemWithDialogue(string,int)`、`GetValueOfState(string)`、`SetValueOfState(string,int)`、`GetSetting(string)`、`ChangeSetting(string,string)`、`SaveGame()`、`LoadGame()`、`ResetRoom()`、`CalculatePassTime()`。
- StateManager：`BuyItem`、`BuyCloth`、`UseItem`、`ApplyEffect`、`CompleteMission(MissionData)`、`PushRecentMissionsDDL(MissionData)`、`SaveState()`、`SaveStateAsync()`、静态 `LoadState()`。

## 公开接口契约

Instance / StateManager 依赖启动顺序；直接从 Room 进入不能视作 Boot 初始化完毕。`LoadGame` 替换状态实例，已有 UI 订阅不会自动迁移。

TimeManager.OnMinuteChanged 实际每 12 个现实分钟触发一次；小时/跨日调用被注释。离线也按 12 分钟换算，但 CalculatePassTime 无论间隔是否不足一轮，末尾仍执行一次 DecayStates。衰减计数器为内存字段。

DecayStates：通常每轮 Full -1，Crazy 时 Full -3 / Love -2；每 2 轮 Clean -1；每 4 轮 San -1，Dirty 时 -3。Dead 停止衰减。状态增量/缺失值约定见 [conventions](../conventions.md)。

## 依赖

[character](character.md)、[storage](storage.md)、[tasks](tasks.md)、[dialogue](dialogue.md)、[room](room.md)、[appearance](appearance.md)、[presentation](presentation.md)。

## 被依赖

[storage](storage.md)、[character](character.md)、[tasks](tasks.md)、[dialogue](dialogue.md)、[room](room.md)、[appearance](appearance.md)、[notifications](notifications.md)、[presentation](presentation.md)；通过状态类型或 Instance 获取当前状态。

## 事件

- BootSceneManager.`event Action<CharacterAppearance,string> OnAppearanceChanged` → AppearanceLoader、HSVSliderController。
- TimeManager.`event Action<DateTime> OnMinuteChanged / OnHourChanged / OnDayChanged` → GameManager；后两者当前不触发。

## 数据所有权

BootSceneManager 保留跨场景编辑信息和 CreatedCharacter；GameManager 持有本场景 StateManager / 数据库；StateManager 持有 Player / Character / Settings / CustomStates。Appearance、PersonalityTags 等可能共享可变引用。

## 文件清单

下方逐文件清单由初始化盘点生成；附属 `.meta` 同归属。
- [Assets/Scripts/Manager/BootScene/BootSceneManager.cs](<../../Assets/Scripts/Manager/BootScene/BootSceneManager.cs>) — 源码。
- [Assets/Scripts/Manager/GameManager.cs](<../../Assets/Scripts/Manager/GameManager.cs>) — 源码。
- [Assets/Scripts/Manager/TimeManager.cs](<../../Assets/Scripts/Manager/TimeManager.cs>) — 源码。
- [Assets/Scripts/Model/StateManager.cs](<../../Assets/Scripts/Model/StateManager.cs>) — 源码。

## 验证

静态核对启动路径、构建场景顺序和订阅。GameManager / RoomManager / TimeManager 的脚本 meta executionOrder 均为 0，未验证实际 Awake 顺序。未运行 Unity。建议从 Boot 验证首次建角、已有存档、编辑返回、失焦保存及离线恢复。

## 最近改动摘要

- 2026-10-28：初始化现状文档（L3 文档建档；业务实现未改），见 [ADR 0001](../adr/0001-existing-architecture.md)。
