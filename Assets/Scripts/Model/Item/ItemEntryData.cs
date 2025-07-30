
public class ItemEntryData
{
    public string ItemID { get; private set; }

    public int Count { get; set; }

    public ItemEntryData(string itemId, int count)
    {
        ItemID = itemId;
        Count = count;
    }
}
