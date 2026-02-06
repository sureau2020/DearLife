using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomData
{
    public int width;
    public int height;
    public List<CellSaveData> cells;
    public List<FurnitureInstanceSaveData> furnitureInstances;
}

[System.Serializable]
public class CellSaveData
{
    public int x;
    public int y;
    public string floorTileId;
    public string decorId;
    public string furnitureInstanceId; // 指向家具实例ID，而不是直接的家具数据ID
}

[System.Serializable] 
public class FurnitureInstanceSaveData
{
    public string instanceId;
    public string furnitureDataId;
    public int anchorX;
    public int anchorY;
}
