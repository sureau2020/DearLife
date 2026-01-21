
using System.Collections.Generic;

public class TileDataBase
{
    private static readonly Dictionary<string, TileData> tileMap = new(){
        { "grass", new TileData { id = "grass", belongsTo = "nature", walkable = true } },
        { "water", new TileData { id = "water", belongsTo = "nature", walkable = false } },
        { "sand", new TileData { id = "sand", belongsTo = "nature", walkable = true } },
        { "stone", new TileData { id = "stone", belongsTo = "nature", walkable = false } },
        { "road", new TileData { id = "road", belongsTo = "manmade", walkable = true } },
        { "wall", new TileData { id = "wall", belongsTo = "manmade", walkable = false } }
    };


    public static TileData GetTileById(string id)
    {
        return tileMap[id];
    }


}

