
using UnityEngine.Tilemaps;

[System.Serializable]
public class TileData 
{
    public string id;
    public bool walkable;
}

[System.Serializable]
public class TileEntry
{
    public string tileId;
    public TileBase tile;
}
