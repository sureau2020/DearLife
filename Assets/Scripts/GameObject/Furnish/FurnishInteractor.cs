using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FurnishInteractor : MonoBehaviour
{
    [SerializeField] private GameObject layerChangeButtons;
    [SerializeField] private Image decorLayerSigh;
    [SerializeField] private Image furnitureLayerSigh;
    [SerializeField] private Image floorLayerSigh;
    private IRoomDataProvider roomData;
    private Plane xzPlane = new Plane(Vector3.up, Vector3.zero);
    public RoomManager roomManager => roomData.roomManager;

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
                FurnitureInstance furniture = roomManager.GetFurnitureAt(hitPoint);
                if (furniture != null) { 
                    
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
        roomManager.ShowFurnitureCells();

    }


    public void ShowDecorLayer()
    {
        SoundManager.Instance.PlaySfx("Click");
        decorLayerSigh.enabled = true;
        furnitureLayerSigh.enabled = false;
        floorLayerSigh.enabled = false;
        roomManager.ShowDecorCells();
    }

    public void ShowFloorLayer()
    {
        SoundManager.Instance.PlaySfx("Click");
        decorLayerSigh.enabled = false;
        furnitureLayerSigh.enabled = false;
        floorLayerSigh.enabled = true;
        roomManager.ShowFloorCells();
    }



}
