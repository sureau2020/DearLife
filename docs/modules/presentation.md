# 通用表现与交互

## 目录/程序集

索引未分配到业务模块的 UI / Manager / GameObject / Tools，OperationResult、Billboard、SetText、FX；默认 Assembly-CSharp。

## 命名空间

全局命名空间。PromptPanelUIManager.cs 实际声明类名为 PromptUIManager，不能根据文件名推断类名。

## 最后同步

commit `49fa54da6e0cec37f33577c8593db375d1a21015`；2026-10-28；包含工作区未提交的 AI 模型下拉框与文档改动。

## 职责

通用面板开关、UI 动画、顶部状态展示、设置输入、音效、图标、确认和错误提示。角色点击/触摸处理 UI 遮挡、聚焦相机、地面移动和点击特效；呼吸与触摸动画控制角色显示。具体任务/商店/外观/对话 UI 归对应业务模块。

## 公开 API

- `OperationResult.Complete()` / `Fail(string)`；泛型版本 `Complete(T)` / `Fail(string)`。
- `ConfirmRequestManager.Request(string,Action,Action = null)` → PromptUIManager → ConfirmPanel；`ErrorNotifier.NotifyError(string)` → PromptUIManager → ErrorUI。
- SoundManager：`PlaySfx(string)`；IconManager：`GetIcon(ItemType,string)`、`GetClothIcon(string)`、`GetMapIcon(string)`。
- Character：`ChangeShowingOfCharacterUI()`、`TouchCharacter()`、`CloseCharacterUI()`；CameraFocus：两个 `FocusOnTarget` 重载、`ResetCamera()`。
- CharacterBreatheComponent：`SetBreathing(bool)`、`SetBreatheParameters(float,float)`、`SetFlipDirection(int)`；CharacterTouchAnimation：`RecordEye()` / `ShowTouchAnimation()`。
- 通用 UI 的按钮/开关/输入字段通过 Unity 事件绑定；TopState 订阅模型事件并读取当前值，BackgroundFXManager 按健康状态显示遮罩。

## 公开接口契约

UI 不拥有业务状态；应通过既有业务入口修改。当前仍有 UI 直接改模型字段和静态单例的实现，不宣称已完成分层。Character 在编辑器走鼠标、移动平台走触摸分支；布置/衣柜会屏蔽部分角色交互。

SettingDropDown 编辑 Model 时根据 Settings.API 从 [dialogue](dialogue.md) 的 AIDialogueProviders 读取模型列表，并优先选中已保存模型；其他设置下拉框保持原选项。每次编辑重新获取，API 改动后无需重启面板。API 输入与 Key 继续使用原 SettingItem；切换供应商后需填写对应密钥并确认模型，没有每家独立凭证或自动切换已保存模型。

DOTween 驱动多处 UI 和角色动画，CameraFocus 仍使用协程；维护时需匹配对象关闭/销毁生命周期。PromptUIManager 的文件/类名不匹配属于现状，不在扫描中擅自改名，挂载状态需 Unity 验证。

## 依赖

[runtime](runtime.md)、[character](character.md)、[dialogue](dialogue.md)（供应商模型清单）、[room](room.md)、[appearance](appearance.md)，uGUI / TMP / DOTween / Unity 输入与音频。

## 被依赖

[runtime](runtime.md)、[storage](storage.md)、[character](character.md)、[tasks](tasks.md)、[dialogue](dialogue.md)、[room](room.md)、[appearance](appearance.md)。OperationResult 是跨模块基础结果类型；确认/错误提示为共享入口。

## 事件

- `ConfirmRequestManager.static Action<ConfirmData> OnRequestConfirm` → PromptUIManager；ConfirmData 包含 content、onConfirm、onCancel。
- `ErrorNotifier.static event Action<string> OnError` → PromptUIManager。
- `Character.static Action recordOpenEyes` → CharacterTouchAnimation。
- `BottomButton.event Action randomDailyEvent` → DialogueManager。
- 消费角色/金钱/健康事件、FurnishManager.onEnterFurnishMode、Closet.onClosetToggled、RoomManager.OnRoomManagerInitialized、GridMap.newCameraLimitMax；详见各来源模块。

