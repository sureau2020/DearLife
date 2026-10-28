# 工程、场景与编辑器工具

## 目录/程序集

Assets/Editor，Scenes，其余 prefab / 美术 / 第三方资源，Packages、ProjectSettings、根目录工程说明；Editor 为默认 Assembly-CSharp-Editor，资源不编译。第三方可自带独立程序集。

## 命名空间

自有编辑器工具为全局命名空间；第三方按其原有命名空间，未逐包审计。

## 最后同步

commit `49fa54da6e0cec37f33577c8593db375d1a21015`；2026-10-28；包含工作区未提交的 Tilemap 编辑器本地包、包锁文件及本文档改动；另同步 DeepSeek 接入的 Room AI 设置提示文本。AI 接入仅更改该场景的一处说明，工作区其他 Room 布局改动保留，不属于 AI 接入；prefab 未改。

## 职责

记录 Unity 版本、包依赖、构建场景、资源接线及 Tilemap 编辑器工具，避免将生成输出当成源码或将其他工程示例套入本工程。

## 公开 API

编辑器菜单：`Tools/Map Editor/Built-in Tile Scanner`、`Map Layout Exporter`、`Tilemap Ghost Buster`；`Tools/Tilemap/Print Used Tile Names`；`Tools/Export Tilemap To PNG`。这些工具会扫描、清理或导出资源，初始化仅阅读，未执行修改型工具。

## 公开接口契约

- Unity `2022.3.62f3`；URP `14.0.12`，Newtonsoft `3.2.1`，TMP `3.0.7`，Notifications `2.4.1`，AI Navigation `1.1.7`；其他依赖以 manifest / lock 为准。
- BuildSettings 顺序为 Boot / EditCharacter / Room，均启用。ProductName 人生养成，版本 0.9.11，CompanyName AbyssalPort；Android / iPhone 配置和本地 APK / Apple 导出物表明移动端目标，不能据此声称构建通过。
- 项目使用旧输入接口（activeInputHandler=0）；存在 inputactions 资源不代表已迁移新输入系统。
- MapLayoutExporter 将 Tilemap 中 Tile 名称与位置写入指定 MapDataSO，不会同步 TileConfig / TileLibrary。更改 tile 名称、SO 路径或 prefab GUID 必须同步检查 [room](room.md)。
- 本地 Plugins 有 DOTween、NativeGallery 等，部分插件目录被 .gitignore 忽略。干净检出依赖完整性未验证，不能保证复制源码即可编译。
- Room 的 AI 设置说明列出 Gemini / DeepSeek，并提示切换 API 后选择对应模型、更新密钥；Model 下拉框运行时由 [presentation](presentation.md) 从 [dialogue](dialogue.md) 供应商目录加载。DeepSeek 接入仅更新说明文本，无需新增组件或修改 prefab。
- `com.unity.2d.tilemap@1.0.0` 以 `Packages/com.unity.2d.tilemap/` 嵌入项目，覆盖 Unity 自带版本；锁文件应为 `source: embedded`。鼠标网格坐标差值先转为 `long` 再相减、求绝对值，避免整数回绕及 `Abs(int.MinValue)` 导致 Scene 视图反复抛出 OverflowException；保留原有每轴 500 格限幅和公开 API。后续升级 Unity 时需比较上游实现并验证是否可移除本地包，详见包内 `LOCAL_PATCH.md`。

## 依赖

[room](room.md) 的 MapDataSO / TileConfigSO / TileBaseLibrarySO，以及 UnityEditor / Tilemap。场景和 prefab 串联所有运行时模块，业务说明按 [索引](../INDEX.md) 链接。

## 被依赖

所有运行时模块依赖其工程设置、场景接线与资源。无独立业务调用方。

## 事件

无自有跨模块 C# 事件；Unity 序列化 UnityEvent / 挂载引用属于场景接线，具体入口见各模块。

## 数据所有权

资源与 `.meta` 由 Unity AssetDatabase 管理。Packages/manifest.json 与 packages-lock.json 管包版本；ProjectSettings 管构建/渲染等设置。Library / Temp / obj / Logs、IDE 工程文件及 Apple / APK 导出物为缓存或输出，不参与业务源码归属。

## 文件清单

