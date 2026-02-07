using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Collections.Generic;

public class MapLayoutExporter : EditorWindow
{
    private Tilemap sourceTilemap;
    private MapDataSO targetMapData;

    [MenuItem("Tools/Map Editor/Map Layout Exporter")]
    public static void ShowWindow() => GetWindow<MapLayoutExporter>("Layout Exporter");

    private void OnGUI()
    {
        GUILayout.Label("地图布局导出器 (仅导出蓝图)", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("该工具仅记录坐标和 Tile ID，不会修改你的 TileConfig 或 TileLibrary。", MessageType.Info);

        EditorGUILayout.Space();
        sourceTilemap = (Tilemap)EditorGUILayout.ObjectField("源 Tilemap (场景中)", sourceTilemap, typeof(Tilemap), true);
        targetMapData = (MapDataSO)EditorGUILayout.ObjectField("目标 MapDataSO (资源)", targetMapData, typeof(MapDataSO), false);

        EditorGUILayout.Space();

        if (GUILayout.Button("导出当前布局", GUILayout.Height(40)))
        {
            if (sourceTilemap == null || targetMapData == null)
            {
                EditorUtility.DisplayDialog("提示", "请先拖入 Tilemap 和 MapDataSO！", "好的");
                return;
            }
            ExportLayout();
        }
    }

    private void ExportLayout()

    {
        sourceTilemap.CompressBounds();
        // 开启撤销功能，万一导错了可以 Ctrl+Z
        Undo.RecordObject(targetMapData, "Export Tilemap Layout");

        targetMapData.tileInstances.Clear();

        // 获取 Tilemap 的实际有效范围
        BoundsInt bounds = sourceTilemap.cellBounds;

        Debug.Log($"压缩后的边界大小: {bounds.size.x} x {bounds.size.y}");

        foreach (var pos in bounds.allPositionsWithin)
        {
            TileBase tile = sourceTilemap.GetTile(pos);
            if (tile != null & pos.x>= 0)
            {
                targetMapData.tileInstances.Add(new TileInstanceData
                {
                    position = pos,
                    tileId = tile.name
                });
            }
        }

        EditorUtility.SetDirty(targetMapData);
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("导出完成", $"已成功记录 {targetMapData.tileInstances.Count} 个格子的位置数据。", "太棒了");
    }
}