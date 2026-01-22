using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomView : MonoBehaviour
{
    private Dictionary<string, TileBase> tileBaseMap = new();
    [SerializeField] private List<TileEntry> builtinTiles;

    [SerializeField] private Tilemap decorMap;
    [SerializeField] private Tilemap fornitureMap;
    [SerializeField] private Tilemap groundMap;
    // Start is called before the first frame update



    //TODO: 完成RoomView的Initialize方法
    public void Initialize(GridMap gridMap, TileDataBase tileDataBase)
    {
        BuildTileBaseMap();
        RenderRoom(gridMap);

    }

    private void BuildTileBaseMap()
    {
        tileBaseMap.Clear();
        foreach (var e in builtinTiles)
            tileBaseMap[e.tileId] = e.tile;
    }

    private void RenderRoom(GridMap gridMap)
    {
        foreach (var cellEntry in gridMap.cells)
        {
            Vector2Int pos = cellEntry.Key;
            GridCell cell = cellEntry.Value;
            Vector3Int tilemapPos = new Vector3Int(pos.x, pos.y, 0);
            // 设置地面图块
            if (cell.Floor != null)
            {
                Tile groundTile = tileBaseMap[cell.Floor.floorTileId] as Tile;
                groundMap.SetTile(tilemapPos, groundTile);
            }
            // 设置家具图块
            if (cell.Furniture != null)
            {
                Tile furnitureTile = tileBaseMap[cell.Furniture.furnitureTileId] as Tile;
                fornitureMap.SetTile(tilemapPos, furnitureTile);
            }
            // 设置装饰图块
            if (cell.Decor != null)
            {
                Tile decorTile = tileBaseMap[cell.Decor.decorTileId] as Tile;
                decorMap.SetTile(tilemapPos, decorTile);
            }
        }
    }

}

[Serializable]
public class TileEntry
{
    public string tileId;
    public TileBase tile;
}