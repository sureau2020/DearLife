using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 家具实例类
public class FurnitureInstance
{
    public string instanceId;
    public string furnitureDataId;
    public Vector2Int anchorPos;
    public List<Vector2Int> occupiedCells;
    public GameObject furnitureObject;// 只有锚点格子才有GameObject引用

    public override string ToString() { 
        return $"FurnitureInstance: instanceId={instanceId}, furnitureDataId={furnitureDataId}, anchorPos={anchorPos}, occupiedCells=[{string.Join(", ", occupiedCells)}], objectPos = [{furnitureObject.transform.position}]";
    }



}

