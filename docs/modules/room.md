# 房间、布置与寻路

## 目录/程序集

Model/Tile、Furniture、Navigation，RoomManager、RoomView、Furnish、两种移动 AI、TileLoader / FurnitureLoader、WalkRules，房间 Resources / SO；默认 Assembly-CSharp。

## 命名空间

全局命名空间。

## 最后同步

commit `00d29af1871434383c29e34a13459b3534ee77ee`；2026-10-28；包含工作区未提交文档；业务实现未改。

## 职责

RoomManager.Awake 恢复 GridMap，失败时创建默认地图，再创建 Pathfinder、初始化 RoomView 并广播完成。GridMap 管地面、家具、装饰与占用；ChunkWorld 用 16×16 区块存格子；Pathfinder 为四方向、曼哈顿启发式 A*，默认最大展开 10000。

RoomView 把数据渲染为 Tilemap / 实例并显示布置预览；FurnishManager / Interactor / HomeDepot 处理分类、选中、移动、确认、取消、删除及换地图。CharacterTileMoveAI 用该网格寻路；CharacterMoveAI 是仍存在的 NavMesh 版本。

## 公开 API

- RoomManager：`FindPath(Vector2Int,Vector2Int)`、`ConfirmMoveFurniture(FurnitureInstance)` / `ConfirmMoveDecor(DecorInstance)`、`PreviewNewFurniture/Decor`、`PreviewMoveFurniture/Decor`、移除实例、坐标换算、`ComfirmNewFloor(string,Action)`、`SaveMapData()`、`ResetRoom()`。
- GridMap：`GetCell` / `GetCellRef`、`CanWalk`、`SetFloor` / `ChangeFloor`、`CanPlaceFurniture/Decor`、`PlaceFurniture` / `PlaceFurnitureKeepInstanceId`、`PlaceDecorKeepInstanceId`、实例创建/查询/移除、枚举格子/可行走格/实例。
- IRoomDataProvider 只暴露 `RoomManager roomManager { get; }`，由 FurnishManager 实现，交给 Interactor.Initialize；接口定义在 FurnishManager.cs。
- RoomView：`Initialize(GridMap)`、`RenderAll()`、`WorldToCell` / `CellLeftBottomToWorld`、各图层/预览接口。
- FurnitureDatabase：内置/自定义家具与装饰的查询、添加、删除和保存；TileDataBase / MapDatabase 提供配置查询。TileLoader：`LoadTile(string)`、`RegisterUserTile(string,string)`。
- CharacterTileMoveAI：`StartWandering()`、`SetFocus(bool)`、`MoveToPosition(Vector3)`、`IsWalkable(Vector3)`；旧 CharacterMoveAI 提供 `SetFocus` / `MoveToIfValid`。

## 公开接口契约

格子必须 HasFloor + FloorWalkable，且无阻挡家具才可走；装饰不参与阻挡判断。家具占用多个格子，实例 ID 与家具数据 ID 不同；移动保留实例 ID。无路径返回 null；RoomManager 额外检查起终点均可行走。

GridMap 默认读 `Resources/Map/room1` 并在代码中放置部分初始家具；FurnitureDatabase 读 `Resources/Furnitures` / `Decors`，TileDataBase 读 TileConfig，TileLoader 读 TileLibrary，MapDatabase 读 Map。换房间会清空现有家具和装饰，须经确认 UI。存档反射契约见 [storage](storage.md)。

TileLoader 的用户磁盘入口当前实际调用 Resources.Load，未实现任意磁盘 Tile 导入。FurnitureLoader.cs、GridCell.cs 主要为注释保留的旧方案；不可当作在用实现。MapDatabase 继承 MonoBehaviour 却由 GameManager 使用 new 构造，运行行为待 Unity 验证。

## 依赖

[runtime](runtime.md)、[storage](storage.md)、[character](character.md)、[presentation](presentation.md)；Unity Tilemap / NavMesh / DOTween。

