using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileLoader : MonoBehaviour
{

    // 用户导入 tile 的磁盘索引 (tileId -> path)
    private Dictionary<string, string> userDiskIndex = new();

    // 已加载的全局缓存 (tileId -> TileBase)
    private Dictionary<string, TileBase> loadedCache = new();

    private Dictionary<string, TileBase> builtinCache = new();

    private void Awake()
    {
        // 初始化内建 tile 索引
        List<TileEntry> tileEntries = Resources.Load<TileBaseLibrarySO>("TileLibrary").tileResources;
        foreach (var entry in tileEntries)
        {
            builtinCache[entry.tileId] = entry.tile;
        }
    }

    // 外部请求 tileBase
    public TileBase LoadTile(string tileId)
    {
        // 先查全局缓存
        if (loadedCache.TryGetValue(tileId, out var tile))
            return tile;

        // 查内建 cache
        if (builtinCache.TryGetValue(tileId, out tile))
        {
            loadedCache[tileId] = tile; // 加入全局缓存
            return tile;
        }

        // 查用户磁盘索引
        if (userDiskIndex.TryGetValue(tileId, out string path))
        {
            tile = LoadTileFromDisk(path);
            loadedCache[tileId] = tile;
            return tile;
        }

        Debug.LogWarning($"TileLoader: tileId {tileId} not found!");
        return null;
    }

    // 注册用户导入 tile
    public void RegisterUserTile(string tileId, string filePath)
    {
        if (!userDiskIndex.ContainsKey(tileId))
            userDiskIndex[tileId] = filePath;
    }

    // 从磁盘加载 tile（同步示例，可改为异步）
    private TileBase LoadTileFromDisk(string path)
    {
        // 使用 Resources.Load 作为示例，如果是 Addressables 或 Texture2D.LoadImage 替换即可
        TileBase tile = Resources.Load<TileBase>(path);
        if (tile == null)
            Debug.LogError($"TileLoader: Failed to load tile at {path}");
        return tile;
    }
}

