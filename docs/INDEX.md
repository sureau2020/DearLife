# DearLife 项目索引

最后同步：commit `00d29af1871434383c29e34a13459b3534ee77ee`；2026-10-28。包含工作区未提交的初始化文档与 AGENTS.md；业务源码、配置、场景未修改。

DearLife（人生养成）是用角色养成、任务报酬、物品消费、对话和房间装饰串联的 TODO 应用。Unity 2022.3.62f3，URP 14.0.12，C#，uGUI / TextMeshPro，Newtonsoft.Json 3.2.1，DOTween，NativeGallery，Mobile Notifications 2.4.1。Android / iOS 有明确实现；桌面有鼠标输入分支，尚未验证桌面发布。未发现 UniTask、Luban、Excel 配表或 Addressables 实现；不套用 CRPG / 卡牌项目设定。

维护入口：[操作手册](OPERATING_MANUAL.md) · [跨模块约定](conventions.md)。模块是文档归属，不代表已经拆分程序集或完成解耦。

## 布局与路径映射

下表是归属入口；精确文件清单见模块“文件清单”。更具体的规则优先，其余 Assets 资源与工程文件归 project。

| 模块 | 归属路径 | 摘要 | 被依赖 |
|---|---|---|---|
| [runtime](modules/runtime.md) | `Manager/GameManager.cs`、`Manager/BootScene/`、`Manager/TimeManager.cs`、`Model/StateManager.cs`（均在 `Assets/Scripts/`） | 启动、场景流转、状态协调与时间 | storage、character、tasks、dialogue、room、appearance、notifications、presentation |
| [storage](modules/storage.md) | `Assets/Scripts/Manager/Persistence/{SaveManager,LoaderManager}.cs`；`Assets/StreamingAssets/`；`Assets/GameData/` | JSON 配置载入和持久化 | runtime、character、tasks、room、appearance |
| [character](modules/character.md) | `Assets/Scripts/Model/{Character,Item}/`；`Model/{PlayerData,GameSettings}.cs`；`Tools/Calculators.cs`；`Manager/ItemInfoManager.cs`；`UI/Store/`；`UI/Character/{BackPackUI,ItemUI,InfoUI}.cs`；`UI/RebirthUI.cs` | 角色、背包、商店、设置值、任务报酬 | runtime、storage、tasks、dialogue、room、appearance、notifications、presentation |
| [tasks](modules/tasks.md) | `Assets/Scripts/Model/Mission/`；`Manager/TaskManager.cs`；`UI/Tasks/` | 月/日任务、例行任务、日历 | runtime、storage、notifications |
| [dialogue](modules/dialogue.md) | `Assets/Scripts/Model/DialogueSystem/`；`Manager/DialogueManager.cs`；`GameObject/AIChat.cs`；`UI/Choice/`；`UI/Character/DialoguePanelUI.cs` | 节点执行、对话 UI、Gemini 调用 | runtime、storage、character |
| [room](modules/room.md) | `Assets/Scripts/Model/{Tile,Furniture,Navigation}/`；`Manager/RoomManager.cs`；`Manager/Persistence/{TileLoader,FurnitureLoader}.cs`；`GameObject/Furnish/`；`GameObject/{RoomView,TileMapLab,CharacterTileMoveAI,CharacterMoveAI}.cs`；`Tools/WalkRules.cs`；`Assets/Resources/` 中除 DOTweenSettings 外资源；`Assets/SO/` | 网格、家具、装饰、A*、房间视图与移动 | runtime、storage、presentation、project |
| [appearance](modules/appearance.md) | `Assets/Scripts/Manager/EditScene/`；`UI/Edit/`；`UI/Closet/`；`GameObject/CharacterAppearanceRenderer.cs`；`Assets/Shader/`；`Assets/Material/AppearenceComponent.mat`；`PreFab/{Component*,Cloth*,uploadHint*}.prefab` | 建角编辑、衣柜、图集、导入图片 | runtime、presentation |
| [notifications](modules/notifications.md) | `Assets/Scripts/Notification/`；`Assets/CoroutineRunner.cs` | 本地通知、状态预测、任务提醒 | 无其他业务模块直接调用；由场景生命周期驱动 |
| [presentation](modules/presentation.md) | `Assets/Scripts/` 中未列入以上模块的 UI、Manager、GameObject、Tools；`Model/OperationResult.cs`；`Assets/{Billboard,SetText}.cs`；`Assets/FX/` | 通用 UI、相机、角色点击、音效、确认与错误提示 | runtime、storage、character、tasks、dialogue、room、appearance |
| [project](modules/project.md) | `Assets/Editor/`；`Assets/Scenes/`；其余 prefab、资源；`Packages/`；`ProjectSettings/`；根目录工程说明和设置 | 编辑器工具、场景接线、依赖与构建配置 | 所有运行时模块的资源与构建环境 |

## 程序入口

构建场景顺序：`Boot` → `EditCharacter` → `Room`。Boot 检查存档后进入 Room 或建角页；确认建角后进入 Room。具体生命周期和依赖见 [runtime](modules/runtime.md)。资源与 `.meta` 必须成对维护。

## ADR

| 编号 | 主题 | 状态 |
|---|---|---|
| [0001](adr/0001-existing-architecture.md) | 单体 Unity 工程与 JSON / ScriptableObject 数据现状 | 现状记录 |
| [0002](adr/0002-async-strategy.md) | UniTask 适用性与分阶段引入建议 | 评估完成；暂不引入 |

## 扫描范围与验证

- 盘点自有 C# 151 个：`Assets/Scripts/` 143、`Assets/Editor/` 5、Assets 根目录 3；查阅核心链路、公开 API、事件和真实调用点，以及场景、prefab、JSON、SO、包与工程设置。
- 配置静态验证：运行时聚合表 85 个事件、31 个物品；JSON 可解析、ID 无重复、物品引用事件存在、事件入口节点存在。
- 已发现分文件 `Events/yinxing_item_news_001.json` 与聚合表内容不同，见 [storage §验证](modules/storage.md#验证)。仅记录，未改动配置。
- 未启动 Unity、未编译或运行 Play Mode / 真机；本次不声明运行时验证通过。
- `Library/`、`Temp/`、`obj/`、`Logs/`、IDE 缓存、APK、Apple 导出物和压缩包不作为源码；第三方仅识别依赖与接入边界，未逐文件审计其内部实现。