## 被依赖

[runtime](runtime.md)、[storage](storage.md)、[presentation](presentation.md)、[project](project.md) 的地图编辑器。

## 事件

- `RoomManager.static event Action<RoomManager> OnRoomManagerInitialized` → CharacterTileMoveAI、CameraController；IsInitialized / GridMap 在广播前赋值。
- `GridMap.Action<Vector2> newCameraLimitMax` → CameraController；这是委托字段，非 event。
- `FurnishManager.static Action<bool> onEnterFurnishMode` → Character、MoveWhenClick；`FurnishItemEvents.static Action<string,FurnishCategory,int> OnItemClicked` → FurnishManager、HomeDepot。

## 数据所有权

RoomManager 拥有 GridMap / Pathfinder；GridMap 拥有区块与家具/装饰实例；RoomView 管显示缓存及实例 GameObject。FurnitureDatabase 管内置与用户配置，独立写 persistentDataPath 下 UserFurnitureData.json / UserDecorData.json。实例同时引用显示对象，非完全无 Unity 依赖的数据层。

## 文件清单

附属 `.meta` 同归属；场景/通用 prefab 接线由 [project](project.md) 记录。
- [Assets/Resources/Decors/carpet.asset](<../../Assets/Resources/Decors/carpet.asset>) — 资源/工程文件。
- [Assets/Resources/Decors/clock.asset](<../../Assets/Resources/Decors/clock.asset>) — 资源/工程文件。
- [Assets/Resources/Decors/door.asset](<../../Assets/Resources/Decors/door.asset>) — 资源/工程文件。
- [Assets/Resources/Decors/flower.asset](<../../Assets/Resources/Decors/flower.asset>) — 资源/工程文件。
- [Assets/Resources/Decors/light.asset](<../../Assets/Resources/Decors/light.asset>) — 资源/工程文件。
- [Assets/Resources/Decors/mirror.asset](<../../Assets/Resources/Decors/mirror.asset>) — 资源/工程文件。
- [Assets/Resources/Decors/window.asset](<../../Assets/Resources/Decors/window.asset>) — 资源/工程文件。
- [Assets/Resources/Furnitures/basin.asset](<../../Assets/Resources/Furnitures/basin.asset>) — 资源/工程文件。
- [Assets/Resources/Furnitures/bath.asset](<../../Assets/Resources/Furnitures/bath.asset>) — 资源/工程文件。
- [Assets/Resources/Furnitures/bed.asset](<../../Assets/Resources/Furnitures/bed.asset>) — 资源/工程文件。
- [Assets/Resources/Furnitures/bed2.asset](<../../Assets/Resources/Furnitures/bed2.asset>) — 资源/工程文件。
- [Assets/Resources/Furnitures/bigBed.asset](<../../Assets/Resources/Furnitures/bigBed.asset>) — 资源/工程文件。
- [Assets/Resources/Furnitures/bigTree.asset](<../../Assets/Resources/Furnitures/bigTree.asset>) — 资源/工程文件。
- [Assets/Resources/Furnitures/bookcase.asset](<../../Assets/Resources/Furnitures/bookcase.asset>) — 资源/工程文件。
- [Assets/Resources/Furnitures/cabinet.asset](<../../Assets/Resources/Furnitures/cabinet.asset>) — 资源/工程文件。
- [Assets/Resources/Furnitures/chair.asset](<../../Assets/Resources/Furnitures/chair.asset>) — 资源/工程文件。
- [Assets/Resources/Furnitures/chest.asset](<../../Assets/Resources/Furnitures/chest.asset>) — 资源/工程文件。
- [Assets/Resources/Furnitures/chimney.asset](<../../Assets/Resources/Furnitures/chimney.asset>) — 资源/工程文件。
- [Assets/Resources/Furnitures/light.asset](<../../Assets/Resources/Furnitures/light.asset>) — 资源/工程文件。
- [Assets/Resources/Furnitures/shelf.asset](<../../Assets/Resources/Furnitures/shelf.asset>) — 资源/工程文件。
- [Assets/Resources/Furnitures/sofa.asset](<../../Assets/Resources/Furnitures/sofa.asset>) — 资源/工程文件。
- [Assets/Resources/Furnitures/table.asset](<../../Assets/Resources/Furnitures/table.asset>) — 资源/工程文件。
- [Assets/Resources/Furnitures/toilet.asset](<../../Assets/Resources/Furnitures/toilet.asset>) — 资源/工程文件。
- [Assets/Resources/Furnitures/wall.asset](<../../Assets/Resources/Furnitures/wall.asset>) — 资源/工程文件。
- [Assets/Resources/Map/room1.asset](<../../Assets/Resources/Map/room1.asset>) — 资源/工程文件。
- [Assets/Resources/Map/room2.asset](<../../Assets/Resources/Map/room2.asset>) — 资源/工程文件。
- [Assets/Resources/TileConfig.asset](<../../Assets/Resources/TileConfig.asset>) — 资源/工程文件。
- [Assets/Resources/TileLibrary.asset](<../../Assets/Resources/TileLibrary.asset>) — 资源/工程文件。
- [Assets/SO/Furniture Database.asset](<../../Assets/SO/Furniture Database.asset>) — 资源/工程文件。
- [Assets/Scripts/GameObject/CharacterMoveAI.cs](<../../Assets/Scripts/GameObject/CharacterMoveAI.cs>) — 源码。
- [Assets/Scripts/GameObject/CharacterTileMoveAI.cs](<../../Assets/Scripts/GameObject/CharacterTileMoveAI.cs>) — 源码。
- [Assets/Scripts/GameObject/Furnish/FurnishInteractor.cs](<../../Assets/Scripts/GameObject/Furnish/FurnishInteractor.cs>) — 源码。
- [Assets/Scripts/GameObject/Furnish/FurnishItemEvents.cs](<../../Assets/Scripts/GameObject/Furnish/FurnishItemEvents.cs>) — 源码。
- [Assets/Scripts/GameObject/Furnish/FurnishManager.cs](<../../Assets/Scripts/GameObject/Furnish/FurnishManager.cs>) — 源码。
- [Assets/Scripts/GameObject/Furnish/FurnitureCell.cs](<../../Assets/Scripts/GameObject/Furnish/FurnitureCell.cs>) — 源码。
- [Assets/Scripts/GameObject/Furnish/HomeDepot.cs](<../../Assets/Scripts/GameObject/Furnish/HomeDepot.cs>) — 源码。
- [Assets/Scripts/GameObject/RoomView.cs](<../../Assets/Scripts/GameObject/RoomView.cs>) — 源码。
- [Assets/Scripts/GameObject/TileMapLab.cs](<../../Assets/Scripts/GameObject/TileMapLab.cs>) — 源码。
- [Assets/Scripts/Manager/Persistence/FurnitureLoader.cs](<../../Assets/Scripts/Manager/Persistence/FurnitureLoader.cs>) — 源码。
- [Assets/Scripts/Manager/Persistence/TileLoader.cs](<../../Assets/Scripts/Manager/Persistence/TileLoader.cs>) — 源码。
- [Assets/Scripts/Manager/RoomManager.cs](<../../Assets/Scripts/Manager/RoomManager.cs>) — 源码。
- [Assets/Scripts/Model/Furniture/DecorScriptableObject.cs](<../../Assets/Scripts/Model/Furniture/DecorScriptableObject.cs>) — 源码。
- [Assets/Scripts/Model/Furniture/FurnishItemData.cs](<../../Assets/Scripts/Model/Furniture/FurnishItemData.cs>) — 源码。
- [Assets/Scripts/Model/Furniture/FurnitureData.cs](<../../Assets/Scripts/Model/Furniture/FurnitureData.cs>) — 源码。
- [Assets/Scripts/Model/Furniture/FurnitureDataBaseScriptableObject.cs](<../../Assets/Scripts/Model/Furniture/FurnitureDataBaseScriptableObject.cs>) — 源码。
- [Assets/Scripts/Model/Furniture/FurnitureDatabase.cs](<../../Assets/Scripts/Model/Furniture/FurnitureDatabase.cs>) — 源码。
- [Assets/Scripts/Model/Furniture/FurnitureScriptableObject.cs](<../../Assets/Scripts/Model/Furniture/FurnitureScriptableObject.cs>) — 源码。
- [Assets/Scripts/Model/Furniture/MapDatabase.cs](<../../Assets/Scripts/Model/Furniture/MapDatabase.cs>) — 源码。
- [Assets/Scripts/Model/Navigation/Chunk.cs](<../../Assets/Scripts/Model/Navigation/Chunk.cs>) — 源码。
- [Assets/Scripts/Model/Navigation/ChunkWorld.cs](<../../Assets/Scripts/Model/Navigation/ChunkWorld.cs>) — 源码。
- [Assets/Scripts/Model/Navigation/MinHeap.cs](<../../Assets/Scripts/Model/Navigation/MinHeap.cs>) — 源码。
- [Assets/Scripts/Model/Navigation/Pathfinder.cs](<../../Assets/Scripts/Model/Navigation/Pathfinder.cs>) — 源码。
- [Assets/Scripts/Model/Tile/DecorInstance.cs](<../../Assets/Scripts/Model/Tile/DecorInstance.cs>) — 源码。
- [Assets/Scripts/Model/Tile/FurnitureInstance.cs](<../../Assets/Scripts/Model/Tile/FurnitureInstance.cs>) — 源码。
- [Assets/Scripts/Model/Tile/GridCell.cs](<../../Assets/Scripts/Model/Tile/GridCell.cs>) — 源码。
- [Assets/Scripts/Model/Tile/GridMap.cs](<../../Assets/Scripts/Model/Tile/GridMap.cs>) — 源码。
- [Assets/Scripts/Model/Tile/MapDataSO.cs](<../../Assets/Scripts/Model/Tile/MapDataSO.cs>) — 源码。
- [Assets/Scripts/Model/Tile/RoomData.cs](<../../Assets/Scripts/Model/Tile/RoomData.cs>) — 源码。
- [Assets/Scripts/Model/Tile/TileBaseLibrarySO.cs](<../../Assets/Scripts/Model/Tile/TileBaseLibrarySO.cs>) — 源码。
- [Assets/Scripts/Model/Tile/TileConfigSO.cs](<../../Assets/Scripts/Model/Tile/TileConfigSO.cs>) — 源码。
- [Assets/Scripts/Model/Tile/TileData.cs](<../../Assets/Scripts/Model/Tile/TileData.cs>) — 源码。
- [Assets/Scripts/Model/Tile/TileDataBase.cs](<../../Assets/Scripts/Model/Tile/TileDataBase.cs>) — 源码。
- [Assets/Scripts/Tools/WalkRules.cs](<../../Assets/Scripts/Tools/WalkRules.cs>) — 源码。

## 验证

静态核对数据/渲染/输入链、资源路径与寻路。Room 场景含两种移动脚本引用：同一角色的旧 NavMesh 组件禁用、TileMove 启用；另一个对象仍有启用的旧 NavMesh 组件，未运行验证用途。初始化依赖 Awake 与广播时机，尚未证明无时序问题。

后续验证需覆盖负坐标区块、家具占用/移除、取消预览、换房间、地图存读一致性、角色死亡/复活与移动。

## 最近改动摘要

- 2026-10-28：初始化房间模块、记录现存两套移动与遗留代码（L3 文档建档），无重构。
