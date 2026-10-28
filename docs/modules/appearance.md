# 建角、外观与衣柜

## 目录/程序集

Manager/EditScene、UI/Edit、UI/Closet、CharacterAppearanceRenderer、SpriteHSL 与外观材质/相关 prefab；默认 Assembly-CSharp。

## 命名空间

全局命名空间。

## 最后同步

commit `00d29af1871434383c29e34a13459b3534ee77ee`；2026-10-28；包含工作区未提交文档；业务实现未改。

## 职责

编辑页读取 BootSceneManager 的临时角色资料，图集提供部件 Sprite，AppearanceLoader / Renderer 应用部件与 HSV。衣柜展示槽、购买、切换衣物及用户图片；LoadHint 通过 NativeGallery 选择/保存图片。

## 公开 API

- AppearanceAtlasManager：`GetPartSprite(string,int)`、`GetAtlasByType(string)`；AppearanceLoader：`ApplyAppearance(CharacterAppearance,string)`。
- CharacterAppearanceRenderer：`LoadAndApplyAppearance()`、`ApplyAppearance(CharacterAppearance)`、`RefreshAppearance()`；useBootSceneManager 决定初始数据入口。
- Closet：`ChangeShowingState()`；ClothList：`BuyCloth()`；ClothUI：`Init(WardrobeSlot,Action<GameObject>)`、`UploadCloth(Sprite)`、默认衣物/添加按钮创建。
- LoadHint：`Init(GameObject)`、`SaveTemplate()`、`PickImage()`、`PickComponent(string)`、`OpenPanel(GameObject)`；各编辑 UI 的公开方法供 Unity 事件调用。

## 公开接口契约

部件字符串 FrontHair / SideHair / BackHair / Body / LeftEye / RightEye / LeftEyeBlanc / RightEyeBlanc / Clothes / HeadDeco1 / HeadDeco2 与 Boot、图集字典、UI 一致；不得任意改名。内置部件用数字 ID，用户服装用持久化图片 ID。

WardrobeSlot.State 字符串为 Locked / Empty / Own / AddButton；声明位于 [character](character.md)。衣柜关闭时提交已拥有的预览服装；导入图片及衣柜表保存见 [storage](storage.md)。CharacterAppearance / HSV 的声明在 CharacterData.cs，改字段需同步检查主存档、Boot、渲染器和编辑 UI。

## 依赖

[runtime](runtime.md)、[character](character.md)、[storage](storage.md)、[presentation](presentation.md)，SpriteAtlas、NativeGallery、DOTween。

## 被依赖

[runtime](runtime.md)、[presentation](presentation.md)；由建角、主场景加载和角色点击触发。

## 事件

`Closet.static Action<bool> onClosetToggled` → MoveWhenClick。消费 BootSceneManager.`OnAppearanceChanged(CharacterAppearance,string)`：AppearanceLoader、HSVSliderController。`InfoItem.event Action<string> EditCompleted` → ApparenceNameUpdater，供编辑结束后更新名称文字。

## 数据所有权

编辑暂存由 BootSceneManager 拥有；已选服装/外观由 CharacterData 持有；WardrobeData.Slots 是静态共享字典。图集与材质是资源，导入图片存于 persistentDataPath；加载/预览 Sprite 不是配置源文件。

## 文件清单

附属 `.meta` 同归属；具体美术图集位于 Assets/Arts，资源整体盘点见 [project](project.md)。
- [Assets/Material/AppearenceComponent.mat](<../../Assets/Material/AppearenceComponent.mat>) — 资源/工程文件。
- [Assets/PreFab/Cloth.prefab](<../../Assets/PreFab/Cloth.prefab>) — 资源/工程文件。
- [Assets/PreFab/Cloth_custom.prefab](<../../Assets/PreFab/Cloth_custom.prefab>) — 资源/工程文件。
- [Assets/PreFab/Component.prefab](<../../Assets/PreFab/Component.prefab>) — 资源/工程文件。
- [Assets/PreFab/Component_custom.prefab](<../../Assets/PreFab/Component_custom.prefab>) — 资源/工程文件。
- [Assets/PreFab/uploadHint Variant.prefab](<../../Assets/PreFab/uploadHint Variant.prefab>) — 资源/工程文件。
- [Assets/PreFab/uploadHint.prefab](<../../Assets/PreFab/uploadHint.prefab>) — 资源/工程文件。
- [Assets/Scripts/GameObject/CharacterAppearanceRenderer.cs](<../../Assets/Scripts/GameObject/CharacterAppearanceRenderer.cs>) — 源码。
- [Assets/Scripts/Manager/EditScene/AppearanceAtlasManager.cs](<../../Assets/Scripts/Manager/EditScene/AppearanceAtlasManager.cs>) — 源码。
- [Assets/Scripts/Manager/EditScene/AppearanceLoader.cs](<../../Assets/Scripts/Manager/EditScene/AppearanceLoader.cs>) — 源码。
- [Assets/Scripts/Manager/EditScene/CharacterInfoManager.cs](<../../Assets/Scripts/Manager/EditScene/CharacterInfoManager.cs>) — 源码。
- [Assets/Scripts/UI/Closet/Closet.cs](<../../Assets/Scripts/UI/Closet/Closet.cs>) — 源码。
- [Assets/Scripts/UI/Closet/ClothList.cs](<../../Assets/Scripts/UI/Closet/ClothList.cs>) — 源码。
- [Assets/Scripts/UI/Closet/ClothUI.cs](<../../Assets/Scripts/UI/Closet/ClothUI.cs>) — 源码。
- [Assets/Scripts/UI/Closet/LoadHint.cs](<../../Assets/Scripts/UI/Closet/LoadHint.cs>) — 源码。
- [Assets/Scripts/UI/Edit/ApparenceComponent.cs](<../../Assets/Scripts/UI/Edit/ApparenceComponent.cs>) — 源码。
- [Assets/Scripts/UI/Edit/ApparenceNameUpdater.cs](<../../Assets/Scripts/UI/Edit/ApparenceNameUpdater.cs>) — 源码。
- [Assets/Scripts/UI/Edit/ComfirmInfo.cs](<../../Assets/Scripts/UI/Edit/ComfirmInfo.cs>) — 源码。
- [Assets/Scripts/UI/Edit/CustomPanel.cs](<../../Assets/Scripts/UI/Edit/CustomPanel.cs>) — 源码。
- [Assets/Scripts/UI/Edit/FinalConfirmEditButton.cs](<../../Assets/Scripts/UI/Edit/FinalConfirmEditButton.cs>) — 源码。
- [Assets/Scripts/UI/Edit/HSVSliderController.cs](<../../Assets/Scripts/UI/Edit/HSVSliderController.cs>) — 源码。
- [Assets/Scripts/UI/Edit/InfoInputAnimator.cs](<../../Assets/Scripts/UI/Edit/InfoInputAnimator.cs>) — 源码。
- [Assets/Scripts/UI/Edit/InfoItem.cs](<../../Assets/Scripts/UI/Edit/InfoItem.cs>) — 源码。
- [Assets/Scripts/UI/Edit/TypeButton.cs](<../../Assets/Scripts/UI/Edit/TypeButton.cs>) — 源码。
- [Assets/Shader/SpriteHSL.shader](<../../Assets/Shader/SpriteHSL.shader>) — 资源/工程文件。

## 验证

静态核对事件/图集名、服装保存与图片选择入口。未运行建角、导入、预览、购买或真机相册权限；后续改动须验证这些路径与切场景后的外观恢复。

## 最近改动摘要

- 2026-10-28：初始化建角和衣柜边界（L3 文档建档），未修改 prefab 或资源。
