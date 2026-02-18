

using System.Drawing;
using UnityEngine;

public class Chunk
{
    public readonly Vector2Int chunkCoord;
    public const int CHUNK_SIZE = 16;
    public CellData[,] cells;


    public Chunk(Vector2Int coord)
    {
        cells = new CellData[CHUNK_SIZE, CHUNK_SIZE];
        chunkCoord = coord;

        for (int x = 0; x < CHUNK_SIZE; x++)
            for (int y = 0; y < CHUNK_SIZE; y++)
                cells[x, y].furnitureInstanceId = "";
    }

    public ref CellData GetCell(int localX, int localY)
    {
        return ref cells[localX, localY];
    }

    public void SetCell(int x, int y, CellData cell)
    {
        cells[x, y] = cell;
    }
}


public struct CellData
{
    public CellFlags flags;
    public string furnitureInstanceId; // 空字符串表示无家具
    public string floorTileId; // null 或空字符串表示无地板
    public string decorInstanceId; // null 或空字符串表示无装饰

    public bool Has(CellFlags flag) => (flags & flag) != 0;
}

struct ChunkCoord
{
    public int cx;
    public int cy;
}

public enum CellFlags : byte
{
    None = 0,
    HasFloor = 1 << 0,
    FloorWalkable = 1 << 1,
    HasFurniture = 1 << 2,
    FurnitureBlocked = 1 << 3,
}