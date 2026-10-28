# 对话节点与 AI

## 目录/程序集

Model/DialogueSystem、DialogueManager、AIChat、Choice UI、DialoguePanelUI；默认 Assembly-CSharp。

## 命名空间

全局命名空间。

## 最后同步

commit `49fa54da6e0cec37f33577c8593db375d1a21015`；2026-10-28；包含工作区未提交的 AI 供应商拆分、DeepSeek 接入与文档改动。

## 职责

EventDataBase 按 ID 和类型保存事件并按角色标签筛选日常候选；DialogueRunner 执行节点，DialogueManager 协调面板、按钮及随机事件。AIChat 负责提示词、请求生命周期和字符串数组到 DialogueNode 的转换；Gemini / DeepSeek 各自实现独立供应商，协议与模型列表不进入 AIChat。结构决策见 [ADR 0003](../adr/0003-ai-dialogue-providers.md)。

## 公开 API

- `DialogueManager.StartDialogue(string)` / `StartDialogue(EventData)`、`StartRandomDailyDialogue()`、`ShowRandomItemDialogue(int,string)`、`ShowAIItemDialogue(int,string,CharacterData)`、`HandleAdvance(string)`。
- `DialogueRunner.StartEvent(EventData)`、`OnClickNext(string)`；`BaseNode.Execute()` → NodeExecResult。支持 Dialogue / Choice / Condition / Effect / Navigate 节点。
- EventDataBase：`AddEvent`、`GetEvent`、`GetCandidateDailyEventIds`、`GetAllEventIds`、`IsEventMatchCharacter(ByEventId)`。
- `AIChat.GenerateAIDialogue(ItemData,CharacterData,Action<OperationResult<EventData>>)`；`GenerateDialogue(string,Action<EventData>,Action<string>,float timeout = 30f)` 返回 IEnumerator。
- `IAIDialogueProvider`：`Name`、只读 `Models`；`CreateRequest(string apiKey,string model,string systemPrompt,string userPrompt)` 返回由调用方发送/释放的 UnityWebRequest；`ReadResponseText(string responseJson)` 返回待解析的对话文本，格式错误或非正常结束抛出异常。
- `AIDialogueProviders.TryGet(string,out IAIDialogueProvider)`：忽略大小写、去首尾空格；当前注册 GeminiDialogueProvider / DeepSeekDialogueProvider，未知名称返回 false。供应商是无请求状态的共享实例。

## 公开接口契约

EventData 包含 EventId、Type、Tags、StartNodeId、Nodes 字典；Tags 必须全部匹配角色标签。节点 ID 必须存在；空下一节点结束；Condition / Effect 通过 GameManager 读写字符串状态；Navigate 替换当前事件，无调用栈返回。

Runner 同步循环推进自动节点，显示对话/选项后停下等待 UI。当前未做完整缺失节点与循环边保护。节点多态绑定由 [storage](storage.md) 两处绑定器维护。

物品先消耗和应用效果，再尝试显示对话；对话启动成功不保证 AI 请求成功。Settings.API 选择供应商，Key / Model / Prompt 沿用现有存档字段。未知供应商、空密钥/模型在发送前失败；切换供应商后需更新对应密钥并选择模型，共享字段不保存每家独立凭证。

GeminiDialogueProvider 保留 `generateContent` 原生格式，密钥置于 `x-goog-api-key`；合并非 thought 文本片段，要求 finishReason 为 STOP。模型选项沿用原场景清单，不表示重新核验这些预览模型的服务可用性。DeepSeekDialogueProvider 请求 `POST https://api.deepseek.com/chat/completions`，使用 Bearer 认证、system/user messages、stream=false、thinking.type=disabled、max_tokens=2000、temperature=0.7；读取 choices[0].message.content，要求 finish_reason 为 stop，不使用 reasoning_content。当前官方模型选项为 deepseek-flash / deepseek-v4-pro（核对日期 2026-10-28）。不用 json_object 模式，保持既有顶层 JSON 字符串数组约定。

