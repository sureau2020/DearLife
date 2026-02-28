using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class RoomManager : MonoBehaviour
{
    public GridMap GridMap { get; private set; }

    private Pathfinder pathfinder;

    [SerializeField] private RoomView roomView;

    // GridMap 初始化完成事件（只通知世界状态）
    //public static event Action<GridMap> OnGridMapInitialized;
    public static event Action<RoomManager> OnRoomManagerInitialized;

    public bool IsInitialized { get; private set; }

    private void Awake()
    {
        // 1. 尝试加载保存的 GridMap
        var loadResult = LoaderManager.LoadGridMap();
        if (loadResult.Success)
        {
            GridMap = loadResult.Data;
            Debug.Log("GridMap 从存档加载成功");
        }
        else
        {
            Debug.Log($"GridMap 加载失败：{loadResult.Message}，创建新的 GridMap");
            // 2. 如果加载失败，构建新的世界
            GridMap = new GridMap();
        }

        // 3. 构建算法工具
        pathfinder = new Pathfinder(GridMap);

        // 4. 初始化 View
        roomView.Initialize(GridMap);

        // 5. 标记完成
        IsInitialized = true;
        Debug.Log("RoomManager: Initialization complete");

        // 6. 广播
        OnRoomManagerInitialized?.Invoke(this);
    }

    // =====================
    // 对外：寻路接口
    // =====================

    public List<Vector2Int> FindPath(Vector2Int from, Vector2Int to)
    {
        if (!IsInitialized)
            return null;

        if (!GridMap.CanWalk(from) || !GridMap.CanWalk(to))
            return null;

        return pathfinder.FindPath(from, to);
    }

    // =====================
    // Debug / View
    // =====================

    public void ShowFurnitureCells()
    {
        roomView.ClearCells();
        roomView.RenderFurnitureLayer();
    }

    public void ShowDecorCells()
    {
        roomView.ClearCells();
        roomView.RenderDecorLayer();
    }

    public void ShowFloorCells()
    {
        roomView.ClearCells();
        roomView.RenderFloorLayer();
    }

    public void ClearAllCells() { 
        roomView.ClearCells();
    }

    // =====================
    // interact with FurnishManager
    // =====================

    public bool ConfirmMoveFurniture(FurnitureInstance currentFurnitureInstance)
    {
        if(currentFurnitureInstance == null)return false;
        Vector2Int cellPos = roomView.WorldToCell(currentFurnitureInstance.furnitureObject.transform.position);
        if (cellPos == null) return false;
        FurnitureData data = GameManager.Instance.FurnitureDatabase.GetFurnitureData(currentFurnitureInstance.furnitureDataId);
        if (data == null) return false;
        if (!GridMap.CanPlaceFurniture(data, cellPos, currentFurnitureInstance.instanceId)) return false;
        GridMap.PlaceFurnitureKeepInstanceId(cellPos, data,currentFurnitureInstance);
        Debug.Log($"Furniture {currentFurnitureInstance.instanceId} moved to {currentFurnitureInstance.anchorPos},{currentFurnitureInstance.furnitureDataId},{currentFurnitureInstance.ToString()}");
        SaveMapData();
        return true;
    }

    public bool ConfirmMoveDecor(DecorInstance currentDecorInstance)
    {
        if(currentDecorInstance == null)return false;
        Vector2Int cellPos = roomView.WorldToCell(currentDecorInstance.decorObject.transform.position);
        if (cellPos == null) return false;
        DecorData data = GameManager.Instance.FurnitureDatabase.GetDecorData(currentDecorInstance.decorId);
        if (data == null) return false;
        if (!GridMap.CanPlaceDecor(cellPos, currentDecorInstance.instanceId)) return false;
        GridMap.PlaceDecorKeepInstanceId(cellPos, currentDecorInstance);
        SaveMapData();
        return true;
    }

    public FurnitureInstance GetFurnitureAt(Vector3 pos)
    {
        return GridMap.GetFurniture(roomView.WorldToCell(pos));
    }

    public DecorInstance GetDecorInstanceAt(Vector3 pos)
    {
        return GridMap.GetDecorAt(roomView.WorldToCell(pos));
    }

    public Vector3 GetCellWorldLeftBottomPosition(Vector2Int cellPos)
    {
        return roomView.CellLeftBottomToWorld(cellPos);
    }

    public Vector3 GetCellWorldLeftBottomPosition(Vector3 hitPoint) { 
        Vector2Int cellPos = roomView.WorldToCell(hitPoint);
        return GetCellWorldLeftBottomPosition(cellPos);
    }

    public void RemoveFurniture(FurnitureInstance currentFurnitureInstance) {
        roomView.RemoveFurniture(currentFurnitureInstance);
        GridMap.RemoveFurniture(currentFurnitureInstance);
        SaveMapData();
    }

    public void RemoveDecor(DecorInstance currentDecorInstance) {
        roomView.RemoveDecor(currentDecorInstance);
        GridMap.RemoveDecor(currentDecorInstance);
        SaveMapData();
    }

    public void PreviewMoveFurniture(FurnitureInstance furniture, Vector3 hitPoint, Vector3 pos) {
        Vector2Int cell = roomView.WorldToCell(pos);
        roomView.PreviewMoveFurniture(furniture, hitPoint, cell);
    }

    public void PreviewMoveDecor(DecorInstance decor, Vector3 hitPoint, Vector3 pos) {
        Vector2Int cell = roomView.WorldToCell(pos);
        roomView.PreviewMoveDecor(decor, hitPoint, cell);
    }

    public FurnitureInstance PreviewNewFurniture(string id, Vector3 hitPoint, Vector3? curPos, FurnitureInstance? instance) {
        Vector2Int cell = roomView.WorldToCell(hitPoint);
        FurnitureData data = GameManager.Instance.FurnitureDatabase.GetFurnitureData(id);
        FurnitureInstance tempInstance = GridMap.CreateFurnitureInstance(data);

        if (curPos != null) {
            Vector2Int curCell = roomView.WorldToCell((Vector3)curPos);
            roomView.PreviewMoveFurniture(instance, hitPoint, curCell);
        } else
            roomView.PreviewNewFurniture(tempInstance, cell, data);
        return tempInstance;
    }

    public DecorInstance PreviewNewDecor(string id, Vector3 hitPoint, Vector3? curPos, DecorInstance? instance)
    {
        Vector2Int cell = roomView.WorldToCell(hitPoint);
        DecorData data = GameManager.Instance.FurnitureDatabase.GetDecorData(id);
        DecorInstance tempInstance = GridMap.CreateDecorInstance(data);

        if (curPos != null)
        {
            Vector2Int curCell = roomView.WorldToCell((Vector3)curPos);
            roomView.PreviewMoveDecor(instance, hitPoint, curCell);
        }
        else
            roomView.PreviewNewDecor(tempInstance, cell, data);
        return tempInstance;
    }

    public void SaveMapData()
    {
        var saveResult = SaveManager.SaveGridMap(GridMap);
        if (!saveResult.Success)
        {
            Debug.LogError($"GridMap 保存失败：{saveResult.Message}");
        }
    }

    public void ResetRoom()
    {
        GridMap = new GridMap();
        pathfinder = new Pathfinder(GridMap);
        roomView.Initialize(GridMap);
        SaveMapData();
    }
}