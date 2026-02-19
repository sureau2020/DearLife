using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomView : MonoBehaviour
{
    private Dictionary<string, TileBase> tileCache = new();

    private FurnitureDatabase furnitureDatabase
        => GameManager.Instance.FurnitureDatabase;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap groundMap;
    [SerializeField] private Tilemap cellsMap;
    [SerializeField] private TileLoader tileLoader;

    [Header("Containers")]
    [SerializeField] private Transform furnitureContainer;
    [SerializeField] private Transform decorContainer;

    [Header("Debug")]
    [SerializeField] private Sprite cellSprite;

    private GridMap gridMap;

    private readonly Dictionary<string, GameObject> furnitureObjects = new();
    private readonly Dictionary<string, GameObject> decorObjects = new();
    private readonly Dictionary<string, Sprite> userSpriteCache = new();

    // =========================
    // Init
    // =========================

    public void Initialize(GridMap map)
    {
        gridMap = map;

        tileCache.Clear();
        ClearAll();

        RenderAll();
    }

    private void ClearAll()
    {
        groundMap.ClearAllTiles();
        cellsMap.ClearAllTiles();

        foreach (var go in furnitureObjects.Values)
            if (go) DestroyImmediate(go);
        furnitureObjects.Clear();

        foreach (var go in decorObjects.Values)
            if (go) DestroyImmediate(go);
        decorObjects.Clear();
    }

    // =========================
    // Render Entry
    // =========================

    private void RenderAll()
    {
        RenderFloors();
        RenderFurniture();
        RenderDecor();
    }

    // =========================
    // Floor
    // =========================

    private void RenderFloors()
    {
        foreach (var pos in gridMap.GetAllWalkableCells()) { } // force chunks alive

        foreach (var kv in gridMap.DebugAllCells())
        {
            Vector2Int pos = kv.Key;
            CellData cell = kv.Value;

            if (!cell.Has(CellFlags.HasFloor)) continue;

            var tile = GetTile(cell.floorTileId);
            if (tile == null) continue;

            groundMap.SetTile(
                new Vector3Int(pos.x, pos.y, 0),
                tile
            );
        }
    }

    // =========================
    // Furniture
    // =========================

    private void RenderFurniture()
    {
        foreach (var inst in gridMap.GetAllFurnitureInstances())
            RenderFurnitureInstance(inst);
    }

    private void RenderFurnitureInstance(FurnitureInstance inst)
    {
        var data = furnitureDatabase.GetFurnitureData(inst.furnitureDataId);
        if (data == null) return;

        Sprite sprite = GetFurnitureSprite(data);
        if (sprite == null) return;

        GameObject go = new($"Furniture_{inst.instanceId}");
        go.transform.SetParent(furnitureContainer);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = -inst.anchorPos.y - data.GetMaxOccupiedY();

        Vector3Int cellPos = new(inst.anchorPos.x, inst.anchorPos.y, 0);
        Vector3 worldPos = groundMap.CellToWorld(cellPos);

        go.transform.position = worldPos + (Vector3)data.renderOffset;
        go.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        furnitureObjects[inst.instanceId] = go;
    }

    // =========================
    // Decor
    // =========================

    private void RenderDecor()
    {
        foreach (var kv in gridMap.GetAllDecorInstances())
        {
            RenderDecorInstance(kv);
        }
    }

    private void RenderDecorInstance(DecorInstance decor)
    {
        var data = furnitureDatabase.GetDecorData(decor.decorId);
        if (data == null) return;

        Sprite sprite = GetDecorSprite(data);
        if (sprite == null) return;

        GameObject go = new($"Decor_{decor.instanceId}");
        go.transform.SetParent(decorContainer);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = -decor.position.y + 1; // render above furniture

        Vector3Int cellPos = new(decor.position.x, decor.position.y, 0);
        Vector3 worldPos = groundMap.CellToWorld(cellPos);

        go.transform.position = worldPos + (Vector3)data.renderOffset;
        go.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        decorObjects[decor.instanceId] = go;
    }

    // =========================
    // Walkable Overlay
    // =========================

    public void RenderWalkableOverlay()
    {
        foreach (var kv in gridMap.DebugAllCells())
        {
            Vector2Int pos = kv.Key;

            var tile = gridMap.CanWalk(pos)
                ? GetTile("White_DefaultCell")
                : GetTile("Red_DefaultCell");

            cellsMap.SetTile(
                new Vector3Int(pos.x, pos.y, 0),
                tile
            );
        }
    }

    public void ClearCells()
    {
        cellsMap.ClearAllTiles();
    }

    public void RenderFurnitureLayer() {
        foreach (var kv in gridMap.DebugAllCells())
        {
            Vector2Int pos = kv.Key;

            if (gridMap.HasFurniture(pos))
            {
                cellsMap.SetTile(
                   new Vector3Int(pos.x, pos.y, 0), GetTile("Red_DefaultCell")
               );
                continue;
            }

            if (gridMap.HasWalkbleFLoor(pos)) {
                cellsMap.SetTile(
                   new Vector3Int(pos.x, pos.y, 0), GetTile("White_DefaultCell")
               );
                continue;
            }
        }
    }

    public void RenderDecorLayer() {
        foreach (var kv in gridMap.DebugAllCells())
        {
            Vector2Int pos = kv.Key;

            if (gridMap.HasFloor(pos))
            {
                cellsMap.SetTile(
                   new Vector3Int(pos.x, pos.y, 0), GetTile("White_DefaultCell")
               );
                continue;
            }
        }
        foreach (var kv in gridMap.GetAllDecorInstances())
        {
            Vector2Int pos = kv.position;
            cellsMap.SetTile(
               new Vector3Int(pos.x, pos.y, 0), GetTile("Red_DefaultCell")
           );
        }
    }

    public void RenderFloorLayer() {
        foreach (var kv in gridMap.DebugAllCells())
        {
            Vector2Int pos = kv.Key;
            cellsMap.SetTile( new Vector3Int(pos.x, pos.y, 0), GetTile("White_DefaultCell"));
        }
    }

    // =========================
    // Runtime Ops
    // =========================

    public void PlaceFurniture(Vector2Int anchorPos, string furnitureId)
    {
        var data = furnitureDatabase.GetFurnitureData(furnitureId);
        if (data == null) return;

        if (!gridMap.PlaceFurniture(anchorPos, data)) return;

        var inst = gridMap.GetFurnitureAt(anchorPos);
        if (inst != null)
            RenderFurnitureInstance(inst);
    }

    public void RemoveFurniture(Vector2Int pos)
    {
        var inst = gridMap.GetFurnitureAt(pos);
        if (inst == null) return;

        if (!gridMap.RemoveFurniture(pos)) return;

        if (furnitureObjects.TryGetValue(inst.instanceId, out var go))
        {
            DestroyImmediate(go);
            furnitureObjects.Remove(inst.instanceId);
        }
    }

    // TODO
    public void PlaceDecor(Vector2Int pos, string decorId)
    {
        gridMap.SetDecor(pos, decorId);
        //RenderDecorInstance(pos, decorId);
        //TODO
    }

    public void RemoveDecor(Vector2Int pos)
    {
        gridMap.RemoveDecor(pos);

        string key = $"{pos.x},{pos.y}";
        if (decorObjects.TryGetValue(key, out var go))
        {
            DestroyImmediate(go);
            decorObjects.Remove(key);
        }
    }

    // =========================
    // Helpers
    // =========================

    private TileBase GetTile(string id)
    {
        if (tileCache.TryGetValue(id, out var tile))
            return tile;

        tile = tileLoader.LoadTile(id);
        if (tile != null)
            tileCache[id] = tile;

        return tile;
    }

    private Sprite GetFurnitureSprite(FurnitureData data)
    {
        if (data.sprite != null) return data.sprite;
        if (!string.IsNullOrEmpty(data.spritePath))
            return LoadSprite(data.spritePath);
        return null;
    }

    private Sprite GetDecorSprite(DecorData data)
    {
        if (data.sprite != null) return data.sprite;
        if (!string.IsNullOrEmpty(data.spritePath))
            return LoadSprite(data.spritePath);
        return null;
    }

    private Sprite LoadSprite(string path)
    {
        if (userSpriteCache.TryGetValue(path, out var s))
            return s;

        if (!System.IO.File.Exists(path))
            return null;

        byte[] bytes = System.IO.File.ReadAllBytes(path);
        var tex = new Texture2D(2, 2);
        if (!tex.LoadImage(bytes)) return null;

        var sprite = Sprite.Create(
            tex,
            new Rect(0, 0, tex.width, tex.height),
            Vector2.one * 0.5f,
            100
        );

        userSpriteCache[path] = sprite;
        return sprite;
    }


    public Vector2Int WorldToCell(Vector3 pos) { 
        return new Vector2Int(groundMap.WorldToCell(pos).x,groundMap.WorldToCell(pos).y);
    }
}