AIChat 使用 realtimeSinceStartup 与 UnityWebRequest.timeout 限制请求，默认 30 秒，显式 timeout 参数继续生效。超时主动 Abort；组件禁用/场景销毁时终止释放全部在途请求、停止自身协程，取消后不回调失效 UI。仍使用协程，无 CancellationToken / UniTask。正常完成或失败调用一次对应回调；消费者回调异常不触发第二次失败回调。HTTP 认证、余额、限流、服务异常转为提示，不回显完整响应或密钥。

共用解析接受 JSON 字符串数组和完整 Markdown 围栏，拒绝空数组、空白行、null、非字符串或截断结果；成功后逐行构造节点。随机对话行数为 1–5，避免生成 0 行；提示词保留必需的 JSON 字符串引号。

### 接入与扩展

设置页先将 API 填为 `DeepSeek`，再填 DeepSeek 密钥，编辑模型时选择 `deepseek-flash` 或 `deepseek-v4-pro`，最后开启 AI。模型列表每次进入编辑时从当前供应商获取；切回 Gemini 同样需更新密钥和模型。实际回应仍受既有物品回应概率影响。

新增供应商：在 `Model/DialogueSystem/AI/` 增加 IAIDialogueProvider 实现，在 AIDialogueProviders 注册；请求格式、认证、响应解析及模型清单放在新实现内。更新本模块文档并验证异常、取消和模型选择，不在 AIChat / 设置 UI 增加供应商分支。

