using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData
{
    public int width;
    public int height;
    public List<CellSaveData> cells;
}

public class CellSaveData
{
    public int x;
    public int y;
    public string floorTileId;
    public string furnitureTileId;
    public string decorTileId;
}
