using System;
using System.Collections.Generic;
using UnityEngine;

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
        // 1. 构建世界
        GridMap = new GridMap();

        // 2. 构建算法工具
        pathfinder = new Pathfinder(GridMap);

        // 3. 初始化 View
        roomView.Initialize(GridMap);

        // 4. 标记完成
        IsInitialized = true;
        Debug.Log("RoomManager: Initialization complete");

        // 5. 广播
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
        roomView.PlaceFurnitureKeepInstanceId(currentFurnitureInstance);
        currentFurnitureInstance.anchorPos = cellPos;

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
    }

    public void RemoveDecor(DecorInstance currentDecorInstance) {
        roomView.RemoveDecor(currentDecorInstance);
        GridMap.RemoveDecor(currentDecorInstance);
    }

    public void PreviewMoveFurniture(FurnitureInstance furniture, Vector3 hitPoint, Vector3 pos) {
        Vector2Int cell = roomView.WorldToCell(pos);
        roomView.PreviewMoveFurniture(furniture, hitPoint, cell);
    }

    public void PreviewMoveDecor(DecorInstance decor, Vector3 hitPoint, Vector3 pos) {
        Vector2Int cell = roomView.WorldToCell(pos);
        roomView.PreviewMoveDecor(decor, hitPoint, cell);
    }
}