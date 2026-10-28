# ADR 0002：UniTask 评估与渐进采用

- 状态：评估完成；当前暂不引入 UniTask，未来按具体需求复评。
- 日期：2026-10-28。
- 基线：commit `00d29af1871434383c29e34a13459b3534ee77ee`；包含工作区未提交文档；依赖和业务代码未改。

## 背景

本工程未安装 UniTask。已有 System.Threading.Tasks.Task 用于存档序列化/文件写入，Coroutine 用于配置加载、AI 请求、移动、镜头及 iOS 授权，DOTween 用于动画。主要需要明确生命周期、异常观察与保存顺序；仅更换返回类型不会自动解决共享可变数据或同步/异步写入竞争。

用户示例提出“强制 UniTask，禁止 async/await”，两者矛盾：UniTask 本身使用 async/await，仅将 Task / Task<T> 换成 Unity 适用的任务类型。

## 决策

现在不为文档初始化引入依赖或批量迁移。推荐将 UniTask 作为未来新增/改造 AI 请求、异步加载、异步切场景时的可选统一工具；现有运行方式可以继续使用。是否安装及锁定哪个版本，在对应实现任务中确定并验证。

采用后，Unity 层新增异步链路优先 UniTask，传递与对象/场景或业务取消绑定的 CancellationToken，明确请求超时和失败处理；返回主线程后再操作 Unity 对象。底层文件 I/O 仍可保持 Task，不强制改成 UniTask；DOTween 动画保留，只有需要 await 动画时再配置其 UniTask 适配。

不全局禁止 Coroutine，也不禁止 async/await；不以更换异步库替代必要的数据快照、并发控制和退出保存方案。

## 备选方案

- 保留现有异步方式并加强取消/异常处理：当前成本最低，作为现阶段选择。
- 立刻全量换 UniTask：涉及存档公开 API、加载和通知生命周期，收益尚无性能证据，改动范围过大，不采用。
- 完全禁止 Task / Coroutine：会使当前实现和第三方调用边界不必要地复杂，不采用。

## 后果与后续验证

AGENTS.md 如实描述当前状态；若后续引入 UniTask，按 L2/L3 实际契约影响同步调用方、包锁和本 ADR 的替代记录。验证 Unity 2022.3 编译、AI 超时/取消、切场景后不回调失效对象、移动端启动/后台恢复；不把 async 换写本身当成功指标。

## 参考

[Cysharp/UniTask 官方仓库与 README](https://github.com/Cysharp/UniTask)：Unity AsyncOperation / UnityWebRequest 的 await 支持、PlayerLoop、生命周期取消、Task 互操作与 DOTween 可选适配。兼容性与 API 应以未来选定的版本为准。

## 模块链接

[storage](../modules/storage.md) · [dialogue](../modules/dialogue.md) · [runtime](../modules/runtime.md) · [room](../modules/room.md) · [notifications](../modules/notifications.md) · [presentation](../modules/presentation.md) · [约定](../conventions.md)