下方列自有编辑器、场景、通用 prefab、工程配置及其余受版本管理资源。大型美术/第三方资源按目录与数量归组，包含其中派生资源；不复制第三方实现。附属 `.meta` 与目录 meta 同归属。未跟踪且被忽略的第三方资源只记录存在，不将其当作仓库保证。
- `Assets/Arts/**`：受版本管理的非 meta 文件 65 个；本地非 meta 文件 712 个。附属 meta 同归属；第三方仅核对接入边界。
- `Assets/Font/**`：受版本管理的非 meta 文件 4 个；本地非 meta 文件 6 个。附属 meta 同归属；第三方仅核对接入边界。
- `Assets/TextMesh Pro/**`：受版本管理的非 meta 文件 32 个；本地非 meta 文件 32 个。附属 meta 同归属；第三方仅核对接入边界。
- `Assets/Plugins/**`：受版本管理的非 meta 文件 0 个；本地非 meta 文件 76 个。附属 meta 同归属；第三方仅核对接入边界。
- `Assets/Brick Project Studio/**`：受版本管理的非 meta 文件 0 个；本地非 meta 文件 698 个。附属 meta 同归属；第三方仅核对接入边界。
- `Assets/Lana Studio/**`：受版本管理的非 meta 文件 0 个；本地非 meta 文件 60 个。附属 meta 同归属；第三方仅核对接入边界。
- `Assets/EnjoyGameClub/**`：受版本管理的非 meta 文件 0 个；本地非 meta 文件 53 个。附属 meta 同归属；第三方仅核对接入边界。
- `Assets/UnityTechnologies/**`：受版本管理的非 meta 文件 0 个；本地非 meta 文件 154 个。附属 meta 同归属；第三方仅核对接入边界。
- [.gitattributes](<../../.gitattributes>) — 资源/工程文件。
- [.gitignore](<../../.gitignore>) — 资源/工程文件。
- [.vsconfig](<../../.vsconfig>) — 资源/工程文件。
- [Assets/Editor/CollectUsedTiles.cs](<../../Assets/Editor/CollectUsedTiles.cs>) — 源码。
- [Assets/Editor/MapLayoutExporter.cs](<../../Assets/Editor/MapLayoutExporter.cs>) — 源码。
- [Assets/Editor/TilemapCleaner.cs](<../../Assets/Editor/TilemapCleaner.cs>) — 源码。
- [Assets/Editor/TilemapDataScanner.cs](<../../Assets/Editor/TilemapDataScanner.cs>) — 源码。
- [Assets/Editor/TilemapToPNG.cs](<../../Assets/Editor/TilemapToPNG.cs>) — 源码。
- [Assets/InputSystem_Actions.inputactions](<../../Assets/InputSystem_Actions.inputactions>) — 资源/工程文件。
- [Assets/Material/BaseColor.mat](<../../Assets/Material/BaseColor.mat>) — 资源/工程文件。
- [Assets/Material/Skybox.mat](<../../Assets/Material/Skybox.mat>) — 资源/工程文件。
- [Assets/MonthCalendar.prefab](<../../Assets/MonthCalendar.prefab>) — 资源/工程文件。
- [Assets/PreFab/CharacterRoot.prefab](<../../Assets/PreFab/CharacterRoot.prefab>) — 资源/工程文件。
- [Assets/PreFab/Choice.prefab](<../../Assets/PreFab/Choice.prefab>) — 资源/工程文件。
- [Assets/PreFab/CustomFurnishItem.prefab](<../../Assets/PreFab/CustomFurnishItem.prefab>) — 资源/工程文件。
- [Assets/PreFab/ErrorPanel.prefab](<../../Assets/PreFab/ErrorPanel.prefab>) — 资源/工程文件。
- [Assets/PreFab/FurnishItem.prefab](<../../Assets/PreFab/FurnishItem.prefab>) — 资源/工程文件。
- [Assets/PreFab/GoodsSample.prefab](<../../Assets/PreFab/GoodsSample.prefab>) — 资源/工程文件。
- [Assets/PreFab/ItemSample.prefab](<../../Assets/PreFab/ItemSample.prefab>) — 资源/工程文件。
- [Assets/PreFab/Misson.prefab](<../../Assets/PreFab/Misson.prefab>) — 资源/工程文件。
- [Assets/PreFab/MonthDayCell.prefab](<../../Assets/PreFab/MonthDayCell.prefab>) — 资源/工程文件。
- [Assets/PreFab/WeekDayCell.prefab](<../../Assets/PreFab/WeekDayCell.prefab>) — 资源/工程文件。
- [Assets/Readme.asset](<../../Assets/Readme.asset>) — 资源/工程文件。
- [Assets/Resources/DOTweenSettings.asset](<../../Assets/Resources/DOTweenSettings.asset>) — 资源/工程文件。
- [Assets/Scenes/Boot.unity](<../../Assets/Scenes/Boot.unity>) — 资源/工程文件。
- [Assets/Scenes/EditCharacter.unity](<../../Assets/Scenes/EditCharacter.unity>) — 资源/工程文件。
- [Assets/Scenes/Room.unity](<../../Assets/Scenes/Room.unity>) — 资源/工程文件。
- [Assets/Scenes/Room/Global Volume Profile.asset](<../../Assets/Scenes/Room/Global Volume Profile.asset>) — 资源/工程文件。
- [Assets/Scenes/Room/NavMesh-NavMesh Surface.asset](<../../Assets/Scenes/Room/NavMesh-NavMesh Surface.asset>) — 资源/工程文件。
- [Assets/Settings/SampleSceneProfile.asset](<../../Assets/Settings/SampleSceneProfile.asset>) — 资源/工程文件。
- [Assets/Settings/URP-Balanced-Renderer.asset](<../../Assets/Settings/URP-Balanced-Renderer.asset>) — 资源/工程文件。
- [Assets/Settings/URP-Balanced.asset](<../../Assets/Settings/URP-Balanced.asset>) — 资源/工程文件。
- [Assets/Settings/URP-HighFidelity-Renderer.asset](<../../Assets/Settings/URP-HighFidelity-Renderer.asset>) — 资源/工程文件。
- [Assets/Settings/URP-HighFidelity.asset](<../../Assets/Settings/URP-HighFidelity.asset>) — 资源/工程文件。
- [Assets/Settings/URP-Performant-Renderer.asset](<../../Assets/Settings/URP-Performant-Renderer.asset>) — 资源/工程文件。
- [Assets/Settings/URP-Performant.asset](<../../Assets/Settings/URP-Performant.asset>) — 资源/工程文件。
- [Assets/UniversalRenderPipelineGlobalSettings.asset](<../../Assets/UniversalRenderPipelineGlobalSettings.asset>) — 资源/工程文件。
- [LICENSE](<../../LICENSE>) — 资源/工程文件。
- [Packages/manifest.json](<../../Packages/manifest.json>) — 配置/聚合资源。
- [Packages/packages-lock.json](<../../Packages/packages-lock.json>) — 配置/聚合资源。
- [Packages/com.unity.2d.tilemap/](<../../Packages/com.unity.2d.tilemap/>) — Unity 2022.3.62f3 自带的 2D Tilemap Editor 1.0.0 完整本地副本（Editor、Tests、Documentation~、包元数据及许可）；保留原始 `.meta` / GUID。本地实现差异仅 `Editor/PaintableGrid.cs`，回归测试补充于 `Tests/Editor/TilemapPackageTests.cs`；维护说明为 `LOCAL_PATCH.md` 及其 `.meta`。
- [ProjectSettings/AudioManager.asset](<../../ProjectSettings/AudioManager.asset>) — 资源/工程文件。
- [ProjectSettings/BurstAotSettings_StandaloneWindows.json](<../../ProjectSettings/BurstAotSettings_StandaloneWindows.json>) — 配置/聚合资源。
- [ProjectSettings/ClusterInputManager.asset](<../../ProjectSettings/ClusterInputManager.asset>) — 资源/工程文件。
- [ProjectSettings/CommonBurstAotSettings.json](<../../ProjectSettings/CommonBurstAotSettings.json>) — 配置/聚合资源。
- [ProjectSettings/DynamicsManager.asset](<../../ProjectSettings/DynamicsManager.asset>) — 资源/工程文件。
- [ProjectSettings/EditorBuildSettings.asset](<../../ProjectSettings/EditorBuildSettings.asset>) — 资源/工程文件。
- [ProjectSettings/EditorSettings.asset](<../../ProjectSettings/EditorSettings.asset>) — 资源/工程文件。
- [ProjectSettings/GraphicsSettings.asset](<../../ProjectSettings/GraphicsSettings.asset>) — 资源/工程文件。
- [ProjectSettings/InputManager.asset](<../../ProjectSettings/InputManager.asset>) — 资源/工程文件。
- [ProjectSettings/MemorySettings.asset](<../../ProjectSettings/MemorySettings.asset>) — 资源/工程文件。
- [ProjectSettings/NavMeshAreas.asset](<../../ProjectSettings/NavMeshAreas.asset>) — 资源/工程文件。
- [ProjectSettings/NotificationsSettings.asset](<../../ProjectSettings/NotificationsSettings.asset>) — 资源/工程文件。
- [ProjectSettings/PackageManagerSettings.asset](<../../ProjectSettings/PackageManagerSettings.asset>) — 资源/工程文件。
- [ProjectSettings/Packages/com.unity.learn.iet-framework/Settings.json](<../../ProjectSettings/Packages/com.unity.learn.iet-framework/Settings.json>) — 配置/聚合资源。
- [ProjectSettings/Physics2DSettings.asset](<../../ProjectSettings/Physics2DSettings.asset>) — 资源/工程文件。
- [ProjectSettings/PresetManager.asset](<../../ProjectSettings/PresetManager.asset>) — 资源/工程文件。
- [ProjectSettings/ProjectSettings.asset](<../../ProjectSettings/ProjectSettings.asset>) — 资源/工程文件。
- [ProjectSettings/ProjectVersion.txt](<../../ProjectSettings/ProjectVersion.txt>) — 资源/工程文件。
- [ProjectSettings/QualitySettings.asset](<../../ProjectSettings/QualitySettings.asset>) — 资源/工程文件。
- [ProjectSettings/SceneTemplateSettings.json](<../../ProjectSettings/SceneTemplateSettings.json>) — 配置/聚合资源。
- [ProjectSettings/ShaderGraphSettings.asset](<../../ProjectSettings/ShaderGraphSettings.asset>) — 资源/工程文件。
- [ProjectSettings/TagManager.asset](<../../ProjectSettings/TagManager.asset>) — 资源/工程文件。
- [ProjectSettings/TimeManager.asset](<../../ProjectSettings/TimeManager.asset>) — 资源/工程文件。
- [ProjectSettings/TimelineSettings.asset](<../../ProjectSettings/TimelineSettings.asset>) — 资源/工程文件。
- [ProjectSettings/URPProjectSettings.asset](<../../ProjectSettings/URPProjectSettings.asset>) — 资源/工程文件。
- [ProjectSettings/UnityConnectSettings.asset](<../../ProjectSettings/UnityConnectSettings.asset>) — 资源/工程文件。
- [ProjectSettings/VFXManager.asset](<../../ProjectSettings/VFXManager.asset>) — 资源/工程文件。
- [ProjectSettings/VersionControlSettings.asset](<../../ProjectSettings/VersionControlSettings.asset>) — 资源/工程文件。
- [ProjectSettings/XRSettings.asset](<../../ProjectSettings/XRSettings.asset>) — 资源/工程文件。
- [README.md](<../../README.md>) — 资源/工程文件。

