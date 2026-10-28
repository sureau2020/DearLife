# 对话节点与 AI

## 目录/程序集

Model/DialogueSystem、DialogueManager、AIChat、Choice UI、DialoguePanelUI；默认 Assembly-CSharp。

## 命名空间

全局命名空间。

## 最后同步

commit `00d29af1871434383c29e34a13459b3534ee77ee`；2026-10-28；包含工作区未提交文档；业务实现未改。

## 职责

EventDataBase 按 ID 和类型保存事件并按角色标签筛选日常候选；DialogueRunner 执行节点，DialogueManager 协调面板、按钮及随机事件。AIChat 以 Gemini 请求生成字符串数组，转换为一串 DialogueNode 后交给相同执行器。

## 公开 API

- `DialogueManager.StartDialogue(string)` / `StartDialogue(EventData)`、`StartRandomDailyDialogue()`、`ShowRandomItemDialogue(int,string)`、`ShowAIItemDialogue(int,string,CharacterData)`、`HandleAdvance(string)`。
- `DialogueRunner.StartEvent(EventData)`、`OnClickNext(string)`；`BaseNode.Execute()` → NodeExecResult。支持 Dialogue / Choice / Condition / Effect / Navigate 节点。
- EventDataBase：`AddEvent`、`GetEvent`、`GetCandidateDailyEventIds`、`GetAllEventIds`、`IsEventMatchCharacter(ByEventId)`。
- `AIChat.GenerateAIDialogue(ItemData,CharacterData,Action<OperationResult<EventData>>)`；`GenerateDialogue(string,Action<EventData>,Action<string>,float timeout = 10f)` 返回 IEnumerator。

## 公开接口契约

EventData 包含 EventId、Type、Tags、StartNodeId、Nodes 字典；Tags 必须全部匹配角色标签。节点 ID 必须存在；空下一节点结束；Condition / Effect 通过 GameManager 读写字符串状态；Navigate 替换当前事件，无调用栈返回。

Runner 同步循环推进自动节点，显示对话/选项后停下等待 UI。当前未做完整缺失节点与循环边保护。节点多态绑定由 [storage](storage.md) 两处绑定器维护。

物品先消耗和应用效果，再尝试显示对话；对话启动成功不保证 AI 请求成功。AIChat 直接请求 Gemini `generateContent`，读取 Settings.Key / Model / Prompt；API 字段当前不是通用供应商路由。超时协程按 Time.deltaTime 累计，默认 10 秒；没有贯穿调用链的 CancellationToken。

日常次数和日期用 PlayerPrefs 的 DailyEventShownCount / DailyEventDate；MaxDailyEvents 当前为 999，达到上限后按健康状态选默认事件。候选为空时可能返回空 ID，StartDialogue(null) 视作成功但不显示。

## 依赖

[runtime](runtime.md)、[character](character.md)、[presentation](presentation.md)、Newtonsoft / UnityWebRequest；配置格式的装载实现见 [storage](storage.md)。

## 被依赖

[runtime](runtime.md)、[storage](storage.md)、[character](character.md)。

## 事件

- Runner：`event Action<DialoguePayload,DialogueType> OnShowDialogue`、`event Action<DialogueType> StartDialogue`、`event Action<List<ChoiceOption>> OnShowChoices`、`event Action OnDialogueEnd` → DialogueManager。
- DialoguePanelUI：`event Action<string> OnNextClicked` → DialogueManager。
- ChoicesUI / ChoiceUI：`event Action<string> OnChoiceClicked`，ChoiceUI → ChoicesUI → DialogueManager。
- 消费 BottomButton.`event Action randomDailyEvent`；AIChat 用结果回调返回异步结果。

## 数据所有权

EventDataBase 为静态配置缓存；Runner 持有当前 EventData / 节点 ID；AI 事件临时构造，未自动写配置或存档。PlayerPrefs 计数独立于主状态 JSON。

## 文件清单

附属 `.meta` 同归属；JSON 文件在 [storage](storage.md)。
- [Assets/Scripts/GameObject/AIChat.cs](<../../Assets/Scripts/GameObject/AIChat.cs>) — 源码。
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

静态核对节点、事件、物品引用和 AI 接口形状；未调用付费/联网 AI，未运行对话 UI。后续修改需验证选项、条件、效果、事件跳转、空候选、网络超时与切场景取消。

## 最近改动摘要

- 2026-10-28：初始化对话/AI 文档（L3 文档建档），异步建议见 [ADR 0002](../adr/0002-async-strategy.md)。
