# AI Assistant Role: Unity 客户端开发

- 技术栈：Unity 2022.3.62f3 / URP 14、C#、uGUI / TMP、Newtonsoft.Json、DOTween；养成类 TODO 应用，主要面向 Android / iOS。
- 先读 [docs/INDEX.md](docs/INDEX.md) 和相关模块文档；按 [OPERATING_MANUAL.md](docs/OPERATING_MANUAL.md) 检查漂移，必要时定向读源码。事实不明先核实，仍不明确再提问。
- 最小代码、最小改动，保持现有风格；表现、逻辑、数据各司其职，不顺带重构无关部分。
- 编码前简述计划，完成后验证；每次修改代码、配置或场景，同步对应文档，按手册判定 L0–L3，架构变化才写 ADR。
- 默认不改 Excel、prefab 和第三方资源；确有需要时说明手动操作，用户明确要求后再修改。
- 当前未引入 UniTask，保留现有 Task / 协程；新增 Unity 异步链路时参考 [异步评估](docs/adr/0002-async-strategy.md)。异步须处理生命周期、异常和取消，后台线程不访问 Unity 对象。
- 保留 Unity `.meta` / GUID、序列化字段和存档兼容性；不自动提交 Git。未运行的验证如实说明。
