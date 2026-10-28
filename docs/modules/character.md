# 角色、经济与物品

## 目录/程序集

Model/Character、Model/Item、PlayerData、GameSettings，商店/背包/角色信息 UI、Calculators；默认 Assembly-CSharp。

## 命名空间

全局命名空间。

## 最后同步

commit `00d29af1871434383c29e34a13459b3534ee77ee`；2026-10-28；包含工作区未提交文档；业务实现未改。

## 职责

CharacterData 维护饱食、清洁、精神、好感、健康状态、关系、外观与服装；PlayerData 维护金钱和背包。ItemDataBase 按 ID/类型提供物品；ItemData 的事件候选按角色标签过滤。UI 处理购买、使用、展示与重生。GameSettings 保存奖励参数及 AI 设置。

## 公开 API

- CharacterData：`ApplyItemEffect(string,int)`、`ApplyEffect(EffectType,int)`、`ChangeFull/Clean/San(int,bool)`、`ChangeLove(int,bool)`、`Rebirth()`、`ShowRebirth()`、健康/性格描述和关系计算。
- PlayerData：`BuyItem(int,ItemData,int)`、`BuyCloth(int,WardrobeSlot)`、`UseItem(string,int)`、`EarnMoney(int)`、`SpendMoney(int)`、`GetItemCount`、`GetTheItemInBag`、容量/余额检查。
- ItemDataBase：`AddItem`、`GetItemById`、`GetItemsByType`、`GetAllItems`；WardrobeData：`AddCloth`、`GetAllClothes`、公开静态 Slots。
- GameSettings：`Validate()`、各 `Change...`、`IsAISettingValid()`；ItemInfoManager：`ShowBuyPanel(ItemData)` / `HideBuyPanel()`。
- Calculators：`RandomSalary`、`EstimatedMinSalary`、`RandomChance`、`RandomEvent`。

## 公开接口契约

Full / Clean / San 普通变更截断至 0–100；Love 不按 100 截断。健康枚举 Normal / Weak / Crazy / Dirty / Dead 控制后续行为。玩家最多 8 种物品，Money 上限 99999999；新买/叠加物品移至背包前部。SpendMoney 要求调用方先检查余额。

常规奖励为 `(Duration * HourlyWage + Difficulty * DifficultyBonus) * 随机倍率` 后取整，任务逾期完成可获一半，协调逻辑见 [runtime](runtime.md)。`RandomChance` 掷 1–5；物品对话调用处 p <= 1 直接不显示，应同时查看 [dialogue](dialogue.md)。

健康状态与存档并非纯数据：CharacterData 构造/变更可能读取存档、访问 GameManager 和触发保存；公开 setter 可能绕过事件。买/用物品与自动保存不是原子事务。API 的操作顺序不可仅由方法名称推断。

## 依赖

[runtime](runtime.md)、[storage](storage.md)、[dialogue](dialogue.md)、[presentation](presentation.md)。

## 被依赖

[runtime](runtime.md)、[storage](storage.md)、[tasks](tasks.md)、[dialogue](dialogue.md)、[room](room.md)、[appearance](appearance.md)、[notifications](notifications.md)、[presentation](presentation.md)。

## 事件

- `CharacterData.event Action<string,int> OnCharacterStateChanged` → TopState。值含义例外见 [conventions](../conventions.md)。
- `CharacterData.event Action OnHealthChanged` → CharacterMoveAI、CharacterTileMoveAI、InfoUI、BackgroundFXManager、RebirthUI。
- `PlayerData.event Action<int> OnMoneyChanged` → TopState；参数为新余额。

## 数据所有权

PlayerData 拥有 Items；CharacterData 拥有角色状态并引用 CharacterAppearance / WardrobeSlot。衣柜槽在 [appearance](appearance.md) UI 与静态 WardrobeData 间共享；物品和事件 ID 是跨配置引用。GameSettings 与玩家和角色一同进入主存档。

## 文件清单

WardrobeData 虽由外观 UI 使用，声明与数据契约归本模块；附属 `.meta` 同归属。
- [Assets/Scripts/Manager/ItemInfoManager.cs](<../../Assets/Scripts/Manager/ItemInfoManager.cs>) — 源码。
- [Assets/Scripts/Model/Character/CharacterData.cs](<../../Assets/Scripts/Model/Character/CharacterData.cs>) — 源码。
- [Assets/Scripts/Model/Character/HealthState.cs](<../../Assets/Scripts/Model/Character/HealthState.cs>) — 源码。
- [Assets/Scripts/Model/Character/PersonalityType.cs](<../../Assets/Scripts/Model/Character/PersonalityType.cs>) — 源码。
- [Assets/Scripts/Model/GameSettings.cs](<../../Assets/Scripts/Model/GameSettings.cs>) — 源码。
- [Assets/Scripts/Model/Item/EffectType.cs](<../../Assets/Scripts/Model/Item/EffectType.cs>) — 源码。
- [Assets/Scripts/Model/Item/ItemData.cs](<../../Assets/Scripts/Model/Item/ItemData.cs>) — 源码。
- [Assets/Scripts/Model/Item/ItemDataBase.cs](<../../Assets/Scripts/Model/Item/ItemDataBase.cs>) — 源码。
- [Assets/Scripts/Model/Item/ItemEntryData.cs](<../../Assets/Scripts/Model/Item/ItemEntryData.cs>) — 源码。
- [Assets/Scripts/Model/Item/ItemType.cs](<../../Assets/Scripts/Model/Item/ItemType.cs>) — 源码。
- [Assets/Scripts/Model/Item/WardrobeData.cs](<../../Assets/Scripts/Model/Item/WardrobeData.cs>) — 源码。
- [Assets/Scripts/Model/PlayerData.cs](<../../Assets/Scripts/Model/PlayerData.cs>) — 源码。
- [Assets/Scripts/Tools/Calculators.cs](<../../Assets/Scripts/Tools/Calculators.cs>) — 源码。
- [Assets/Scripts/UI/Character/BackPackUI.cs](<../../Assets/Scripts/UI/Character/BackPackUI.cs>) — 源码。
- [Assets/Scripts/UI/Character/InfoUI.cs](<../../Assets/Scripts/UI/Character/InfoUI.cs>) — 源码。
- [Assets/Scripts/UI/Character/ItemUI.cs](<../../Assets/Scripts/UI/Character/ItemUI.cs>) — 源码。
- [Assets/Scripts/UI/RebirthUI.cs](<../../Assets/Scripts/UI/RebirthUI.cs>) — 源码。
- [Assets/Scripts/UI/Store/Goods.cs](<../../Assets/Scripts/UI/Store/Goods.cs>) — 源码。
- [Assets/Scripts/UI/Store/ItemInfoPanel.cs](<../../Assets/Scripts/UI/Store/ItemInfoPanel.cs>) — 源码。
- [Assets/Scripts/UI/Store/ItemList.cs](<../../Assets/Scripts/UI/Store/ItemList.cs>) — 源码。
- [Assets/Scripts/UI/Store/StoreCategoryButton.cs](<../../Assets/Scripts/UI/Store/StoreCategoryButton.cs>) — 源码。
- [Assets/Scripts/UI/Store/UISlider.cs](<../../Assets/Scripts/UI/Store/UISlider.cs>) — 源码。

## 验证

核对状态变更、经济入口、事件订阅及配置引用。未运行 Unity；后续行为修改需验证容量、余额、重复完成、逾期奖励、使用物品后存档恢复、死亡/重生 UI。

## 最近改动摘要

- 2026-10-28：初始化角色与物品模块文档（L3 文档建档），无业务变更。
