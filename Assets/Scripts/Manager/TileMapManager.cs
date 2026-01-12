using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileTypeManager : MonoBehaviour
{
    private Dictionary<string, TileData> map = new();
    public GridMap gridMap { get; set; }


    void Awake()
    {
        gridMap = new GridMap();
        gridMap.Initialize();
    }




    public TileData GetTile(string id)
    {
        return map[id];
    }


}

