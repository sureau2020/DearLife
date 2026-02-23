using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class GridMap
{
    public Vector2 CameraLimitMax { get; private set; }
    public Action<Vector2> newCameraLimitMax;

    private readonly ChunkWorld world = new();

    // 家具实例
    private readonly Dictionary<string, FurnitureInstance> furnitureInstances = new();
    private int nextFurnitureInstanceId = 1;

    private readonly Dictionary<string, DecorInstance> decorInstances = new();
    private int nextDecorInstanceId = 1;

    private TileDataBase tileDataBase;
    private FurnitureDatabase furnitureDatabase;


    public GridMap()
    {
          InitializeMap();
    }

    private void InitializeMap()
    {
        tileDataBase = GameManager.Instance.TileDataBase;
        furnitureDatabase = GameManager.Instance.FurnitureDatabase;
        var mapData = Resources.Load<MapDataSO>("MapData");
        if (mapData == null) {
            ErrorNotifier.NotifyError("Failed to load MapDataSO from Resources/MapData"); return;
        }

        Vector2Int maxPos = Vector2Int.zero;
        foreach (var tileInstance in mapData.tileInstances)
        {
            // 将Vector3Int转换为Vector2Int（忽略Z轴）
            Vector2Int pos = new Vector2Int(tileInstance.position.x, tileInstance.position.y);
            if (pos.x > maxPos.x) maxPos.x = pos.x;
            if (pos.y > maxPos.y) maxPos.y = pos.y;

            if (pos.x == 1 && pos.y == 4)
            {
                AddCellInternal(pos.x, pos.y, tileInstance.tileId, "sofa", "");
            }
            else if (pos.x == 1 && pos.y == 3)
            {
                AddCellInternal(pos.x, pos.y, tileInstance.tileId, "light", "");
            }
            else if (pos.x == 7 && pos.y == 6)
            {
                AddCellInternal(pos.x, pos.y, tileInstance.tileId, "toilet", "");
            }
            else if (pos.x == 5 && pos.y == 7)
            {
                AddCellInternal(pos.x, pos.y, tileInstance.tileId, "bath", "");
            }
            else if (pos.x == 1 && pos.y == 7)
            {
                AddCellInternal(pos.x, pos.y, tileInstance.tileId, "bookcase", "");
            }
            else if (pos.x == 4 && pos.y == 3)
            {
                AddCellInternal(pos.x, pos.y, tileInstance.tileId, "shelf", "");
            }
            else if (pos.x == 1 && pos.y == 0)
            {
                AddCellInternal(pos.x, pos.y, tileInstance.tileId, "bed", "");
            }
            else if (pos.x == 4 && pos.y == 0)
            {
                AddCellInternal(pos.x, pos.y, tileInstance.tileId, "table", "");
            }
            else if (pos.x == 4 && pos.y == 8)
            {
                AddCellInternal(pos.x, pos.y, tileInstance.tileId, "", "mirror");
            }
            else
            {
                // 使用MapDataSO中的tileId作为地板
                AddCellInternal(pos.x, pos.y, tileInstance.tileId, "", "");
            }

        }
        CameraLimitMax = maxPos;
        newCameraLimitMax?.Invoke(new Vector2(maxPos.x, maxPos.y));
    }

    private void AddCellInternal(int x, int y, string floorTileId, string furnitureDataId, string decorId)
    {
        
        Vector2Int pos = new Vector2Int(x, y);
        ref CellData cell = ref world.GetCellRef(pos);
        if (!string.IsNullOrWhiteSpace(floorTileId))
        {
            SetFloor(pos, floorTileId, tileDataBase.IsWalkable(floorTileId));
        }
        if (!string.IsNullOrWhiteSpace(furnitureDataId))
        {
            if (furnitureDatabase.GetFurnitureData(furnitureDataId) == null) { 
                Debug.LogError($"FurnitureData with id '{furnitureDataId}' not found in FurnitureDatabase"); return;
            }
            PlaceFurniture(pos, furnitureDatabase.GetFurnitureData(furnitureDataId));
        }
        if (!string.IsNullOrWhiteSpace(decorId))
        {
            SetDecor(pos, decorId);
        }
    }


    // ==== 基础 Cell 操作 ====

    public ref CellData GetCellRef(Vector2Int pos)
        => ref world.GetCellRef(pos);

    public CellData GetCell(Vector2Int pos)
        => world.GetCell(pos);

    // ==== Floor ====

    public void SetFloor(Vector2Int pos, string floorTileId, bool walkable)
    {
        ref CellData cell = ref world.GetCellRef(pos);

        cell.floorTileId = floorTileId;
        cell.flags |= CellFlags.HasFloor;

        if (walkable)
            cell.flags |= CellFlags.FloorWalkable;
        else
            cell.flags &= ~CellFlags.FloorWalkable;
    }

    // ==== Walkable ====

    public bool CanWalk(Vector2Int pos)
    {
        CellData cell = world.GetCell(pos);

        if (!cell.Has(CellFlags.HasFloor)) return false;
        if (!cell.Has(CellFlags.FloorWalkable)) return false;

        if (cell.Has(CellFlags.HasFurniture) &&
            cell.Has(CellFlags.FurnitureBlocked))
            return false;

        return true;
    }

    public bool HasFloor(Vector2Int pos)
    {
        CellData cell = world.GetCell(pos);
        return cell.Has(CellFlags.HasFloor);
    }

    public bool HasWalkbleFLoor(Vector2Int pos)
    {
        CellData cell = world.GetCell(pos);
        return cell.Has(CellFlags.FloorWalkable);
    }

    public bool HasFurniture(Vector2Int pos)
    {
        CellData cell = world.GetCell(pos);
        return cell.Has(CellFlags.HasFurniture);
    }

    public FurnitureInstance GetFurniture(Vector2Int pos)
    {
        CellData cell = world.GetCell(pos);
        if (!cell.Has(CellFlags.HasFurniture)) return null;
        Debug.Log($"在GetFurniture时，cell.furnitureInstanceId={cell.furnitureInstanceId}");
        return furnitureInstances.GetValueOrDefault(cell.furnitureInstanceId);
    }

    // ==== Furniture ====

    public void PlaceFurnitureKeepInstanceId(Vector2Int anchorPos, FurnitureData data, FurnitureInstance furnitureInstance)
    {
        Vector2Int originPos = furnitureInstance.anchorPos;
        string instanceId = furnitureInstance.instanceId;
        var offsets = data.occupiedCells.ToArray();

        // --- 第一步：清除旧位置的所有格点数据 ---
        if (originPos.x != int.MinValue) {
            foreach (var offset in offsets)
            {
                Vector2Int oldPos = originPos + offset;
                ref CellData cell = ref world.GetCellRef(oldPos);

                cell.furnitureInstanceId = "";
                cell.flags &= ~CellFlags.HasFurniture;
                if (data.blocksMovement)
                    cell.flags &= ~CellFlags.FurnitureBlocked;
            }
        }
        // --- 第二步：更新实例本身的数据 ---
        // 这里直接清除并重新填充，避免在循环中 Add/Remove
        furnitureInstance.occupiedCells.Clear();
        furnitureInstance.anchorPos = anchorPos;
        // --- 第三步：填充新位置的所有格点数据 ---
        foreach (var offset in offsets)
        {
            Vector2Int pos = anchorPos + offset;
            ref CellData cell = ref world.GetCellRef(pos);

            cell.furnitureInstanceId = instanceId;
            cell.flags |= CellFlags.HasFurniture;
            if (data.blocksMovement)
                cell.flags |= CellFlags.FurnitureBlocked;

            furnitureInstance.occupiedCells.Add(pos);
        }
    }

    // 直接放置家具（不检查位置是否合法），仅供初始化使用
    private void PlaceFurnitureInternal(Vector2Int anchorPos, FurnitureData data) {
        string instanceId = $"furniture_{nextFurnitureInstanceId++}";

        FurnitureInstance instance = new FurnitureInstance {
            instanceId = instanceId,
            furnitureDataId = data.id,
            anchorPos = anchorPos,
            occupiedCells = new List<Vector2Int>()
        };

        // 写入 cell
        foreach (var offset in data.occupiedCells)
        {
            Vector2Int pos = anchorPos + offset;
            ref CellData cell = ref world.GetCellRef(pos);

            cell.furnitureInstanceId = instanceId;
            cell.flags |= CellFlags.HasFurniture;

            if (data.blocksMovement)
                cell.flags |= CellFlags.FurnitureBlocked;

            instance.occupiedCells.Add(pos);
        }

        furnitureInstances.Add(instanceId, instance);
    }

    // 尝试放置家具，成功返回 true，失败（位置被占用）返回 false
    public bool PlaceFurniture(Vector2Int anchorPos, FurnitureData data)
    {
        if (data == null) { Debug.LogError("FurnitureData is null"); return false; }
        // 可放置检测
        foreach (var offset in data.occupiedCells)
        {
            Vector2Int pos = anchorPos + offset;
            CellData cell = world.GetCell(pos);

            if (cell.Has(CellFlags.HasFurniture))
                return false;
        }

        PlaceFurnitureInternal(anchorPos, data);
        return true;
    }

    public bool CanPlaceFurniture(FurnitureData furnitureData, Vector2Int cellPos, string instanceId)
    {
        foreach (var offset in furnitureData.occupiedCells)
        {
            Vector2Int pos = cellPos + offset;
            CellData cell = world.GetCell(pos);
            if (!CanWalk(pos)) {
                if (cell.furnitureInstanceId == instanceId) continue;
                return false;
            } 
            Debug.Log($"{furnitureData.occupiedCells.Count}在检查时");
        }
        return true;
    }

    public void RemoveFurniture(FurnitureInstance furnitureInstance)
    {
        String id = furnitureInstance.instanceId;
        foreach (var pos in furnitureInstance.occupiedCells)
        {
            ref CellData c = ref world.GetCellRef(pos);
            c.flags &= ~(CellFlags.HasFurniture | CellFlags.FurnitureBlocked);
            c.furnitureInstanceId = "";
        }
        furnitureInstances.Remove(id);
    }

    public FurnitureInstance CreateFurnitureInstance(FurnitureData data)
    {
        string instanceId = $"furniture_{nextFurnitureInstanceId++}";

        FurnitureInstance instance = new FurnitureInstance
        {
            instanceId = instanceId,
            furnitureDataId = data.id,
            anchorPos = new Vector2Int(int.MinValue, int.MinValue), 
            occupiedCells = data.occupiedCells
        };
        furnitureInstances.Add(instanceId, instance);
        return instance;
    }

    public FurnitureInstance GetFurnitureInstance(string instanceId)
        => furnitureInstances.GetValueOrDefault(instanceId);


    public FurnitureInstance GetFurnitureAt(Vector2Int pos)
    {
        CellData cell = world.GetCell(pos);
        if (!cell.Has(CellFlags.HasFurniture)) return null;

        return furnitureInstances.GetValueOrDefault(cell.furnitureInstanceId);
    }

    public DecorInstance GetDecorAt(Vector2Int pos)
    {
        CellData cell = world.GetCell(pos);
        if (string.IsNullOrWhiteSpace(cell.decorInstanceId)) return null;
        return decorInstances.GetValueOrDefault(cell.decorInstanceId);
    }

    // ==== Decor ====
    public DecorInstance CreateDecorInstance(DecorData data) {
        string instanceId = $"decor_{nextDecorInstanceId++}";
        DecorInstance decor = new DecorInstance
        {
            instanceId = instanceId,
            decorId = data.id,
            position = new Vector2Int(int.MinValue, int.MinValue),
        };

        decorInstances.Add(instanceId, decor);

        return decor;
    }

    public bool CanPlaceDecor(Vector2Int cellPos, string instanceId)
    {

        CellData cell = world.GetCell(cellPos);
        if (!cell.Has(CellFlags.HasFloor)) return false;
        if (!string.IsNullOrEmpty(cell.decorInstanceId) && cell.decorInstanceId != instanceId) return false;

        return true;
    }

    public void PlaceDecorKeepInstanceId(Vector2Int anchorPos, DecorInstance decorInstance)
    {
        Vector2Int originPos = decorInstance.position;
        string instanceId = decorInstance.instanceId;

        ref CellData cell = ref world.GetCellRef(originPos);
        cell.decorInstanceId = "";

        decorInstance.position = anchorPos;

        ref CellData newCell = ref world.GetCellRef(anchorPos);
        newCell.decorInstanceId = instanceId;
    }

    public void SetDecor(Vector2Int pos, string decorId)
    {
        ref CellData cell = ref world.GetCellRef(pos);
        cell.decorInstanceId = $"decor_{nextDecorInstanceId++}";
        decorInstances[cell.decorInstanceId] = new DecorInstance
        {
            instanceId = cell.decorInstanceId,
            decorId = decorId,
            position = pos
        };
    }

    public void RemoveDecor(DecorInstance decorInstance)
    {
        ref CellData cell = ref world.GetCellRef(decorInstance.position);
        cell.decorInstanceId = "";
        String id = decorInstance.instanceId;
        decorInstances.Remove(id);
    }

    // ==== Query ====

    public IEnumerable<Vector2Int> GetAllWalkableCells()
    {
        foreach (var chunkPair in world.DebugChunks())
        {
            var chunk = chunkPair.Value;
            var basePos = chunk.chunkCoord * Chunk.CHUNK_SIZE;

            for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
                for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
                {
                    Vector2Int worldPos = basePos + new Vector2Int(x, y);
                    if (CanWalk(worldPos))
                        yield return worldPos;
                }
        }
    }

    // 所有 cell（仅用于渲染 / debug）
    public IEnumerable<KeyValuePair<Vector2Int, CellData>> DebugAllCells()
    {
        foreach (var chunkPair in world.DebugChunks())
        {
            var chunk = chunkPair.Value;
            Vector2Int basePos = chunk.chunkCoord * Chunk.CHUNK_SIZE;

            for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
                for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
                {
                    yield return new(
                        basePos + new Vector2Int(x, y),
                        chunk.cells[x, y]
                    );
                }
        }
    }

    public IEnumerable<FurnitureInstance> GetAllFurnitureInstances()
        => furnitureInstances.Values;

    public IEnumerable<DecorInstance> GetAllDecorInstances()
        => decorInstances.Values;
}