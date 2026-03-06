using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDatabase : MonoBehaviour
{
    private readonly Dictionary<string, MapData> maps = new();

    public MapDatabase()
    {
        LoadBuiltinData();
    }

    private void LoadBuiltinData()
    {
        var allMaps = Resources.LoadAll<MapDataSO>("Map");

        foreach (var map in allMaps)
        {
            MapData mapData = new MapData
            {
                id = map.name,
                icon = null, // 初始化不链接图标，打开家具库时再加载图标资源
                tileInstances = map.tileInstances
            };

            maps[mapData.id] = mapData;
        }

        Debug.Log($"已加载 {maps.Count} 个内置Map,");
    }

    public List<MapData> GetAllMap(List<MapData> allMap) {
        allMap.Clear();
        foreach (var item in maps.Values)
        {
            allMap.Add(item);
        }
        return allMap;
    }


    public MapData GetMapById(string id)
    {
        return maps[id];
    }

}


public class MapData
{
    public string id;
    public Sprite icon;
    public List<TileInstanceData> tileInstances;
}