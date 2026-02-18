using System.Collections.Generic;
using UnityEngine;

public class ChunkWorld
{
    private readonly Dictionary<Vector2Int, Chunk> chunks = new();

    public IReadOnlyDictionary<Vector2Int, Chunk> DebugChunks()
    => chunks;

    public CellData GetCell(Vector2Int worldPos)
    {
        var chunkCoord = WorldToChunk(worldPos);
        if (!chunks.TryGetValue(chunkCoord, out var chunk))
            return default;

        var local = WorldToLocal(worldPos);
        return chunk.GetCell(local.x, local.y);
    }

    public ref CellData GetCellRef(Vector2Int worldPos)
    {
        var chunk = GetOrCreateChunk(WorldToChunk(worldPos));
        var local = WorldToLocal(worldPos);
        return ref chunk.GetCell(local.x, local.y);
    }

    private Chunk GetOrCreateChunk(Vector2Int coord)
    {
        if (!chunks.TryGetValue(coord, out var chunk))
        {
            chunk = new Chunk(coord);
            chunks.Add(coord, chunk);
        }
        return chunk;
    }

    private static Vector2Int WorldToChunk(Vector2Int pos)
        => new(DivFloor(pos.x, Chunk.CHUNK_SIZE),DivFloor(pos.y, Chunk.CHUNK_SIZE));

    private static Vector2Int WorldToLocal(Vector2Int pos)
    => new(Mod(pos.x, Chunk.CHUNK_SIZE), Mod(pos.y, Chunk.CHUNK_SIZE)); 
    
    private static int Mod(int a, int m) { int r = a % m; return r < 0 ? r + m : r; }
    private static int DivFloor(int a, int size)
    {
        if (a >= 0) return a / size;
        return -(((-a - 1) / size) + 1);
    }

}
