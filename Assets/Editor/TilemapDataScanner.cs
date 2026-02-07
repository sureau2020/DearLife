using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class TilemapDataScanner : EditorWindow
{
    private Tilemap targetTilemap;
    private TileConfigSO configSO;
    private TileBaseLibrarySO librarySO;

    [MenuItem("Tools/Map Editor/Built-in Tile Scanner")]
    public static void ShowWindow()
    {
        GetWindow<TilemapDataScanner>("Tile Scanner");
    }

    private void OnGUI()
    {
        GUILayout.Label("数据扫描配置 (增量更新)", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("扫描将提取 Tile 名字作为 ID。如果 ID 已存在，则不会覆盖原有属性。", MessageType.Info);

        EditorGUILayout.Space();
        targetTilemap = (Tilemap)EditorGUILayout.ObjectField("样本 Tilemap", targetTilemap, typeof(Tilemap), true);
        configSO = (TileConfigSO)EditorGUILayout.ObjectField("逻辑配置 SO (TileConfig)", configSO, typeof(TileConfigSO), false);
        librarySO = (TileBaseLibrarySO)EditorGUILayout.ObjectField("资源库 SO (TileLibrary)", librarySO, typeof(TileBaseLibrarySO), false);

        EditorGUILayout.Space();

        if (GUILayout.Button("开始扫描并同步", GUILayout.Height(40)))
        {
            ExecuteScan();
        }
    }

    private void ExecuteScan()
    {
        if (targetTilemap == null || configSO == null || librarySO == null)
        {
            EditorUtility.DisplayDialog("提示", "请确保所有字段都已填入。存档资产不能为空。", "好的");
            return;
        }

        // 1. 获取所有不重复的 TileBase
        BoundsInt bounds = targetTilemap.cellBounds;
        TileBase[] allTiles = targetTilemap.GetTilesBlock(bounds);
        var uniqueTiles = allTiles.Where(t => t != null).Distinct().ToList();

        int newLogicAdded = 0;
        int newLibraryAdded = 0;

        foreach (var tile in uniqueTiles)
        {
            string tid = tile.name;

            // 2. 同步资源映射 (Library)
            if (!librarySO.tileResources.Any(x => x.tileId == tid))
            {
                librarySO.tileResources.Add(new TileEntry
                {
                    tileId = tid,
                    tile = tile
                });
                newLibraryAdded++;
            }

            // 3. 同步逻辑配置 (Config)
            // 这里使用的是你外部定义的 TileLogicData
            if (!configSO.tileProperties.Any(x => x.id == tid))
            {
                configSO.tileProperties.Add(new TileData
                {
                    id = tid,
                    walkable = true // 默认设为可通行
                });
                newLogicAdded++;
            }
        }

        // 4. 保存更改
        EditorUtility.SetDirty(configSO);
        EditorUtility.SetDirty(librarySO);
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("扫描结果",
            $"扫描完成！\n发现唯一 Tile: {uniqueTiles.Count}个\n新增逻辑项: {newLogicAdded}个\n新增资源项: {newLibraryAdded}个",
            "确认");
    }
}