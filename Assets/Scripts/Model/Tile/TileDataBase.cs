
using System.Collections.Generic;
using UnityEngine;

public class TileDataBase
{
    private readonly Dictionary<string, TileData> tileMap = new();

    public TileDataBase()
    {
        LoadBuiltinData();
    }

    private void LoadBuiltinData()
    {
        List<TileData> allTileData = Resources.Load<TileConfigSO>("TileConfig").tileProperties;

        foreach (var tile in allTileData)
        {
            tileMap[tile.id] = tile;
        }

        Debug.Log($"已加载 {tileMap.Count} 个内置tile,");
    }


    public TileData GetTileById(string id)
    {
        return tileMap[id];
    }


}

