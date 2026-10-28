# 跨模块约定

最后同步：commit `00d29af1871434383c29e34a13459b3534ee77ee`；2026-10-28；包含工作区未提交文档，业务实现未改。

## 阅读与维护

从 [INDEX](INDEX.md) 定位模块，按 [操作手册](OPERATING_MANUAL.md) 检查所有归属路径的已提交、工作区及未跟踪变更。当前文档模块不是程序集边界；以下区分现状与后续约束，不能将改进建议当作已实现。

## 现有结构与依赖

- 自有运行时脚本处于全局命名空间，使用 Unity 默认 `Assembly-CSharp`；`Assets/Editor` 使用默认 Editor 程序集。未发现自有 asmdef。
- MonoBehaviour Manager / UI 通过序列化引用、静态 Instance 和 C# 事件连接。模型已有直接访问 GameManager / SaveManager / UI 的情况，不能声称模型完全独立。
- 启动和存档入口见 [runtime](modules/runtime.md)、[storage](modules/storage.md)；房间数据和视图边界见 [room](modules/room.md)；外观见 [appearance](modules/appearance.md)。消费者与事件以对应模块文档为准。

## 公开契约

- `OperationResult` / `OperationResult<T>` 用 `Success`、`Message`、`Data` 表达业务结果；返回成功可能仅表示异步流程已启动，例如 AI 对话，不能等同完成。
- 状态字符串包括 `Full`、`Clean`、`San`、`Love`、`Money`，其余走 CustomStates。`GameManager.SetValueOfState` 实际应用增量；`GetCustomState` 缺失返回 `int.MinValue`。
- `OnCharacterStateChanged(string,int)` 的普通 Change 方法发当前值，但部分健康转换分支发固定值；UI 当前重新读取模型。不要仅根据注释把第二参数当成统一增量或统一当前值。
- 任务月份键 `yyyy-MM`，日期键 `yyyy-MM-dd`；实际时间来自本地 `DateTime.Now` / Today；不假定 UTC。时间事件的实际频率见 [runtime](modules/runtime.md)。
- JSON 标识、节点 `$type`、枚举名、Resources 路径、图集部件名、序列化字段和 Unity GUID 都是兼容性契约；修改时检查真实消费者及旧存档。

## 数据与异步约束

- 配置源和持久化路径以 [storage](modules/storage.md) 为准；修改分文件不代表运行时聚合文件已经更新。
- 集合多为可变共享引用，不保证深拷贝或线程安全。现有存档将共享模型交给后台序列化；新增异步逻辑须明确快照、写入顺序及失败反馈。
- 当前为 Task + Coroutine + DOTween，未安装 UniTask。采用策略见 [ADR 0002](adr/0002-async-strategy.md)；UniTask 仍使用 `async/await`，不得同时要求 UniTask 并禁止该语法。
- 新增或维护异步流程须明确取消与生命周期、异常处理、请求超时；Unity API 仅在主线程调用。静态事件订阅与退订须成对，需保留委托实例。

## 资源和验证

- 默认不改 Excel、prefab、第三方资源；用户明确要求时按授权范围执行。场景/资源接线不明先查 YAML 或 Unity，再对未解决的问题提问。
- 保留源文件编码与现有风格。仓库源码存在不同编码；不要为小改全文件转码。文档使用 UTF-8。
- 通常从 Boot 验证，以保证跨场景单例和状态已初始化。不要用现有 Library 或导出的 `.csproj` 作为干净检出可编译的证据。
- 文档静态核对不替代 Unity 编译、Play Mode、存档往返或 Android / iOS 真机通知测试。只报告实际执行过的检查。
