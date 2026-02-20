using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FurnishInteractor : MonoBehaviour
{
    [SerializeField] private GameObject layerChangeButtons;
    [SerializeField] private GameObject kuang;
    [SerializeField] private Image decorLayerSigh;
    [SerializeField] private Image furnitureLayerSigh;
    [SerializeField] private Image floorLayerSigh;

    private FurnitureInstance currentFurnitureInstance;
    
    
    private IRoomDataProvider roomData;
    private Plane xzPlane = new Plane(Vector3.up, Vector3.zero);

    private void Update()
    {
        //#if UNITY_ANDROID || UNITY_IOS
        //            CheckTouch();
        //        
        //#else
           CheckClick();
        //        CheckTouch();
        //#endif
    }

    public void CheckClick() {
        if (Application.isMobilePlatform || Input.touchSupported)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float distance; // 射线到平面的距离
            if (xzPlane.Raycast(ray, out distance))
            {
                Vector3 hitPoint = ray.GetPoint(distance);
                if (roomData == null) return;
                FurnitureInstance furniture = roomData.roomManager.GetFurnitureAt(hitPoint);
                if (furniture != null) {
                    Vector3 spriteSize = furniture.furnitureObject.GetComponent<SpriteRenderer>().bounds.size;
                    Vector3 realSize = new Vector3(spriteSize.x, spriteSize.z, 1);
                    kuang.SetActive(true);
                    kuang.transform.GetComponent<SpriteRenderer>().size = realSize;
                    kuang.transform.position = roomData.roomManager.GetCellWorldLeftBottomPosition(furniture.anchorPos);
                    currentFurnitureInstance = furniture;
                }
            }
        }
    }

    public void Initialize(IRoomDataProvider provider) { 
        roomData = provider;
        layerChangeButtons.SetActive(true);
        ShowFurnitureLayer();
    }

    public void ShowFurnitureLayer() { 
        SoundManager.Instance.PlaySfx("Click");
        decorLayerSigh.enabled = false;
        furnitureLayerSigh.enabled = true;
        floorLayerSigh.enabled = false;
        roomData.roomManager.ShowFurnitureCells();

    }


    public void ShowDecorLayer()
    {
        SoundManager.Instance.PlaySfx("Click");
        decorLayerSigh.enabled = true;
        furnitureLayerSigh.enabled = false;
        floorLayerSigh.enabled = false;
        roomData.roomManager.ShowDecorCells();
    }

    public void ShowFloorLayer()
    {
        SoundManager.Instance.PlaySfx("Click");
        decorLayerSigh.enabled = false;
        furnitureLayerSigh.enabled = false;
        floorLayerSigh.enabled = true;
        roomData.roomManager.ShowFloorCells();
    }



}