官方资料：[首次调用](https://api-docs.deepseek.com/)、[Chat Completions](https://api-docs.deepseek.com/api/create-chat-completion/)、[错误码](https://api-docs.deepseek.com/quick_start/error_codes/)、[Gemini 认证](https://ai.google.dev/api)。

日常次数和日期用 PlayerPrefs 的 DailyEventShownCount / DailyEventDate；MaxDailyEvents 当前为 999，达到上限后按健康状态选默认事件。候选为空时可能返回空 ID，StartDialogue(null) 视作成功但不显示。

## 依赖

[runtime](runtime.md)、[character](character.md)、[presentation](presentation.md)、Newtonsoft / UnityWebRequest；配置格式的装载实现见 [storage](storage.md)。

## 被依赖

[runtime](runtime.md)、[storage](storage.md)、[character](character.md)、[presentation](presentation.md)（模型下拉框读取供应商目录）。

## 事件

- Runner：`event Action<DialoguePayload,DialogueType> OnShowDialogue`、`event Action<DialogueType> StartDialogue`、`event Action<List<ChoiceOption>> OnShowChoices`、`event Action OnDialogueEnd` → DialogueManager。
- DialoguePanelUI：`event Action<string> OnNextClicked` → DialogueManager。
- ChoicesUI / ChoiceUI：`event Action<string> OnChoiceClicked`，ChoiceUI → ChoicesUI → DialogueManager。
- 消费 BottomButton.`event Action randomDailyEvent`；AIChat 用结果回调返回异步结果。

## 数据所有权

EventDataBase 为静态配置缓存；Runner 持有当前 EventData / 节点 ID；AI 事件临时构造，未自动写配置或存档。PlayerPrefs 计数独立于主状态 JSON。供应商持有只读模型清单，不保存密钥、角色或请求；AIChat 独占在途请求并负责释放。

## 文件清单

附属 `.meta` 同归属；JSON 文件在 [storage](storage.md)。
- [Assets/Scripts/GameObject/AIChat.cs](<../../Assets/Scripts/GameObject/AIChat.cs>) — 源码。
- [Assets/Scripts/Model/DialogueSystem/AI/IAIDialogueProvider.cs](<../../Assets/Scripts/Model/DialogueSystem/AI/IAIDialogueProvider.cs>) — 供应商契约。
- [Assets/Scripts/Model/DialogueSystem/AI/AIDialogueProviders.cs](<../../Assets/Scripts/Model/DialogueSystem/AI/AIDialogueProviders.cs>) — 供应商注册与查询。
- [Assets/Scripts/Model/DialogueSystem/AI/GeminiDialogueProvider.cs](<../../Assets/Scripts/Model/DialogueSystem/AI/GeminiDialogueProvider.cs>) — Gemini 协议及模型清单。
- [Assets/Scripts/Model/DialogueSystem/AI/DeepSeekDialogueProvider.cs](<../../Assets/Scripts/Model/DialogueSystem/AI/DeepSeekDialogueProvider.cs>) — DeepSeek 协议及模型清单。
- [Assets/Scripts/Manager/DialogueManager.cs](<../../Assets/Scripts/Manager/DialogueManager.cs>) — 源码。
- [Assets/Scripts/Model/DialogueSystem/DialogueRunner.cs](<../../Assets/Scripts/Model/DialogueSystem/DialogueRunner.cs>) — 源码。
- [Assets/Scripts/Model/DialogueSystem/EventData.cs](<../../Assets/Scripts/Model/DialogueSystem/EventData.cs>) — 源码。
- [Assets/Scripts/Model/DialogueSystem/EventDataBase.cs](<../../Assets/Scripts/Model/DialogueSystem/EventDataBase.cs>) — 源码。
- [Assets/Scripts/Model/DialogueSystem/Node/BaseNode.cs](<../../Assets/Scripts/Model/DialogueSystem/Node/BaseNode.cs>) — 源码。
- [Assets/Scripts/Model/DialogueSystem/Node/ChoiceNode.cs](<../../Assets/Scripts/Model/DialogueSystem/Node/ChoiceNode.cs>) — 源码。
- [Assets/Scripts/Model/DialogueSystem/Node/ConditionNode.cs](<../../Assets/Scripts/Model/DialogueSystem/Node/ConditionNode.cs>) — 源码。
- [Assets/Scripts/Model/DialogueSystem/Node/DialogueNode.cs](<../../Assets/Scripts/Model/DialogueSystem/Node/DialogueNode.cs>) — 源码。
- [Assets/Scripts/Model/DialogueSystem/Node/EffectNode.cs](<../../Assets/Scripts/Model/DialogueSystem/Node/EffectNode.cs>) — 源码。
- [Assets/Scripts/Model/DialogueSystem/Node/NavigateNode.cs](<../../Assets/Scripts/Model/DialogueSystem/Node/NavigateNode.cs>) — 源码。
- [Assets/Scripts/Model/DialogueSystem/NodeExecResult.cs](<../../Assets/Scripts/Model/DialogueSystem/NodeExecResult.cs>) — 源码。
- [Assets/Scripts/UI/Character/DialoguePanelUI.cs](<../../Assets/Scripts/UI/Character/DialoguePanelUI.cs>) — 源码。
- [Assets/Scripts/UI/Choice/ChoiceUI.cs](<../../Assets/Scripts/UI/Choice/ChoiceUI.cs>) — 源码。
- [Assets/Scripts/UI/Choice/ChoicesUI.cs](<../../Assets/Scripts/UI/Choice/ChoicesUI.cs>) — 源码。

## 验证

2026-10-28：使用本地 Unity 程序集及项目编译输入完成 C# 编译，0 error；依赖现有 Library 和第三方资源，不等同干净检出或 Unity Editor 编译。临时离线测试直接编译本次源码、模拟 Unity 网络与生命周期，40 项检查通过，覆盖供应商路由、鉴权/载荷、响应/思考内容隔离、非法数组、节点串联、HTTP 401/402/429/500/503、超时 Abort/Dispose、禁用后取消及回调次数。

未使用真实 API 密钥、未调用联网 AI，未运行 Unity Play Mode、设置 UI 或真机。仍需从 Boot 验证供应商切换、模型下拉框、实际赠送物品、断网/超时、时间缩放、禁用/切场景；离线模拟不替代这些检查。

## 最近改动摘要

- 2026-10-28：L2，AIChat 协议提取为独立供应商，实现 DeepSeek；设置 UI 消费模型目录，默认超时调整为 30 秒并补齐禁用取消；保留存档字段与节点格式，新增 ADR 0003。
- 2026-10-28：初始化对话/AI 文档（L3 文档建档），异步建议见 [ADR 0002](../adr/0002-async-strategy.md)。
