using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#nullable enable

public class GridCell
{
    public Vector2Int Pos { get; set; }

    public GroundLayer? Floor { get; private set; }
    public FurnitureLayer? Furniture { get; private set; }
    public DecorLayer? Decor { get; private set; }

    public GridCell(Vector2Int pos,GroundLayer? ground = null,FurnitureLayer? furniture = null,DecorLayer? decor = null)
    {
        Pos = pos;
        Floor = ground;
        Furniture = furniture;
        Decor = decor;
    }

    public bool CanWalk()
    {
        if (Floor == null)
            return false;
        if (!Floor.walkable)
            return false;
        if (Furniture != null && Furniture.blocked)
            return false;
        return true;
    }

    public void SetOccupied(bool occupied)
    {
        if (Furniture != null)
            Furniture.blocked = occupied;
    }

    public void SetFloor(GroundLayer ground)
    {
        Floor = ground;
    }

    public void SetFurniture(FurnitureLayer furniture)
    {
        Furniture = furniture;
    }

    public void SetDecor(DecorLayer decor)
    {
        Decor = decor;
    }
}
#nullable restore

public class GroundLayer
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

