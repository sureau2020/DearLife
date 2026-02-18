

public static class WalkRules
{
    public static bool CanWalk(in CellData cell)
    {
        if (!cell.Has(CellFlags.HasFloor)) return false;
        if (!cell.Has(CellFlags.FloorWalkable)) return false;
        if (cell.Has(CellFlags.HasFurniture) &&
            cell.Has(CellFlags.FurnitureBlocked)) return false;
        return true;
    }
}
