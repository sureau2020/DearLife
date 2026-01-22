using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class GridMap
{
    
    public Dictionary<Vector2Int, GridCell> cells = new();
    public List<Vector2Int> walkableCells;

    // 默认房间，目前是硬编码的
    public GridMap()
    {
        cells = new Dictionary<Vector2Int, GridCell>();
        walkableCells = new List<Vector2Int>();

        // 布局示意（y 向上）
        // (0,2) grass | (1,2) sand  | (2,2) grass
        // (0,1) sand  | (1,1) water | (2,1) sand
        // (0,0) grass | (1,0) sand  | (2,0) grass

        AddCell(0, 0, "grass", "sand", "water");
        AddCell(1, 0, "sand","", "");
        AddCell(2, 0, "sand", "","grass");

        AddCell(0, 1, "sand", "", "");
        AddCell(1, 1, "water", "", "");
        AddCell(2, 1, "sand", "", "");

        AddCell(0, 2, "grass", "", "");
        AddCell(1, 2, "sand", "", "");
        AddCell(2, 2, "grass", "", "");
    }

    public void AddCell(int x, int y, string groundTileId, string objectTileId, string decorTileId) {
        Vector2Int pos = new Vector2Int(x, y);
        GroundLayer ground = string.IsNullOrWhiteSpace(groundTileId) ? null : CreateGround(groundTileId);
        FurnitureLayer furniture = string.IsNullOrWhiteSpace(objectTileId) ? null : CreateFurniture(objectTileId);
        DecorLayer decor = string.IsNullOrWhiteSpace(decorTileId) ? null : CreateDecor(decorTileId);

        cells[pos] = new GridCell(pos, ground, furniture, decor);

        if (ground.walkable)
            walkableCells.Add(pos);
    }

    private GroundLayer CreateGround(string tileId) {
        TileData tileData = TileDataBase.GetTileById(tileId);
        if (tileData == null) return null;
        GroundLayer groundLayer = new GroundLayer
        {
            floorTileId = tileId,
            walkable = tileData.walkable
        };
        return groundLayer;
    }

    private FurnitureLayer CreateFurniture(string tileId) {
        TileData tileData = TileDataBase.GetTileById(tileId);
        if (tileData == null) return null;
        FurnitureLayer furnitureLayer = new FurnitureLayer
        {
            furnitureTileId = tileId,
            blocked = tileData.walkable
        };
        return furnitureLayer;
    }

    private DecorLayer CreateDecor(string tileId) {
        TileData tileData = TileDataBase.GetTileById(tileId);
        if (tileData == null) return null;
        DecorLayer decorLayer = new DecorLayer
        {
            decorTileId = tileId
        };
        return decorLayer;
    }



    // 初始化地图数据，从Tilemap中读取,build walkablecells TODO
    public void Initialize() { 
        
    }

    public Vector2Int GetRandomWalkablePos()
    {
        int index = Random.Range(0, walkableCells.Count);
        return walkableCells[index];
    }


    public void SetOccupied(Vector2Int pos, bool occupied)
    {
        var cell = cells[pos];
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

}
