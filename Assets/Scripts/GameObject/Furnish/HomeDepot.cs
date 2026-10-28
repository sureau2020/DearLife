using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class HomeDepot : MonoBehaviour
{
    //[SerializeField] private RectTransform homeDepot;
    [SerializeField] private TextMeshProUGUI type;
    [SerializeField] private RectTransform cellList;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject customCellPrefab;
    [SerializeField] private TileLoader tileLoader;
    private FurnishCategory currentCategory;

    private List<FurnitureCell> furnitureList = new List<FurnitureCell>();
    private List<DecorData> decors = new List<DecorData>();
    private List<FurnitureData> furnitures = new List<FurnitureData>();
    private List<MapData> maps = new List<MapData>();

    int currentIndex = -1;
    private CustomFurnishEditor importEditor;
    public bool IsImportOpen => importEditor != null && importEditor.gameObject.activeSelf;
    public event System.Action ImportOpened;

    void OnEnable() => FurnishItemEvents.OnItemClicked += HandleItemClick;
    void OnDisable()
    {
        FurnishItemEvents.OnItemClicked -= HandleItemClick;
        if (importEditor != null) importEditor.Close();
    }


    public void ShowFurnitureDepot()
    {
        ChangeType("家具");
        currentCategory = FurnishCategory.Furniture;
        furnitures = GameManager.Instance.FurnitureDatabase.GetAllFurniture(furnitures);
        ClearList();
        int index = 0;
        currentIndex = -1;
        foreach (var furniture in furnitures)
        {
            GameObject cell = Instantiate(cellPrefab, cellList);
            FurnitureCell furnitureCell = cell.GetComponent<FurnitureCell>();
            furnitureCell.SetFurnitureData(furniture.sprite,furniture.id, FurnishCategory.Furniture,index);
            furnitureList.Add(furnitureCell);
            index++;
        }
        AddImportCell();
    }

    private void ClearList() {
        furnitureList.Clear();
        foreach (Transform child in cellList)
        {
            Destroy(child.gameObject);
        }
    }

    public void ShowDecorDepot() { 
        ChangeType("装饰");
        currentCategory = FurnishCategory.Decor;
        decors = GameManager.Instance.FurnitureDatabase.GetAllDecors(decors);
        ClearList();
        int index = 0;
        currentIndex = -1;
        foreach (var decor in decors)
        {
            GameObject cell = Instantiate(cellPrefab, cellList);
            FurnitureCell furnitureCell = cell.GetComponent<FurnitureCell>();
            furnitureCell.SetFurnitureData(decor.sprite,decor.id, FurnishCategory.Decor, index);
            furnitureList.Add(furnitureCell);
            index++;
        }
        AddImportCell();
    }

    private void AddImportCell()
    {
        GameObject cell = Instantiate(customCellPrefab, cellList);
        cell.GetComponent<FurnitureCell>().SetImportAction(OpenImport);
    }

    private void OpenImport()
    {
        if (IsImportOpen) return;
        var prefab = Resources.Load<CustomFurnishEditor>("UI/CustomFurnishEditor");
        if (prefab == null)
        {
            ErrorNotifier.NotifyError("CustomFurnishEditor prefab is missing");
            return;
        }
        var canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;
        ImportOpened?.Invoke();
        var category = currentCategory;
        var kind = category == FurnishCategory.Furniture ? CustomFurnishKind.Furniture
            : category == FurnishCategory.Decor ? CustomFurnishKind.Decor : CustomFurnishKind.Background;
        importEditor = Instantiate(prefab, canvas.rootCanvas.transform);
        importEditor.transform.SetAsLastSibling();
        importEditor.Initialize(kind, GameManager.Instance.FurnitureDatabase.CustomItems,
            () => {
                if (this == null || !isActiveAndEnabled) return;
                if (category == FurnishCategory.Furniture) ShowFurnitureDepot();
                else if (category == FurnishCategory.Decor) ShowDecorDepot();
                else ShowFloorDepot();
            }, () => { importEditor = null; });
    }

    public void ShowFloorDepot() { 
        ChangeType("地板");
        currentCategory = FurnishCategory.Floor;
        ClearList();
        int index = 0;
        currentIndex = -1;
        GameManager.Instance.MapDataBase.GetAllMap(maps).ForEach(map => {
            GameObject cell = Instantiate(cellPrefab, cellList);
            FurnitureCell furnitureCell = cell.GetComponent<FurnitureCell>();
            if (map.icon == null)
            {
                furnitureCell.SetFurnitureData(GetMapIconAndSetIcon(map), map.id, FurnishCategory.Floor, index);
            }
            else { 
                furnitureCell.SetFurnitureData(map.icon, map.id, FurnishCategory.Floor, index);
            }
            furnitureList.Add(furnitureCell);
            index++;
        });
        foreach (var entry in GameManager.Instance.FurnitureDatabase.CustomItems.Entries)
        {
            if (entry.Data.kind != CustomFurnishKind.Background) continue;
            var cell = Instantiate(cellPrefab, cellList).GetComponent<FurnitureCell>();
            cell.SetFurnitureData(entry.Sprite, entry.Data.id, FurnishCategory.Floor, index++);
            furnitureList.Add(cell);
        }
        AddImportCell();
    }

    private Sprite GetMapIconAndSetIcon(MapData map)
    {
        Sprite icon = IconManager.GetMapIcon(map.id);
        if (icon != null)
        {
            map.icon = icon;
            return icon;
        }
        return null;
    }

    public void HandleItemClick(string id, FurnishCategory category, int index) {
        if (category != currentCategory || index < 0 || index >= furnitureList.Count) return;
        FurnitureCell furnitureCell = furnitureList[index];
        if (furnitureCell == null) return;
        furnitureCell.SetSelected(true);
        if (currentIndex != -1 && currentIndex != index)
            furnitureList[currentIndex]?.SetSelected(false);
        currentIndex = index;
    }

    private void ChangeType(string t) { 
        type.text = t;
    }
}


public enum FurnishCategory
{
    Furniture,
    Decor,
    Floor
}