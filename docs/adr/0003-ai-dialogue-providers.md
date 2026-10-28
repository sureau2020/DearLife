# ADR 0003：独立 AI 对话供应商

- 状态：已采用。
- 日期：2026-10-28。
- 基线：commit `49fa54da6e0cec37f33577c8593db375d1a21015`；包含工作区未提交实现与文档。

## 背景

原 AIChat 同时拼接角色提示词、构造 Gemini 协议、发送请求和构建对话节点。Settings 已保存 API 字段但请求忽略它；模型下拉框只有场景序列化的 Gemini 清单。此次需要接入 DeepSeek，用户要求与 Gemini 独立，并支持以后扩展。

## 决策

在既有 dialogue 模块和默认 Assembly-CSharp 内引入 IAIDialogueProvider；独立 GeminiDialogueProvider / DeepSeekDialogueProvider 各自持有模型列表、构造请求及解析响应。AIDialogueProviders 集中注册并按名称查询无状态实例，不新增程序集、模块或外部 SDK。

AIChat 只依赖接口，负责提示词、协程、超时/取消、网络失败提示、共享 JSON 数组校验及节点构造；请求所有权归 AIChat，禁用时终止释放，不回调失效 UI。延续 [ADR 0002](0002-async-strategy.md) 的现有协程策略。

设置 UI 从供应商读取模型清单，仍通过原 GameManager.ChangeSetting 保存。GameSettings 与旧存档字段不变，默认仍为 Gemini。API / Key / Model 是共享的一组设置，切换后用户填写相应凭证和模型，不引入多供应商凭证迁移。

新增供应商只增加接口实现及注册项；增加同供应商模型只改其模型列表。供应商不得保存请求、密钥、角色或 UI 状态。

## 备选方案

- 在 AIChat 中增加 Gemini / DeepSeek 条件分支：协议继续耦合，不满足此次独立性要求。
- 完整的多供应商账户配置、异步库迁移、流式输出和插件系统：超出当前物品短对话需求，暂不引入。

## 后果与验证

表现模块新增对 dialogue 供应商目录的直接依赖。列表与协议维护于同一实现；AIChat 不需要了解特定供应商。供应商可使用不同格式，但必须返回供共享解析器处理的 JSON 字符串数组文本；非正常结束应抛出异常，不将思考内容作为角色台词。

默认请求超时由 10 秒调整为 30 秒；组件禁用时取消不回调。原 AIChat 内未被使用的公开 Gemini 响应嵌套类移出，不改变 Unity 序列化字段、GUID、事件格式或存档。已搜索自有源码，无这些嵌套类型的外部消费者。

已完成本地 C# 编译和 40 项模拟环境离线检查，未进行真实 API、Unity Play Mode 或移动端验证。后续新增实现需验证载荷、响应、失败、取消和设置选择。

## 模块与官方资料

[dialogue](../modules/dialogue.md) · [presentation](../modules/presentation.md) · [character](../modules/character.md) · [DeepSeek 首次调用](https://api-docs.deepseek.com/) · [Chat Completions](https://api-docs.deepseek.com/api/create-chat-completion/)
