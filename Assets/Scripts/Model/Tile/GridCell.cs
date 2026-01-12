using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell
{
    public Vector2Int pos;

    public FloorLayer floor;        // 地面
    public FurnitureLayer furniture; // 家具
    public DecorLayer decor;


    public bool CanWalk()
    {
        if (floor == null)
            return false;
        if (!floor.walkable)
            return false;
        if (furniture != null && furniture.blocked)
            return false;
        return true;
    }

    public void SetOccupied(bool occupied)
    {
        if (furniture != null)
            furniture.blocked = occupied;
    }
}


public class FloorLayer
{
    public string floorTileId;
    public bool walkable;
}

public class FurnitureLayer
{
    public bool blocked;
    public string furnitureTileId; 
}

public class DecorLayer
{
    public string decorTileId; 
}