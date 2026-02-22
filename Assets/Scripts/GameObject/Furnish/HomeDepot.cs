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

    private List<DecorData> decors = new List<DecorData>();
    private List<FurnitureData> furnitures = new List<FurnitureData>();
    private List<TileData> floorTiles = new List<TileData>();
    private Dictionary<string, Sprite> tileCache = new();


    public void ShowFurnitureDepot()
    {
        ChangeType("家具");
        currentCategory = FurnishCategory.Furniture;
        furnitures = GameManager.Instance.FurnitureDatabase.GetAllFurniture(furnitures);
        ClearList();
        foreach (var furniture in furnitures)
        {
            GameObject cell = Instantiate(cellPrefab, cellList);
            cell.GetComponent<FurnitureCell>().SetFurnitureData(furniture.sprite,furniture.id, FurnishCategory.Furniture);
        }
    }

    private void ClearList() { 
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
        foreach (var decor in decors)
        {
            GameObject cell = Instantiate(cellPrefab, cellList);
            cell.GetComponent<FurnitureCell>().SetFurnitureData(decor.sprite,decor.id, FurnishCategory.Decor);
        }
    }

    public void ShowFloorDepot() { 
        ChangeType("地板");
        currentCategory = FurnishCategory.Floor;
        ClearList();
        GameManager.Instance.TileDataBase.GetAllFloorTiles(floorTiles).ForEach(tile => {
            GameObject cell = Instantiate(cellPrefab, cellList);
            cell.GetComponent<FurnitureCell>().SetFurnitureData(GetTileBase(tile.id),tile.id, FurnishCategory.Floor);
        });
    }

    private Sprite GetTileBase(string tileId)
    {
        if (tileCache.TryGetValue(tileId, out var tileSprite))
            return tileSprite;
        Tile tile = tileLoader.LoadTile(tileId) as Tile;
        if (tile != null)
        {
            tileCache[tileId] = tileSprite;
            return tile.sprite;
        }
        return null;
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