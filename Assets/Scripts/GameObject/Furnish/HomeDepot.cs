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
    [SerializeField] private TileLoader tileLoader;
    private FurnishCategory currentCategory;

    private List<FurnitureCell> furnitureList = new List<FurnitureCell>();
    private List<DecorData> decors = new List<DecorData>();
    private List<FurnitureData> furnitures = new List<FurnitureData>();
    private List<TileData> floorTiles = new List<TileData>();
    private Dictionary<string, Sprite> tileCache = new();

    int currentIndex = -1;

    void OnEnable() => FurnishItemEvents.OnItemClicked += HandleItemClick;
    void OnDisable() => FurnishItemEvents.OnItemClicked -= HandleItemClick;


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
    }

    public void ShowFloorDepot() { 
        ChangeType("地板");
        currentCategory = FurnishCategory.Floor;
        ClearList();
        int index = 0;
        currentIndex = -1;
        GameManager.Instance.TileDataBase.GetAllFloorTiles(floorTiles).ForEach(tile => {
            GameObject cell = Instantiate(cellPrefab, cellList);
            FurnitureCell furnitureCell = cell.GetComponent<FurnitureCell>();
            furnitureCell.SetFurnitureData(GetTileBase(tile.id),tile.id, FurnishCategory.Floor, index);
            furnitureList.Add(furnitureCell);
            index++;
        });
    }

    private Sprite GetTileBase(string tileId)
    {
        if (tileCache.TryGetValue(tileId, out var tileSprite))
            return tileSprite;
        Tile tile = tileLoader.LoadTile(tileId) as Tile;
        if (tile != null)
        {
            tileCache[tileId] = tile.sprite;
            return tile.sprite;
        }
        return null;
    }

    public void HandleItemClick(string id, FurnishCategory category, int index) {
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