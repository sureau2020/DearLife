using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomView : MonoBehaviour
{
    private Dictionary<string, TileBase> roomCache = new();

    [SerializeField] private Tilemap decorMap;
    [SerializeField] private Tilemap fornitureMap;
    [SerializeField] private Tilemap groundMap;

    [SerializeField] private TileLoader tileLoader;

    // 初始化时传入 GridMap + TileLoader
    public void Initialize(GridMap gridMap)
    {
        roomCache.Clear();
        RenderRoom(gridMap);
    }

    private void RenderRoom(GridMap gridMap)
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

            if (cell.Furniture != null)
            {
                TileBase furnitureTile = GetTileFromCache(cell.Furniture.furnitureTileId);
                fornitureMap.SetTile(tilemapPos, furnitureTile);
            }

            if (cell.Decor != null)
            {
                TileBase decorTile = GetTileFromCache(cell.Decor.decorTileId);
                decorMap.SetTile(tilemapPos, decorTile);
            }
        }
    }

    private TileBase GetTileFromCache(string tileId)
    {
        if (roomCache.TryGetValue(tileId, out var tile))
            return tile;

        // 不在房间缓存，从 loader 拿
        tile = tileLoader.LoadTile(tileId);
        if (tile != null)
            roomCache[tileId] = tile;

        return tile;
    }
}
