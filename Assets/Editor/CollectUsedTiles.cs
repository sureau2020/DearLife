using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Collections.Generic;

public static class CollectUsedTiles
{
    [MenuItem("Tools/Tilemap/Print Used Tile Names")]
    public static void PrintUsedTileNames()
    {
        var tilemaps = Selection.GetFiltered<Tilemap>(SelectionMode.Editable);

        if (tilemaps.Length == 0)
        {
            Debug.LogWarning("No Tilemap selected.");
            return;
        }

        HashSet<TileBase> usedTiles = new HashSet<TileBase>();

        foreach (var tilemap in tilemaps)
        {
            BoundsInt bounds = tilemap.cellBounds;
            foreach (var pos in bounds.allPositionsWithin)
            {
                TileBase tile = tilemap.GetTile(pos);
                if (tile != null)
                    usedTiles.Add(tile);
            }
        }

        if (usedTiles.Count == 0)
        {
            Debug.Log("No tiles used.");
            return;
        }

        Debug.Log($"Used TileBase count: {usedTiles.Count}");
        foreach (var tile in usedTiles)
        {
            Debug.Log(tile.name);
        }
    }
}
