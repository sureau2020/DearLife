using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridMap
{
    
    public Dictionary<Vector2Int, GridCell> cells = new();
    public List<Vector2Int> walkableCells;

    // 初始化地图数据，从Tilemap中读取,build walkablecells TODO
    public void Initialize() { 
        
    }

    public Vector2Int GetRandomWalkablePos()
    {
        int index = Random.Range(0, walkableCells.Count);
        return walkableCells[index];
    }


    public void BuildWalkableList()
    {
        walkableCells = new List<Vector2Int>();

        foreach (var kv in cells)
        {
            GridCell cell = kv.Value;
            if (cell.CanWalk())
                walkableCells.Add(cell.pos);
        }
    }

    public void SetOccupied(Vector2Int pos, bool occupied)
    {
        var cell = cells[pos];
        cell.SetOccupied(occupied);

        if (occupied)
            walkableCells.Remove(pos);
        else if (cell.CanWalk())
            walkableCells.Add(pos);
    }



    public bool CanWalk(Vector2Int pos)
    {
        if (!cells.TryGetValue(pos, out var cell))
            return false;

        return cell.CanWalk();
    }

}
