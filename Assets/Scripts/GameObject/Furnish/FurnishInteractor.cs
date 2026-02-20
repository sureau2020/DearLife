using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class FurnishInteractor : MonoBehaviour
{
    [SerializeField] private GameObject cancelBinOkButton;
    [SerializeField] private GameObject layerChangeButtons;
    [SerializeField] private GameObject kuang;
    [SerializeField] private Image decorLayerSigh;
    [SerializeField] private Image furnitureLayerSigh;
    [SerializeField] private Image floorLayerSigh;

    private FurnitureInstance currentFurnitureInstance;
    private DecorInstance currentDecorInstance;
    private enum Layer { Furniture, Decor, Floor }
    private Layer currentLayer = Layer.Furniture;
    private bool isInstanceSelected = false;


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
                if (isInstanceSelected)
                {
                    PreviewInstanceMove(hitPoint);
                }
                else {
                    SelectInstance(hitPoint);
                }
            }
        }
    }

    private void PreviewInstanceMove(Vector3 hitPoint) {
        switch (currentLayer)
        {
            case Layer.Furniture:
                CheckFurnitureMove(hitPoint);
                break;
            case Layer.Decor:
                break;
            case Layer.Floor:
                //CheckFloorClick(hitPoint);
                break;
        }
    }

    private void SelectInstance(Vector3 hitPoint)
    {
        switch (currentLayer)
        {
            case Layer.Furniture:
                CheckFurnitureClick(hitPoint);
                break;
            case Layer.Decor:
                CheckDecorClick(hitPoint);
                break;
            case Layer.Floor:
                //CheckFloorClick(hitPoint);
                break;
        }
    }

    private void CheckFurnitureMove(Vector3 hitPoint) {
        if (currentFurnitureInstance == null) return;
        Vector3 pos = currentFurnitureInstance.furnitureObject.transform.position;
        roomData.roomManager.PreviewMoveFurniture(currentFurnitureInstance, hitPoint,pos);
        kuang.transform.position = roomData.roomManager.GetCellWorldLeftBottomPosition(hitPoint);
    }

    private void CheckFurnitureClick(Vector3 hitPoint) {
        
        FurnitureInstance furniture = roomData.roomManager.GetFurnitureAt(hitPoint);
        if (furniture != null)
        {
            Vector3 spriteSize = furniture.furnitureObject.GetComponent<SpriteRenderer>().bounds.size;
            Vector3 realSize = new Vector3(spriteSize.x, spriteSize.z, 1);
            kuang.SetActive(true);
            kuang.transform.GetComponent<SpriteRenderer>().size = realSize;
            kuang.transform.position = roomData.roomManager.GetCellWorldLeftBottomPosition(furniture.anchorPos);
            currentFurnitureInstance = furniture;
            ShowCancelBinOKButtons();
            isInstanceSelected = true;
        }
    }

    private void CheckDecorClick(Vector3 hitPoint)
    {
        DecorInstance decor = roomData.roomManager.GetDecorInstanceAt(hitPoint);
        if (decor != null) {
            Vector3 spriteSize = decor.decorObject.GetComponent<SpriteRenderer>().bounds.size;
            Vector3 realSize = new Vector3(spriteSize.x, spriteSize.z, 1);
            if(!kuang.activeSelf) kuang.SetActive(true);
            kuang.transform.GetComponent<SpriteRenderer>().size = realSize;
            kuang.transform.position = roomData.roomManager.GetCellWorldLeftBottomPosition(decor.position);
            currentDecorInstance = decor;
            ShowCancelBinOKButtons();
            isInstanceSelected = true;
        }
    }

    public void CancelSelection() {
        HideCancelBinOkButtons();
        switch (currentLayer) { 
            case Layer.Furniture:
                RefreshFrnitureLayer();
                currentFurnitureInstance.furnitureObject.transform.position = roomData.roomManager.GetCellWorldLeftBottomPosition(currentFurnitureInstance.anchorPos);
                currentFurnitureInstance = null;
                break;
            case Layer.Decor:
                RefreshDecorLayer();
                currentFurnitureInstance.furnitureObject.transform.position = roomData.roomManager.GetCellWorldLeftBottomPosition(currentFurnitureInstance.anchorPos);
                currentDecorInstance = null;
                break;
            case Layer.Floor:
                //roomData.roomManager.ShowFloorCells();
                break;
        }
    }

    private void ShowCancelBinOKButtons() {
        SoundManager.Instance.PlaySfx("Pop");
        layerChangeButtons.SetActive(false);
        cancelBinOkButton.SetActive(true);
    }

    public void DeleteInstance() {
        SoundManager.Instance.PlaySfx("Click");
        if (currentLayer == Layer.Furniture && currentFurnitureInstance != null) {
            roomData.roomManager.RemoveFurniture(currentFurnitureInstance);
            currentFurnitureInstance = null;
            HideCancelBinOkButtons();
            RefreshFrnitureLayer();

        }
        else if (currentLayer == Layer.Decor && currentDecorInstance != null) {
            roomData.roomManager.RemoveDecor(currentDecorInstance);
            currentDecorInstance = null;
            HideCancelBinOkButtons();
            RefreshDecorLayer();
        }
    }

    public void HideCancelBinOkButtons() {
        SoundManager.Instance.PlaySfx("Click");
        if (kuang.activeSelf) kuang.SetActive(false);
        isInstanceSelected = false;
        layerChangeButtons.SetActive(true);
        cancelBinOkButton.SetActive(false);
    }

    public void Initialize(IRoomDataProvider provider) { 
        roomData = provider;
        layerChangeButtons.SetActive(true);
        ShowFurnitureLayer();
    }

    public void ShowFurnitureLayer() { 
        SoundManager.Instance.PlaySfx("Click");
        if (kuang.activeSelf) kuang.SetActive(false);
        currentLayer = Layer.Furniture;
        decorLayerSigh.enabled = false;
        furnitureLayerSigh.enabled = true;
        floorLayerSigh.enabled = false;
        RefreshFrnitureLayer();
    }

    private void RefreshFrnitureLayer() {
        roomData.roomManager.ShowFurnitureCells();
    }

    private void RefreshDecorLayer() {
        roomData.roomManager.ShowDecorCells();
    }


    public void ShowDecorLayer()
    {
        SoundManager.Instance.PlaySfx("Click");
        if (kuang.activeSelf) kuang.SetActive(false);
        currentLayer = Layer.Decor;
        decorLayerSigh.enabled = true;
        furnitureLayerSigh.enabled = false;
        floorLayerSigh.enabled = false;
        RefreshDecorLayer();
    }

    public void ShowFloorLayer()
    {
        SoundManager.Instance.PlaySfx("Click");
        if (kuang.activeSelf) kuang.SetActive(false);
        currentLayer = Layer.Floor;
        decorLayerSigh.enabled = false;
        furnitureLayerSigh.enabled = false;
        floorLayerSigh.enabled = true;
        roomData.roomManager.ShowFloorCells();
    }

    
}
