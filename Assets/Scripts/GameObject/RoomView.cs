using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomView : MonoBehaviour
{
    private Dictionary<string, TileBase> roomCache = new();
    private FurnitureDatabase furnitureDatabase => GameManager.Instance.FurnitureDatabase;

    [SerializeField] private Tilemap groundMap;
    [SerializeField] private TileLoader tileLoader;
    
    // GameObject容器
    [SerializeField] private Transform furnitureContainer;
    [SerializeField] private Transform decorContainer;
    
    // 默认预制件（只用于没有Sprite的情况）
    [SerializeField] private GameObject defaultFurniturePrefab;
    [SerializeField] private GameObject defaultDecorPrefab;
    
    private Dictionary<string, GameObject> furnitureObjects = new();
    private Dictionary<Vector2Int, GameObject> decorObjects = new();
    private Dictionary<string, Sprite> userSpriteCache = new(); // 缓存用户自定义图片
    private GridMap currentGridMap;

    public void Initialize(GridMap gridMap)
    {
        currentGridMap = gridMap;
        roomCache.Clear();
        ClearExistingObjects();
        RenderRoom(gridMap);
    }
    
    private void ClearExistingObjects()
    {
        foreach (var obj in furnitureObjects.Values)
        {
            if (obj != null) DestroyImmediate(obj);
        }
        furnitureObjects.Clear();
        
        foreach (var obj in decorObjects.Values)
        {
            if (obj != null) DestroyImmediate(obj);
        }
        decorObjects.Clear();
    }

    private void RenderRoom(GridMap gridMap)
    {
        RenderFloors(gridMap);
        RenderFurnitureInstances(gridMap);
        RenderDecors(gridMap);
    }
    
    private void RenderFloors(GridMap gridMap)
    {
        foreach (var cellEntry in gridMap.cells)
        {
            Vector2Int pos = cellEntry.Key;
            GridCell cell = cellEntry.Value;
            Vector3Int tilemapPos = new Vector3Int(pos.x, pos.y, 0);

            if (cell.Floor != null)
            {
                TileBase groundTile = GetTileFromCache(cell.Floor.floorTileId);
                groundMap.SetTile(tilemapPos, groundTile);
            }
        }
    }
    
    private void RenderFurnitureInstances(GridMap gridMap)
    {
        foreach (var furnitureInstance in gridMap.GetAllFurnitureInstances())
        {
            RenderFurnitureInstance(furnitureInstance);
        }
    }
    
    private void RenderFurnitureInstance(FurnitureInstance instance)
    {
        var furnitureData = furnitureDatabase.GetFurnitureData(instance.furnitureDataId);
        if (furnitureData == null) return;

        // 创建GameObject
        var furnitureObj = CreateFurnitureGameObject(furnitureData);
        if (furnitureObj == null) return;
        
        // 设置父级
        furnitureObj.transform.SetParent(furnitureContainer);
        
        // 计算锚点的世界位置
        Vector3Int tilemapPos = new Vector3Int(instance.anchorPos.x, instance.anchorPos.y, 0);
        Vector3 anchorWorldPos = groundMap.CellToWorld(tilemapPos) + Vector3.one * 0.5f;
        
        // 设置位置（锚点位置 + 渲染偏移）
        furnitureObj.transform.position = anchorWorldPos + (Vector3)furnitureData.renderOffset;
        furnitureObj.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        // 设置渲染层级（基于锚点的Y坐标）
        var spriteRenderer = furnitureObj.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 0;
                //furnitureData.sortingOrder - instance.anchorPos.y;
        }
        
        furnitureObj.SetActive(true);
        
        // 缓存GameObject并绑定到GridMap
        furnitureObjects[instance.instanceId] = furnitureObj;
        currentGridMap.BindFurnitureObject(instance.instanceId, furnitureObj);
    }
    
    private void RenderDecors(GridMap gridMap)
    {
        foreach (var (position, decor) in gridMap.GetAllDecors())
        {
            RenderDecor(position, decor);
        }
    }
    
    private void RenderDecor(Vector2Int gridPos, DecorLayer decor)
    {
        var decorData = furnitureDatabase.GetDecorData(decor.decorId);
        if (decorData == null) return;
        
        // 直接创建GameObject
        var decorObj = CreateDecorGameObject(decorData);
        if (decorObj == null) return;
        
        // 设置父级
        decorObj.transform.SetParent(decorContainer);
        
        Vector3Int tilemapPos = new Vector3Int(gridPos.x, gridPos.y, 0);
        Vector3 worldPos = groundMap.CellToWorld(tilemapPos) + Vector3.one * 0.5f;
        
        decorObj.transform.position = worldPos + (Vector3)decorData.renderOffset;
        
        var spriteRenderer = decorObj.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = decorData.sortingOrder - gridPos.y;
        }
        
        decorObj.SetActive(true);
        decorObjects[gridPos] = decorObj;
        currentGridMap.BindDecorObject(gridPos, decorObj);
    }

    // 直接创建家具GameObject
    private GameObject CreateFurnitureGameObject(FurnitureData furnitureData)
    {
        Sprite sprite = GetFurnitureSprite(furnitureData);
        if (sprite == null) return null;
        
        // 创建GameObject
        var go = new GameObject($"Furniture_{furnitureData.id}");
        
        // 添加SpriteRenderer
        var spriteRenderer = go.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        
        return go;
    }
    
    // 直接创建装饰GameObject
    private GameObject CreateDecorGameObject(DecorData decorData)
    {
        Sprite sprite = GetDecorSprite(decorData);
        if (sprite == null) return null;
        
        // 创建GameObject
        var go = new GameObject($"Decor_{decorData.id}");
        
        // 添加SpriteRenderer
        var spriteRenderer = go.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        
        return go;
    }
    
    // 获取家具Sprite
    private Sprite GetFurnitureSprite(FurnitureData furnitureData)
    {
        // 优先使用ScriptableObject中的Sprite
        if (furnitureData.sprite != null)
            return furnitureData.sprite;
        
        // 如果是用户自定义家具，从文件加载
        if (!string.IsNullOrEmpty(furnitureData.spritePath))
        {
            return LoadSpriteFromPath(furnitureData.spritePath);
        }
        
        return null;
    }
    
    // 获取装饰Sprite
    private Sprite GetDecorSprite(DecorData decorData)
    {
        // 优先使用ScriptableObject中的Sprite
        if (decorData.sprite != null)
            return decorData.sprite;
        
        // 如果是用户自定义装饰，从文件加载
        if (!string.IsNullOrEmpty(decorData.spritePath))
        {
            return LoadSpriteFromPath(decorData.spritePath);
        }
        
        return null;
    }
    
    // 从文件路径加载Sprite（仅用于用户自定义）
    private Sprite LoadSpriteFromPath(string path)
    {
        if (userSpriteCache.TryGetValue(path, out var cachedSprite))
            return cachedSprite;
        
        if (System.IO.File.Exists(path))
        {
            byte[] fileData = System.IO.File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            
            if (texture.LoadImage(fileData))
            {
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 100);
                userSpriteCache[path] = sprite;
                return sprite;
            }
        }
        
        return null;
    }

    private TileBase GetTileFromCache(string tileId)
    {
        if (roomCache.TryGetValue(tileId, out var tile))
            return tile;

        tile = tileLoader.LoadTile(tileId);
        if (tile != null)
            roomCache[tileId] = tile;

        return tile;
    }
    
    // 动态操作方法
    public void PlaceFurniture(Vector2Int anchorPos, string furnitureDataId)
    {
        if (currentGridMap == null) return;
        
        if (currentGridMap.PlaceFurniture(anchorPos, furnitureDataId))
        {
            var furnitureInstance = currentGridMap.GetFurnitureInstanceAtPosition(anchorPos);
            if (furnitureInstance != null)
            {
                RenderFurnitureInstance(furnitureInstance);
            }
        }
    }
    
    public void RemoveFurniture(Vector2Int position)
    {
        if (currentGridMap == null) return;
        
        var furnitureInstance = currentGridMap.GetFurnitureInstanceAtPosition(position);
        if (furnitureInstance == null) return;
        
        string instanceId = furnitureInstance.instanceId;
        
        if (currentGridMap.RemoveFurniture(position))
        {
            if (furnitureObjects.TryGetValue(instanceId, out var obj))
            {
                DestroyImmediate(obj);
                furnitureObjects.Remove(instanceId);
            }
        }
    }
    
    public void PlaceDecor(Vector2Int position, string decorId)
    {
        if (currentGridMap == null) return;
        
        if (currentGridMap.PlaceDecor(position, decorId))
        {
            if (currentGridMap.cells.TryGetValue(position, out var cell) && cell.Decor != null)
            {
                RenderDecor(position, cell.Decor);
            }
        }
    }
    
    public void RemoveDecor(Vector2Int position)
    {
        if (currentGridMap == null) return;
        
        if (currentGridMap.RemoveDecor(position))
        {
            if (decorObjects.TryGetValue(position, out var obj))
            {
                DestroyImmediate(obj);
                decorObjects.Remove(position);
            }
        }
    }
    
    public FurnitureInstance GetFurnitureAtWorldPosition(Vector3 worldPosition)
    {
        if (currentGridMap == null) return null;
        
        Vector3Int cellPos = groundMap.WorldToCell(worldPosition);
        Vector2Int gridPos = new Vector2Int(cellPos.x, cellPos.y);
        
        return currentGridMap.GetFurnitureInstanceAtPosition(gridPos);
    }
    
    public DecorLayer GetDecorAtWorldPosition(Vector3 worldPosition)
    {
        if (currentGridMap == null) return null;
        
        Vector3Int cellPos = groundMap.WorldToCell(worldPosition);
        Vector2Int gridPos = new Vector2Int(cellPos.x, cellPos.y);
        
        if (currentGridMap.cells.TryGetValue(gridPos, out var cell))
        {
            return cell.Decor;
        }
        
        return null;
    }
}
