using System;
using System.Collections.Generic;
using UnityEngine;

public enum CustomFurnishKind { Furniture, Decor, Background }

// Disk DTO: no Unity objects or computed Vector properties are serialized.
[Serializable]
public class CustomFurnishData
{
    public const int MaxImageSize = 500;
    public const int PixelsPerCell = 16;
    public int version = 1;
    public string id;
    public string displayName;
    public CustomFurnishKind kind;
    public int width, height;
    public int anchorX, anchorY;
    public List<CustomFurnishCell> blockedCells = new();

    public Vector2Int GridSize() => new(
        Mathf.CeilToInt((width - anchorX) / (float)PixelsPerCell),
        Mathf.CeilToInt((height - anchorY) / (float)PixelsPerCell));

    public string Validate()
    {
        if (version != 1) return "不支持的素材版本";
        if (!Enum.IsDefined(typeof(CustomFurnishKind), kind)) return "素材类型无效";
        if (width < 1 || height < 1 || width > MaxImageSize || height > MaxImageSize)
            return "图片宽和高都必须在 1～500 像素之间";
        if (anchorX < 0 || anchorX >= width || anchorY < 0 || anchorY >= height)
            return "锚点必须位于图片内";
        if (string.IsNullOrWhiteSpace(displayName) || displayName.Length > 40)
            return "请输入 1～40 字的素材名称";
        if (blockedCells == null) return "格子数据缺失";
        if (kind == CustomFurnishKind.Decor && blockedCells.Count != 0)
            return "装饰不能包含碰撞格子";
        Vector2Int size = GridSize();
        var unique = new HashSet<Vector2Int>();
        foreach (var cell in blockedCells)
        {
            if (cell == null || cell.x < 0 || cell.y < 0 || cell.x >= size.x || cell.y >= size.y)
                return "格子超出图片范围";
            if (!unique.Add(new Vector2Int(cell.x, cell.y))) return "格子重复";
        }
        if (kind == CustomFurnishKind.Background && unique.Count == size.x * size.y)
            return "房间背景至少需要一个白色可行走格";
        return null;
    }

    public List<Vector2Int> GetBlockedCells()
    {
        var result = new List<Vector2Int>();
        foreach (var cell in blockedCells) result.Add(new Vector2Int(cell.x, cell.y));
        return result;
    }
}

[Serializable]
public class CustomFurnishCell
{
    public int x, y;
    public CustomFurnishCell(int x, int y) { this.x = x; this.y = y; }
}
