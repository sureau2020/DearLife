using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapToPNG
{
    [MenuItem("Tools/Export Tilemap To PNG")]
    static void Export()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogError("Select a Tilemap");
            return;
        }

        Tilemap tilemap = Selection.activeGameObject.GetComponent<Tilemap>();

        if (tilemap == null)
        {
            Debug.LogError("Selected object is not a Tilemap");
            return;
        }

        // 手动输入范围
        int xMin = -1;
        int xMax = 8;
        int yMin = -1;
        int yMax = 10;

        int tileWidth = 16;  // tile 宽度
        int tileHeight = 16; // tile 高度

        int width = (xMax - xMin + 1) * tileWidth;
        int height = (yMax - yMin + 1) * tileHeight;

        Texture2D output = new Texture2D(width, height, TextureFormat.RGBA32, false);

        // 清空为透明底
        Color[] clear = new Color[width * height];
        for (int i = 0; i < clear.Length; i++)
            clear[i] = new Color(0, 0, 0, 0);
        output.SetPixels(clear);

        for (int x = xMin; x <= xMax; x++)
        {
            for (int y = yMin; y <= yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(pos);
                if (tile == null) continue;

                Sprite sprite = tilemap.GetSprite(pos);
                if (sprite == null) continue;

                Texture2D tex = sprite.texture;
                Rect r = sprite.textureRect;

                Color[] pixels = tex.GetPixels(
                    (int)r.x,
                    (int)r.y,
                    (int)r.width,
                    (int)r.height
                );

                int px = (x - xMin) * tileWidth;
                int py = (y - yMin) * tileHeight;

                output.SetPixels(px, py, tileWidth, tileHeight, pixels);
            }
        }

        output.Apply();

        byte[] png = output.EncodeToPNG();
        string path = Application.dataPath + "/tilemap_export.png";
        File.WriteAllBytes(path, png);

        Debug.Log("Exported: " + path);

        AssetDatabase.Refresh();
    }
}