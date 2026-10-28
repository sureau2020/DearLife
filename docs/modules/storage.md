# JSON 配置与持久化

## 目录/程序集

SaveManager、LoaderManager、StreamingAssets、GameData；默认 Assembly-CSharp，Python 合并脚本不参与 Unity 编译。

## 命名空间

C# 为全局命名空间。

## 最后同步

commit `00d29af1871434383c29e34a13459b3534ee77ee`；2026-10-28；包含工作区未提交文档；业务源码与配置未改。

## 职责

LoaderManager.LoadAllData 依次加载运行时聚合事件表、衣柜、聚合物品表。事件/物品在 Android / iOS 使用 UnityWebRequest，其他分支直接读文件；衣柜初始化回退分支只在 Android 使用请求。

SaveManager 保存主状态、任务、衣柜、自定义衣物图片、GridMap，也提供旧的事件/物品导出入口。`Assets/GameData/` 当前无非 meta 文件，不能将旧导出目录当运行时配置入口。

## 公开 API

- `LoaderManager.LoadAllData()` / `LoadWardrobeFromSave()` 返回 IEnumerator；`LoadGridMap()` 返回 `OperationResult<GridMap>`。
- SaveManager：主状态 `SaveStateManager` / `SaveStateManagerAsync` / `LoadStateManager`；月份 `SaveMonthTasks` / `SaveMonthTasksAsync` / `LoadMonthTasks` / `SaveAllMonthTasks` / `SaveAllMonthTasksAsync`；例行任务 `SaveRecurringMissionsAsync` / `LoadRecurringMissions`。
- 衣柜 `SaveWardrobe` / `LoadWardrobe`、自定义衣物 `SaveCustomClothSprite` / `LoadCustomClothSprite`；地图 `SaveGridMap`。
- 作者工具遗留入口 `SaveEvent` / `SaveItem` / `ExportAllEvents` / `ExportAllItems` 写 Assets/GameData。

## 公开接口契约

| 数据 | 运行时路径（相对 Application 对应根目录） |
|---|---|
| 事件、物品 | streamingAssetsPath 下 `GameData/all_events_list.json`、`GameData/all_items_list.json` |
| 初始衣柜 | streamingAssetsPath 下 `GameData/clothes.json` |
| 主状态、衣柜、地图 | persistentDataPath 下 `SaveData/StateManager.json`、`SaveData/Wardrobe.json`、`SaveData/GridMap.json` |
| 月任务、例行任务 | persistentDataPath 下 `TaskData/month_yyyy-MM.json`、`TaskData/recurring_missions.json` |
| 自定义服装图片 | persistentDataPath 下 `CustomClothes/` |

JSON 使用 Newtonsoft、TypeNameHandling.Auto、字符串枚举、ReferenceLoopHandling.Ignore；绑定器识别 DialogueNode / ChoiceNode / ConditionNode / EffectNode / NavigateNode。新增节点须同时维护两个绑定器及配置消费者。

异步主状态写入用 SemaphoreSlim 排队；同步写入不使用这把锁。异步序列化引用共享模型，未生成深拷贝快照；月/例行写入无同等排队。主状态/地图先写 `.tmp` 再删除旧文件并移动，不能承诺完整事务或崩溃安全。UI 多处 `_ = Save...Async()`，结果可能被忽略。

地图存档记录格子、实例、相机上界和 ID 计数器；载入反射访问 GridMap 私有字段及属性。字段重命名、构造器初始化副作用都需检查兼容性；CreateEmptyGridMap 实际仍调用默认构造器后替换字段。

## 依赖

[runtime](runtime.md)、[character](character.md)、[tasks](tasks.md)、[dialogue](dialogue.md)、[room](room.md)、[presentation](presentation.md)，以及 Newtonsoft / UnityWebRequest。

## 被依赖

[runtime](runtime.md)、[character](character.md)、[tasks](tasks.md)、[room](room.md)、[appearance](appearance.md)。

## 事件

无模块公开事件；协程完成顺序是配置初始化契约。

## 数据所有权

静态 ItemDataBase / EventDataBase / WardrobeData 持有运行时配置；用户存档由各模型拥有，SaveManager 负责序列化而非业务修改。家具自定义 JSON 由 [room](room.md) 内 FurnitureDatabase 单独管理。

