using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridMap
{
    public Dictionary<Vector2Int, GridCell> cells = new();
    public List<Vector2Int> walkableCells = new();
    
    // 家具实例管理
    private Dictionary<string, FurnitureInstance> furnitureInstances = new();
    private int nextFurnitureInstanceId = 1;

    // 默认房间，目前是硬编码的
    public GridMap()
    {
        cells = new Dictionary<Vector2Int, GridCell>();
        walkableCells = new List<Vector2Int>();

        // 创建默认地板 - 批量添加，最后统一更新walkableCells
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                AddCellInternal(x, y, "grass", "light", "");
            }
        }
        
        // 批量添加完成后，统一更新可行走区域
        UpdateWalkableCells();
    }

    // 内部方法：添加格子但不更新walkableCells（用于批量操作）
    private void AddCellInternal(int x, int y, string groundTileId, string furnitureId, string decorId) 
    {
        Vector2Int pos = new Vector2Int(x, y);
        GroundLayer ground = string.IsNullOrWhiteSpace(groundTileId) ? null : CreateGround(groundTileId);
        DecorLayer decor = string.IsNullOrWhiteSpace(decorId) ? null : CreateDecor(decorId);

        cells[pos] = new GridCell(pos, ground, null, decor);

        // 如果有家具，稍后通过PlaceFurniture方法添加
        if (!string.IsNullOrWhiteSpace(furnitureId))
        {
            PlaceFurnitureInternal(pos, furnitureId);
        }
        Debug.Log($"Added cell at ({x},{y}) with ground '{groundTileId}', furniture '{furnitureId}', decor '{decorId}'");
    }

    // 公共方法：添加单个格子并更新walkableCells
    public void AddCell(int x, int y, string groundTileId, string furnitureId, string decorId) 
    {
        AddCellInternal(x, y, groundTileId, furnitureId, decorId);
        UpdateWalkableCells();
    }

    private GroundLayer CreateGround(string tileId) 
    {
        TileData tileData = TileDataBase.GetTileById(tileId);
        if (tileData == null) return null;
        
        return new GroundLayer
        {
            floorTileId = tileId,
            walkable = tileData.walkable
        };
    }

    private DecorLayer CreateDecor(string decorId) 
    {
        return new DecorLayer
        {
            decorId = decorId,
            decorObject = null//TODO
        };
    }

    // 内部方法：放置家具但不更新walkableCells（用于批量操作和初始化）
    private bool PlaceFurnitureInternal(Vector2Int anchorPos, string furnitureDataId)
    {
       FurnitureDatabase db = GameManager.Instance.FurnitureDatabase;

        var furnitureData = db.GetFurnitureData(furnitureDataId);
        if (furnitureData == null) return false;

        // 检查是否可以放置
        if (!CanPlaceFurniture(anchorPos, furnitureData))
            return false;

        // 生成家具实例ID
        string instanceId = $"furniture_{nextFurnitureInstanceId++}";

        // 创建家具实例
        FurnitureInstance instance = new FurnitureInstance
        {
            instanceId = instanceId,
            furnitureDataId = furnitureDataId,
            anchorPos = anchorPos,
            occupiedCells = new List<Vector2Int>()
        };

        // 在所有占用的格子上放置家具层
        foreach (var cellOffset in furnitureData.occupiedCells)
        {
            Vector2Int cellPos = anchorPos + cellOffset;
            instance.occupiedCells.Add(cellPos);
            
            if (cells.TryGetValue(cellPos, out var cell))
            {
                var furnitureLayer = new FurnitureLayer
                {
                    furnitureInstanceId = instanceId,
                    blocked = furnitureData.blocksMovement,
                };
                
                cell.SetFurniture(furnitureLayer);
            }
        }

        furnitureInstances[instanceId] = instance;
        return true;
    }

    // 公共方法：放置家具并更新walkableCells
    public bool PlaceFurniture(Vector2Int anchorPos, string furnitureDataId)
    {
        bool success = PlaceFurnitureInternal(anchorPos, furnitureDataId);
        if (success)
        {
            UpdateWalkableCells();
        }
        return success;
    }

    // 移除家具
    public bool RemoveFurniture(Vector2Int position)
    {
        if (!cells.TryGetValue(position, out var cell) || cell.Furniture == null)
            return false;

        string instanceId = cell.Furniture.furnitureInstanceId;
        
        if (!furnitureInstances.TryGetValue(instanceId, out var instance))
            return false;

        // 清除所有占用格子的家具层
        foreach (var cellPos in instance.occupiedCells)
        {
            if (cells.TryGetValue(cellPos, out var occupiedCell))
            {
                occupiedCell.SetFurniture(null);
            }
        }

        furnitureInstances.Remove(instanceId);
        UpdateWalkableCells();
        return true;
    }

    // 检查是否可以放置家具，TODO应该考虑玩家看到的白色格子状态
    private bool CanPlaceFurniture(Vector2Int anchorPos, FurnitureData furnitureData)
    {
        foreach (var cellOffset in furnitureData.occupiedCells)
        {
            Vector2Int checkPos = anchorPos + cellOffset;
            
            // 检查格子是否存在
            if (!cells.TryGetValue(checkPos, out var cell))
                return false;

            // 检查是否已有家具
            if (cell.Furniture != null)
                return false;
        }
        return true;
    }

    // 放置装饰
    public bool PlaceDecor(Vector2Int position, string decorId)
    {
        if (!cells.TryGetValue(position, out var cell))
            return false;

        var decor = new DecorLayer
        {
            decorId = decorId,
            decorObject = null
        };

        cell.SetDecor(decor);
        return true;
    }

    // 移除装饰
    public bool RemoveDecor(Vector2Int position)
    {
        if (!cells.TryGetValue(position, out var cell))
            return false;

        cell.SetDecor(null);
        return true;
    }

    // 获取家具实例
    public FurnitureInstance GetFurnitureInstance(string instanceId)
    {
        return furnitureInstances.GetValueOrDefault(instanceId);
    }

    // 获取位置上的家具实例
    public FurnitureInstance GetFurnitureInstanceAtPosition(Vector2Int position)
    {
        if (!cells.TryGetValue(position, out var cell) || cell.Furniture == null)
            return null;

        return furnitureInstances.GetValueOrDefault(cell.Furniture.furnitureInstanceId);
    }

    // 绑定GameObject到家具实例的锚点格子
    public void BindFurnitureObject(string instanceId, GameObject furnitureObject)
    {
        if (!furnitureInstances.TryGetValue(instanceId, out var instance))
            return;

        if (cells.TryGetValue(instance.anchorPos, out var anchorCell) && anchorCell.Furniture != null)
        {
            anchorCell.Furniture.furnitureObject = furnitureObject;
        }
    }

    public void BindDecorObject(Vector2Int position, GameObject decorObject)
    {
        if (cells.TryGetValue(position, out var cell) && cell.Decor != null)
        {
            cell.Decor.decorObject = decorObject;
        }
    }

    // 获取所有家具实例（用于渲染）
    public IEnumerable<FurnitureInstance> GetAllFurnitureInstances()
    {
        return furnitureInstances.Values;
    }

    // 获取所有装饰（用于渲染）
    public IEnumerable<(Vector2Int position, DecorLayer decor)> GetAllDecors()
    {
        foreach (var kv in cells)
        {
            if (kv.Value.Decor != null)
                yield return (kv.Key, kv.Value.Decor);
        }
    }

    // 更新可行走区域 - 只在需要时调用
    private void UpdateWalkableCells()
    {
        walkableCells.Clear();
        
        foreach (var kv in cells)
        {
            if (kv.Value.CanWalk())
                walkableCells.Add(kv.Key);
        }
    }

    // 批量操作完成后手动调用
    public void RefreshWalkableCells()
    {
        UpdateWalkableCells();
    }

    public Vector2Int GetRandomWalkablePos()
    {
        if (walkableCells.Count == 0) return Vector2Int.zero;
        int index = Random.Range(0, walkableCells.Count);
        return walkableCells[index];
    }

    public void SetOccupied(Vector2Int pos, bool occupied)
    {
        if (!cells.TryGetValue(pos, out var cell)) return;
        
        cell.SetOccupied(occupied);

        if (occupied)
            walkableCells.Remove(pos);
        else if (cell.CanWalk())
            walkableCells.Add(pos);
    }

    public bool CanWalk(Vector2Int pos)
    {
        if (!cells.TryGetValue(pos, out var cell))
            return false;

        return cell.CanWalk();
    }

    public RoomData ToSaveData()
    {
        var data = new RoomData
        {
            cells = new List<CellSaveData>(),
            furnitureInstances = new List<FurnitureInstanceSaveData>()
        };

        // 保存格子数据
        foreach (var kv in cells)
        {
            var pos = kv.Key;
            var cell = kv.Value;

            data.cells.Add(new CellSaveData
            {
                x = pos.x,
                y = pos.y,
                floorTileId = cell.Floor?.floorTileId,
                decorId = cell.Decor?.decorId,
                furnitureInstanceId = cell.Furniture?.furnitureInstanceId
            });
        }

        // 保存家具实例数据
        foreach (var instance in furnitureInstances.Values)
        {
            data.furnitureInstances.Add(new FurnitureInstanceSaveData
            {
                instanceId = instance.instanceId,
                furnitureDataId = instance.furnitureDataId,
                anchorX = instance.anchorPos.x,
                anchorY = instance.anchorPos.y
            });
        }

        return data;
    }

    public static GridMap FromSaveData(RoomData data)
    {
        var map = new GridMap();
        map.cells.Clear();
        map.walkableCells.Clear();
        map.furnitureInstances.Clear();

        // 首先创建所有基础格子（不包含家具）
        foreach (var c in data.cells)
        {
            Vector2Int pos = new Vector2Int(c.x, c.y);
            GroundLayer ground = string.IsNullOrWhiteSpace(c.floorTileId) ? null : map.CreateGround(c.floorTileId);
            DecorLayer decor = string.IsNullOrWhiteSpace(c.decorId) ? null : map.CreateDecor(c.decorId);

            map.cells[pos] = new GridCell(pos, ground, null, decor);
        }

        // 然后恢复家具实例
        if (data.furnitureInstances != null)
        {
            foreach (var instanceData in data.furnitureInstances)
            {
                Vector2Int anchorPos = new Vector2Int(instanceData.anchorX, instanceData.anchorY);
                map.PlaceFurnitureInternal(anchorPos, instanceData.furnitureDataId); // 使用内部方法，不触发UpdateWalkableCells
                
                // 更新实例ID以匹配保存的数据
                var newInstance = map.furnitureInstances.Values.Last();
                map.furnitureInstances.Remove(newInstance.instanceId);
                newInstance.instanceId = instanceData.instanceId;
                map.furnitureInstances[instanceData.instanceId] = newInstance;
                
                // 更新所有相关格子的实例ID
                foreach (var cellPos in newInstance.occupiedCells)
                {
                    if (map.cells.TryGetValue(cellPos, out var cell) && cell.Furniture != null)
                    {
                        cell.Furniture.furnitureInstanceId = instanceData.instanceId;
                    }
                }
            }
        }

        // 找到下一个可用的实例ID
        if (map.furnitureInstances.Count > 0)
        {
            var maxId = map.furnitureInstances.Keys
                .Where(id => id.StartsWith("furniture_"))
                .Select(id => int.Parse(id.Substring(10)))
                .DefaultIfEmpty(0)
                .Max();
            map.nextFurnitureInstanceId = maxId + 1;
        }

        // 最后统一更新可行走区域
        map.UpdateWalkableCells();
        return map;
    }
}

// 家具实例类
public class FurnitureInstance
{
    public string instanceId;
    public string furnitureDataId;
    public Vector2Int anchorPos;
    public List<Vector2Int> occupiedCells;
}
