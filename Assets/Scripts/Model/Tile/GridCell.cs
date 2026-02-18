using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#nullable enable

//public class GridCell
//{
//    public Vector2Int Pos { get; set; }

//    public GroundLayer? Floor { get; private set; }
//    public FurnitureLayer? Furniture { get; private set; }
//    public DecorLayer? Decor { get; private set; }

//    public GridCell(Vector2Int pos, GroundLayer? ground = null, FurnitureLayer? furniture = null, DecorLayer? decor = null)
//    {
//        Pos = pos;
//        Floor = ground;
//        Furniture = furniture;
//        Decor = decor;
//    }

//    public bool CanWalk()
//    {
//        if (Floor == null)
//            return false;
//        if (!Floor.walkable)
//            return false;
//        if (Furniture != null && Furniture.blocked)
//            return false;
//        return true;
//    }

//    public void SetOccupied(bool occupied)
//    {
//        if (Furniture != null)
//            Furniture.blocked = occupied;
//    }

//    public void SetFloor(GroundLayer ground)
//    {
//        Floor = ground;
//    }

//    public void SetFurniture(FurnitureLayer furniture)
//    {
//        Furniture = furniture;
//    }

//    public void SetDecor(DecorLayer decor)
//    {
//        Decor = decor;
//    }
//}
//#nullable restore

//public class GroundLayer
//{
//    public string floorTileId;
//    public bool walkable;
//}

//// 家具层 - 每个格子只记录它属于哪个家具实例
//public class FurnitureLayer
//{
//    public string furnitureInstanceId; // 家具实例ID，同一个家具的所有格子共享此ID
//    public bool blocked;
//    public Vector2Int anchorPos; // 该家具的锚点位置（左下角格子）
//}

//public class DecorLayer
//{
//    public string decorInstanceId;
//}

