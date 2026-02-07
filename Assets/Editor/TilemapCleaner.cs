using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class TilemapCleaner : EditorWindow
{
    private Tilemap targetTilemap;
    private int yLimit = 100; // 超过正负 1000 的全部删掉

    [MenuItem("Tools/Map Editor/Tilemap Ghost Buster")]
    public static void ShowWindow() => GetWindow<TilemapCleaner>("Ghost Buster");

    private void OnGUI()
    {
        GUILayout.Label("Tilemap 幽灵瓦片清理器", EditorStyles.boldLabel);
        targetTilemap = (Tilemap)EditorGUILayout.ObjectField("目标 Tilemap", targetTilemap, typeof(Tilemap), true);
        yLimit = EditorGUILayout.IntField("有效 Y 轴范围 (±)", yLimit);

        if (GUILayout.Button("一键清理并重置边界", GUILayout.Height(40)))
        {
            Clean();
        }
    }

    private void Clean()
    {
        if (targetTilemap == null) return;

        int removedCount = 0;
        // 注意：因为边界太夸张，我们不能遍历 cellBounds，否则会卡死
        // 我们改用底层方法获取所有已放置的瓦片位置

        // 获取当前所有有瓦片的位置
        BoundsInt bounds = targetTilemap.cellBounds;

        // 我们只检查 Y 轴极端的区域
        // 如果你的地图是横向的，你可以增加 X 轴的清理
        foreach (var pos in targetTilemap.cellBounds.allPositionsWithin)
        {
            if (targetTilemap.HasTile(pos))
            {
                if (Mathf.Abs(pos.y) > yLimit || Mathf.Abs(pos.x) > yLimit)
                {
                    targetTilemap.SetTile(pos, null);
                    removedCount++;
                }
            }
        }

        targetTilemap.CompressBounds();
        EditorUtility.SetDirty(targetTilemap);

        Debug.Log($"清理完成！删除了 {removedCount} 个超远距离的垃圾瓦片。");
        Debug.Log($"新边界大小: {targetTilemap.cellBounds.size.x} x {targetTilemap.cellBounds.size.y}");
        EditorUtility.DisplayDialog("清理结果", $"删除了 {removedCount} 个幽灵瓦片。\n新边界: {targetTilemap.cellBounds.size.x} x {targetTilemap.cellBounds.size.y}", "太棒了");
    }
}