## 数据所有权

本模块管理临时 UI / 动画 / 相机状态、展示对象和图标缓存；设置业务值属于 Character/GameSettings。静态事件是共享连接点；委托字段并非只可订阅的 event。

## 文件清单

附属 `.meta` 同归属；通用 prefab、材质和字体资源见 [project](project.md)。
- [Assets/Billboard.cs](<../../Assets/Billboard.cs>) — 源码。
- [Assets/FX/Prefabs/FX001_01.prefab](<../../Assets/FX/Prefabs/FX001_01.prefab>) — 资源/工程文件。
- [Assets/FX/Sprites/Smoke/FX001/Anim/Anim.anim](<../../Assets/FX/Sprites/Smoke/FX001/Anim/Anim.anim>) — 资源/工程文件。
- [Assets/FX/Sprites/Smoke/FX001/Anim/FX001_01.controller](<../../Assets/FX/Sprites/Smoke/FX001/Anim/FX001_01.controller>) — 资源/工程文件。
- [Assets/FX/Sprites/Smoke/FX001/FX001_01.png](<../../Assets/FX/Sprites/Smoke/FX001/FX001_01.png>) — 资源/工程文件。
- [Assets/FX/Sprites/Smoke/FX001/FX001_02.png](<../../Assets/FX/Sprites/Smoke/FX001/FX001_02.png>) — 资源/工程文件。
- [Assets/FX/Sprites/Smoke/FX001/FX001_03.png](<../../Assets/FX/Sprites/Smoke/FX001/FX001_03.png>) — 资源/工程文件。
- [Assets/FX/Sprites/Smoke/FX001/FX001_04.png](<../../Assets/FX/Sprites/Smoke/FX001/FX001_04.png>) — 资源/工程文件。
- [Assets/FX/Sprites/Smoke/FX001/FX001_05.png](<../../Assets/FX/Sprites/Smoke/FX001/FX001_05.png>) — 资源/工程文件。
- [Assets/Scripts/GameObject/CameraController.cs](<../../Assets/Scripts/GameObject/CameraController.cs>) — 源码。
- [Assets/Scripts/GameObject/CameraFocus.cs](<../../Assets/Scripts/GameObject/CameraFocus.cs>) — 源码。
- [Assets/Scripts/GameObject/Character.cs](<../../Assets/Scripts/GameObject/Character.cs>) — 源码。
- [Assets/Scripts/GameObject/CharacterBreatheComponent.cs](<../../Assets/Scripts/GameObject/CharacterBreatheComponent.cs>) — 源码。
- [Assets/Scripts/GameObject/CharacterTouchAnimation.cs](<../../Assets/Scripts/GameObject/CharacterTouchAnimation.cs>) — 源码。
- [Assets/Scripts/GameObject/Name.cs](<../../Assets/Scripts/GameObject/Name.cs>) — 源码。
- [Assets/Scripts/Manager/BackgroundFXManager.cs](<../../Assets/Scripts/Manager/BackgroundFXManager.cs>) — 源码。
- [Assets/Scripts/Manager/ConfirmRequestManager.cs](<../../Assets/Scripts/Manager/ConfirmRequestManager.cs>) — 源码。
- [Assets/Scripts/Manager/IconManager.cs](<../../Assets/Scripts/Manager/IconManager.cs>) — 源码。
- [Assets/Scripts/Manager/PromptPanelUIManager.cs](<../../Assets/Scripts/Manager/PromptPanelUIManager.cs>) — 源码。
- [Assets/Scripts/Manager/SoundManager.cs](<../../Assets/Scripts/Manager/SoundManager.cs>) — 源码。
- [Assets/Scripts/Model/OperationResult.cs](<../../Assets/Scripts/Model/OperationResult.cs>) — 源码。
- [Assets/Scripts/Tools/ErrorNotifier.cs](<../../Assets/Scripts/Tools/ErrorNotifier.cs>) — 源码。
- [Assets/Scripts/UI/Animation/AnimationWhenDisable.cs](<../../Assets/Scripts/UI/Animation/AnimationWhenDisable.cs>) — 源码。
- [Assets/Scripts/UI/Animation/AnimationWhenEnable.cs](<../../Assets/Scripts/UI/Animation/AnimationWhenEnable.cs>) — 源码。
- [Assets/Scripts/UI/Animation/MoveWhenClick.cs](<../../Assets/Scripts/UI/Animation/MoveWhenClick.cs>) — 源码。
- [Assets/Scripts/UI/BottomButton.cs](<../../Assets/Scripts/UI/BottomButton.cs>) — 源码。
- [Assets/Scripts/UI/Character/EditButton.cs](<../../Assets/Scripts/UI/Character/EditButton.cs>) — 源码。
- [Assets/Scripts/UI/Character/PanelChanger.cs](<../../Assets/Scripts/UI/Character/PanelChanger.cs>) — 源码。
- [Assets/Scripts/UI/ConfirmPanel.cs](<../../Assets/Scripts/UI/ConfirmPanel.cs>) — 源码。
- [Assets/Scripts/UI/ErrorUI.cs](<../../Assets/Scripts/UI/ErrorUI.cs>) — 源码。
- [Assets/Scripts/UI/Setting/BgmButton.cs](<../../Assets/Scripts/UI/Setting/BgmButton.cs>) — 源码。
- [Assets/Scripts/UI/Setting/ResetMoney.cs](<../../Assets/Scripts/UI/Setting/ResetMoney.cs>) — 源码。
- [Assets/Scripts/UI/Setting/RestMap.cs](<../../Assets/Scripts/UI/Setting/RestMap.cs>) — 源码。
- [Assets/Scripts/UI/Setting/SettingDropDown.cs](<../../Assets/Scripts/UI/Setting/SettingDropDown.cs>) — 源码。
- [Assets/Scripts/UI/Setting/SettingItem.cs](<../../Assets/Scripts/UI/Setting/SettingItem.cs>) — 源码。
- [Assets/Scripts/UI/Setting/SettingToogle.cs](<../../Assets/Scripts/UI/Setting/SettingToogle.cs>) — 源码。
- [Assets/Scripts/UI/TopState/StatusBar.cs](<../../Assets/Scripts/UI/TopState/StatusBar.cs>) — 源码。
- [Assets/Scripts/UI/TopState/TopState.cs](<../../Assets/Scripts/UI/TopState/TopState.cs>) — 源码。
- [Assets/Scripts/UI/Ttorial/PageChange.cs](<../../Assets/Scripts/UI/Ttorial/PageChange.cs>) — 源码。
- [Assets/SetText.cs](<../../Assets/SetText.cs>) — 源码。

## 验证

静态核对公开入口、订阅与调用方。Character 当前用两个独立 lambda 进行订阅/退订，不能视作有效退订；本次仅记录。未运行点击、相机、动画或音效；维护后需在 Unity 验证 UI 遮挡和平台输入。

2026-10-28：模型下拉框源码通过基于本地 Unity 程序集的 C# 编译；静态核对 Room 场景 Model 挂载为启用的 SettingDropDown（旧文本输入禁用），未修改挂载或 prefab。未运行 UI；需从 Boot 验证先改 API 为 DeepSeek、打开模型选择、保存、重开及切回 Gemini。

## 最近改动摘要

- 2026-10-28：L2，SettingDropDown 从独立 AI 供应商目录加载模型列表，保留设置入口和序列化引用。
- 2026-10-28：初始化通用表现模块（L3 文档建档），记录委托及文件名限制。