## 文件清单

聚合表和索引属于提交的派生/聚合资源，未发现 Luban 生成代码。分文件和聚合表均纳入清单；附属 `.meta` 同归属。
- [Assets/Scripts/Manager/Persistence/LoaderManager.cs](<../../Assets/Scripts/Manager/Persistence/LoaderManager.cs>) — 源码。
- [Assets/Scripts/Manager/Persistence/SaveManager.cs](<../../Assets/Scripts/Manager/Persistence/SaveManager.cs>) — 源码。
- [Assets/StreamingAssets/GameData/Events/12345_item_icecream_001.json](<../../Assets/StreamingAssets/GameData/Events/12345_item_icecream_001.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/12345_item_icecream_002.json](<../../Assets/StreamingAssets/GameData/Events/12345_item_icecream_002.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/12345_item_icecream_003.json](<../../Assets/StreamingAssets/GameData/Events/12345_item_icecream_003.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/12345_item_icecream_004.json](<../../Assets/StreamingAssets/GameData/Events/12345_item_icecream_004.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/12345_item_icecream_005.json](<../../Assets/StreamingAssets/GameData/Events/12345_item_icecream_005.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/12345_item_orange_001.json](<../../Assets/StreamingAssets/GameData/Events/12345_item_orange_001.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/12345_item_orange_002.json](<../../Assets/StreamingAssets/GameData/Events/12345_item_orange_002.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/12345_item_orange_003.json](<../../Assets/StreamingAssets/GameData/Events/12345_item_orange_003.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/12345_item_orangejuice_001.json](<../../Assets/StreamingAssets/GameData/Events/12345_item_orangejuice_001.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/12345_item_orangejuice_002.json](<../../Assets/StreamingAssets/GameData/Events/12345_item_orangejuice_002.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/12345_item_pear_001.json](<../../Assets/StreamingAssets/GameData/Events/12345_item_pear_001.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/12345_item_pizza_001.json](<../../Assets/StreamingAssets/GameData/Events/12345_item_pizza_001.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/12345_item_popcorn_001.json](<../../Assets/StreamingAssets/GameData/Events/12345_item_popcorn_001.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/12345_item_ramen_001.json](<../../Assets/StreamingAssets/GameData/Events/12345_item_ramen_001.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/12345_item_rice_001.json](<../../Assets/StreamingAssets/GameData/Events/12345_item_rice_001.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/12345_item_vegesushi_001.json](<../../Assets/StreamingAssets/GameData/Events/12345_item_vegesushi_001.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/12345_item_watermelon_001.json](<../../Assets/StreamingAssets/GameData/Events/12345_item_watermelon_001.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/KAGE_daily_001.json](<../../Assets/StreamingAssets/GameData/Events/KAGE_daily_001.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/KAGE_daily_F03.json](<../../Assets/StreamingAssets/GameData/Events/KAGE_daily_F03.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_B01.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_B01.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_B02.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_B02.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_B03.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_B03.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_B04.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_B04.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_B05.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_B05.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_B06.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_B06.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_B07.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_B07.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_B08.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_B08.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_B09.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_B09.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_B10.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_B10.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_B11.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_B11.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_B12.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_B12.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_B13.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_B13.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_B14.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_B14.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_B15.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_B15.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_B16.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_B16.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_C01.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_C01.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_C02.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_C02.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_C03.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_C03.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_D01.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_D01.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_D02.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_D02.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_D03.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_D03.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_E01.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_E01.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_E02.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_E02.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_E03.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_E03.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_F01.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_F01.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_F02.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_F02.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_G01.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_G01.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_G02.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_G02.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_G03.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_G03.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_G04.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_G04.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_H01.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_H01.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_H02.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_H02.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_H03.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_H03.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_H04.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_H04.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_I01.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_I01.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_I02.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_I02.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_I03.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_I03.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_crazy_default_1.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_crazy_default_1.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_dirty_default_1.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_dirty_default_1.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_normal_default_1.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_normal_default_1.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/Kage_daily_weak_default_1.json](<../../Assets/StreamingAssets/GameData/Events/Kage_daily_weak_default_1.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/merge.py](<../../Assets/StreamingAssets/GameData/Events/merge.py>) — 合并工具（未执行）。
- [Assets/StreamingAssets/GameData/Events/yinxing_item_apple_001.json](<../../Assets/StreamingAssets/GameData/Events/yinxing_item_apple_001.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/yinxing_item_book_001.json](<../../Assets/StreamingAssets/GameData/Events/yinxing_item_book_001.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/yinxing_item_burger_001.json](<../../Assets/StreamingAssets/GameData/Events/yinxing_item_burger_001.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/yinxing_item_chocomilk_001.json](<../../Assets/StreamingAssets/GameData/Events/yinxing_item_chocomilk_001.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/yinxing_item_chocomilk_002.json](<../../Assets/StreamingAssets/GameData/Events/yinxing_item_chocomilk_002.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/yinxing_item_drawing_001.json](<../../Assets/StreamingAssets/GameData/Events/yinxing_item_drawing_001.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/yinxing_item_drawing_002.json](<../../Assets/StreamingAssets/GameData/Events/yinxing_item_drawing_002.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/yinxing_item_fish_001.json](<../../Assets/StreamingAssets/GameData/Events/yinxing_item_fish_001.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/yinxing_item_fish_002.json](<../../Assets/StreamingAssets/GameData/Events/yinxing_item_fish_002.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/yinxing_item_fish_003.json](<../../Assets/StreamingAssets/GameData/Events/yinxing_item_fish_003.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/yinxing_item_general_001.json](<../../Assets/StreamingAssets/GameData/Events/yinxing_item_general_001.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/yinxing_item_general_002.json](<../../Assets/StreamingAssets/GameData/Events/yinxing_item_general_002.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/yinxing_item_generalsilence_001.json](<../../Assets/StreamingAssets/GameData/Events/yinxing_item_generalsilence_001.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/yinxing_item_milktea_001.json](<../../Assets/StreamingAssets/GameData/Events/yinxing_item_milktea_001.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/yinxing_item_milktea_002.json](<../../Assets/StreamingAssets/GameData/Events/yinxing_item_milktea_002.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/yinxing_item_milktea_003.json](<../../Assets/StreamingAssets/GameData/Events/yinxing_item_milktea_003.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/yinxing_item_miqingji_001.json](<../../Assets/StreamingAssets/GameData/Events/yinxing_item_miqingji_001.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/yinxing_item_miqingji_002.json](<../../Assets/StreamingAssets/GameData/Events/yinxing_item_miqingji_002.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/yinxing_item_miqingji_003.json](<../../Assets/StreamingAssets/GameData/Events/yinxing_item_miqingji_003.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/yinxing_item_news_001.json](<../../Assets/StreamingAssets/GameData/Events/yinxing_item_news_001.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/yinxing_item_news_002.json](<../../Assets/StreamingAssets/GameData/Events/yinxing_item_news_002.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/yinxing_item_yingyangye_001.json](<../../Assets/StreamingAssets/GameData/Events/yinxing_item_yingyangye_001.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/yinxing_item_yingyangye_002.json](<../../Assets/StreamingAssets/GameData/Events/yinxing_item_yingyangye_002.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Events/yinxing_item_yingyangye_003.json](<../../Assets/StreamingAssets/GameData/Events/yinxing_item_yingyangye_003.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/12345_fries.json](<../../Assets/StreamingAssets/GameData/Items/12345_fries.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/12345_icecream.json](<../../Assets/StreamingAssets/GameData/Items/12345_icecream.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/12345_orange.json](<../../Assets/StreamingAssets/GameData/Items/12345_orange.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/12345_orangejuice.json](<../../Assets/StreamingAssets/GameData/Items/12345_orangejuice.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/12345_pear.json](<../../Assets/StreamingAssets/GameData/Items/12345_pear.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/12345_pizza.json](<../../Assets/StreamingAssets/GameData/Items/12345_pizza.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/12345_popcorn.json](<../../Assets/StreamingAssets/GameData/Items/12345_popcorn.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/12345_ramen.json](<../../Assets/StreamingAssets/GameData/Items/12345_ramen.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/12345_rice.json](<../../Assets/StreamingAssets/GameData/Items/12345_rice.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/12345_salad.json](<../../Assets/StreamingAssets/GameData/Items/12345_salad.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/12345_sandwitch.json](<../../Assets/StreamingAssets/GameData/Items/12345_sandwitch.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/12345_soup.json](<../../Assets/StreamingAssets/GameData/Items/12345_soup.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/12345_vegesushi.json](<../../Assets/StreamingAssets/GameData/Items/12345_vegesushi.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/12345_watermelon.json](<../../Assets/StreamingAssets/GameData/Items/12345_watermelon.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/yinxing_apple.json](<../../Assets/StreamingAssets/GameData/Items/yinxing_apple.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/yinxing_bodylotion.json](<../../Assets/StreamingAssets/GameData/Items/yinxing_bodylotion.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/yinxing_book.json](<../../Assets/StreamingAssets/GameData/Items/yinxing_book.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/yinxing_burger.json](<../../Assets/StreamingAssets/GameData/Items/yinxing_burger.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/yinxing_chicken.json](<../../Assets/StreamingAssets/GameData/Items/yinxing_chicken.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/yinxing_chocomilk.json](<../../Assets/StreamingAssets/GameData/Items/yinxing_chocomilk.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/yinxing_cup.json](<../../Assets/StreamingAssets/GameData/Items/yinxing_cup.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/yinxing_drawing.json](<../../Assets/StreamingAssets/GameData/Items/yinxing_drawing.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/yinxing_fish.json](<../../Assets/StreamingAssets/GameData/Items/yinxing_fish.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/yinxing_milktea.json](<../../Assets/StreamingAssets/GameData/Items/yinxing_milktea.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/yinxing_miqingji.json](<../../Assets/StreamingAssets/GameData/Items/yinxing_miqingji.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/yinxing_news.json](<../../Assets/StreamingAssets/GameData/Items/yinxing_news.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/yinxing_plante.json](<../../Assets/StreamingAssets/GameData/Items/yinxing_plante.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/yinxing_shampoo.json](<../../Assets/StreamingAssets/GameData/Items/yinxing_shampoo.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/yinxing_soap.json](<../../Assets/StreamingAssets/GameData/Items/yinxing_soap.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/yinxing_yingyangye.json](<../../Assets/StreamingAssets/GameData/Items/yinxing_yingyangye.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/Items/yinxing_zhendingji.json](<../../Assets/StreamingAssets/GameData/Items/yinxing_zhendingji.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/SaveData/Wardrobe.json](<../../Assets/StreamingAssets/GameData/SaveData/Wardrobe.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/all_events.json](<../../Assets/StreamingAssets/GameData/all_events.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/all_events_list.json](<../../Assets/StreamingAssets/GameData/all_events_list.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/all_items_list.json](<../../Assets/StreamingAssets/GameData/all_items_list.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/clothes.json](<../../Assets/StreamingAssets/GameData/clothes.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/events_index.json](<../../Assets/StreamingAssets/GameData/events_index.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/items_index.json](<../../Assets/StreamingAssets/GameData/items_index.json>) — 配置/聚合资源。
- [Assets/StreamingAssets/GameData/merge.py](<../../Assets/StreamingAssets/GameData/merge.py>) — 合并工具（未执行）。

## 验证

全部 123 个 StreamingAssets JSON 可解析；聚合表 85 个事件 / 31 个物品，ID 无重复，物品引用事件与事件入口节点均存在。此项不包含全部分支边和语义的运行验证。

`Events/yinxing_item_news_001.json` 与 `all_events_list.json` 同 ID 条目内容不同；运行时以聚合表为准。当前 Items 分文件均可在聚合物品表找到相同对象。两个 merge.py 的内容均为合并 `./Items`，Events/merge.py 不能据名称推断为事件合并器；未执行脚本，未覆盖聚合文件。`all_events.json`、索引文件和 StreamingAssets 内 Wardrobe 存档样例不在当前 LoadAllData 主链上。

未测试实际磁盘存档往返、并发写入、移动端文件读取或旧存档兼容。

## 最近改动摘要

- 2026-10-28：初始化存储与配置边界，记录聚合表漂移及异步限制（L3 文档建档）；见 [ADR 0001](../adr/0001-existing-architecture.md)、[ADR 0002](../adr/0002-async-strategy.md)。