## 验证

初始化时静态核对版本、包、构建场景、脚本引用、资源目录和 .gitignore，未发现自有 `*Test*.cs` / asmdef 测试代码；依赖 Unity Test Framework 1.1.33，根目录 Tests.csproj 不代表可用测试套件。

2026-10-28 Tilemap 溢出修复：Unity 2022.3.62f3 已自动解析嵌入包并完成编辑器及测试程序集编译。与安装目录原始包逐文件比对，仅上述实现和测试两个文件内容改变，原始 `.meta` 保持一致。通过 Unity TestRunnerApi 在 EditMode 运行 `TilemapPackageEditorTests`，12/12 通过（1 个原有包加载用例 + 11 个新增坐标用例：X/Y 最小值、最大值、双向跨界、极值邻近、小步移动、500/501 限幅及不动），0 失败、0 跳过；临时验证脚本及其 `.meta` 已移除。坐标限幅提取为内部纯函数，测试不创建窗口或依赖 `Event.current`。本次未进入 Play Mode 或进行设备构建。

后续验证入口：使用指定 Unity 打开项目、确认第三方资源和包完整，等待导入/编译；从 Boot 测试首启和恢复，再按受影响模块验证。Android / iOS 的通知、相册和后台存档必须在对应设备验证。

## 最近改动摘要

- 2026-10-28：L0，更新 Room 的 AI 供应商使用说明；AI 接入未调整布局、组件接线或依赖包，并行的其他工作区改动保留。
- 2026-10-28：经用户明确授权嵌入 Tilemap 1.0.0 并修复鼠标坐标差值溢出，新增边界回归测试、保留全部原始 GUID（L0，project 内部编辑器实现；无运行时契约或架构变化，无 ADR）。
- 2026-10-28：初始化工程和资源盘点（L3 文档建档），未改包依赖、场景或 prefab。
