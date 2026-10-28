# ADR 0001：现有 Unity 单体与数据边界

- 状态：现状记录，未进行架构迁移。
- 日期：2026-10-28。
- 基线：commit `00d29af1871434383c29e34a13459b3534ee77ee`；包含工作区未提交文档。

## 背景

新加入的操作手册要求先初始化扫描、建立模块映射和依赖，再按变更范围增量维护。本仓库只有该手册，没有它链接的索引和模块文档；其中旧基线及删除状态来自参考工程，不适用于本仓库。

## 决策

以磁盘和 Git 基线为依据，为当前实现建立文档，不推断历史决策者或设计动机。自有运行时代码为 Unity 默认程序集、全局命名空间；Manager / UI / Model 有目录区分，但仍经静态单例、共享对象和事件耦合。文档模块按职责建立，不假装已拆程序集或达成严格解耦。

配置使用 StreamingAssets 聚合 JSON 和 Resources ScriptableObject；运行状态与用户数据写 persistentDataPath。保留场景 Boot / EditCharacter / Room 和现有 Task / Coroutine / DOTween。本次只改文档及 AGENTS.md。

## 备选方案

- 原样照搬 CRPG / 卡牌 / Luban / PC-Mac 示例：与现有工程证据不符，不采用。
- 初始化同时重构程序集、替换数据管线或迁移全部异步：超出了解项目与建立协作规则的范围，不采用。

## 后果

后续按路径找到模块、检查漂移并局部维护；已有耦合、存档与生命周期限制明确留在模块验证章节，待实际开发任务定向处理。全库重建仍需明确任务，不作为每次修改的默认步骤。

## 模块链接

[索引](../INDEX.md) · [runtime](../modules/runtime.md) · [storage](../modules/storage.md) · [room](../modules/room.md) · [project](../modules/project.md) · [约定](../conventions.md)
