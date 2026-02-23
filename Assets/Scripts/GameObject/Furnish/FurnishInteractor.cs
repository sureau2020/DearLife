using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class FurnishInteractor : MonoBehaviour
{
    [SerializeField] private GameObject cancelBinOkButton;
    [SerializeField] private RectTransform TopStateUI;
    [SerializeField] private RectTransform HomeDepotUI;
    [SerializeField] private Image HomeDepotCover;
    public int offsetYForTopStateUI = 520;
    public int offsetYForHomeDepotUI = -720;
    [SerializeField] private GameObject layerChangeButtons;
    [SerializeField] private Button furnishButton;
    [SerializeField] private Button closetButton;
    [SerializeField] private GameObject kuang;
    [SerializeField] private Image decorLayerSigh;
    [SerializeField] private Image furnitureLayerSigh;
    [SerializeField] private Image floorLayerSigh;
    [SerializeField] private TextMeshProUGUI hint;
    [SerializeField] private HomeDepot homeDepot;

    private FurnitureInstance currentFurnitureInstance;
    private DecorInstance currentDecorInstance;

    private FurnishCategory currentLayer = FurnishCategory.Furniture;
    private bool isInstanceSelected = false;
    private bool isInEditMode = false;
    private bool isCreatingNewInstance = false;
    private string creatingInstanceDataId = "";


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

    public void CheckClick()
    {
        if (!isInEditMode || Application.isMobilePlatform || Input.touchSupported)
            return;

        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float distance; // 射线到平面的距离
            if (xzPlane.Raycast(ray, out distance))
            {
                Vector3 hitPoint = ray.GetPoint(distance);
                if (roomData == null) return;
                if (isCreatingNewInstance) {
                    PreviewNewInstance(hitPoint);
                }
                else if (isInstanceSelected)
                {
                    PreviewInstanceMove(hitPoint);
                }
                else
                {
                    SelectInstance(hitPoint);
                }
            }
        }
    }

    private void PreviewInstanceMove(Vector3 hitPoint)
    {
        switch (currentLayer)
        {
            case FurnishCategory.Furniture:
                CheckFurnitureMove(hitPoint);
                break;
            case FurnishCategory.Decor:
                CheckDecorMove(hitPoint);
                break;
            case FurnishCategory.Floor:
                //CheckFloorClick(hitPoint);
                break;
        }
    }

    private void SelectInstance(Vector3 hitPoint)
    {
        switch (currentLayer)
        {
            case FurnishCategory.Furniture:
                CheckFurnitureClick(hitPoint);
                break;
            case FurnishCategory.Decor:
                CheckDecorClick(hitPoint);
                break;
            case FurnishCategory.Floor:
                //CheckFloorClick(hitPoint);
                break;
        }
    }

    private void CheckFurnitureMove(Vector3 hitPoint)
    {
        if (currentFurnitureInstance == null) return;
        Vector3 pos = currentFurnitureInstance.furnitureObject.transform.position;
        roomData.roomManager.PreviewMoveFurniture(currentFurnitureInstance, hitPoint, pos);
        kuang.transform.position = roomData.roomManager.GetCellWorldLeftBottomPosition(hitPoint);
    }

    private void CheckDecorMove(Vector3 hitPoint)
    {
        if (currentDecorInstance == null) return;
        Vector3 pos = currentDecorInstance.decorObject.transform.position;
        roomData.roomManager.PreviewMoveDecor(currentDecorInstance, hitPoint, pos);
        kuang.transform.position = roomData.roomManager.GetCellWorldLeftBottomPosition(hitPoint);
    }

    private void CheckFurnitureClick(Vector3 hitPoint)
    {

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
            HomeDepotCover.enabled = true;
        }
    }

    private void CheckDecorClick(Vector3 hitPoint)
    {
        DecorInstance decor = roomData.roomManager.GetDecorInstanceAt(hitPoint);
        if (decor != null)
        {
            Vector3 spriteSize = decor.decorObject.GetComponent<SpriteRenderer>().bounds.size;
            Vector3 realSize = new Vector3(spriteSize.x, spriteSize.z, 1);
            if (!kuang.activeSelf) kuang.SetActive(true);
            kuang.transform.GetComponent<SpriteRenderer>().size = realSize;
            kuang.transform.position = roomData.roomManager.GetCellWorldLeftBottomPosition(decor.position);
            currentDecorInstance = decor;
            ShowCancelBinOKButtons();
            isInstanceSelected = true;
            HomeDepotCover.enabled = true;
        }
    }

    public void CancelSelection()
    {
        
        if (isCreatingNewInstance) { DeleteNewInstance(); return; }
        HideCancelBinOkButtons();
        switch (currentLayer)
        {
            case FurnishCategory.Furniture:
                RefreshFrnitureLayer();
                currentFurnitureInstance.furnitureObject.transform.position = roomData.roomManager.GetCellWorldLeftBottomPosition(currentFurnitureInstance.anchorPos);
                currentFurnitureInstance = null;
                ShowHint("点击绿色格子选择相应部件");
                break;
            case FurnishCategory.Decor:
                RefreshDecorLayer();
                currentDecorInstance.decorObject.transform.position = roomData.roomManager.GetCellWorldLeftBottomPosition(currentDecorInstance.position);
                currentDecorInstance = null;
                ShowHint("点击绿色格子选择相应部件");
                break;
            case FurnishCategory.Floor:
                //roomData.roomManager.ShowFloorCells();
                break;
        }
    }

    public void DeleteNewInstance()
    {
        HideCancelBinOkButtons();
        if (currentLayer == FurnishCategory.Furniture && currentFurnitureInstance != null)
        {
            roomData.roomManager.RemoveFurniture(currentFurnitureInstance);
            RefreshFrnitureLayer();
            currentFurnitureInstance = null;
        }
        else if (currentLayer == FurnishCategory.Decor && currentDecorInstance != null)
        {
            roomData.roomManager.RemoveDecor(currentDecorInstance);
            RefreshDecorLayer();
            currentDecorInstance = null;
        }
        isCreatingNewInstance = false;
        ShowHint("点击绿色格子选择相应部件");
    }

    public void ConfirmMove()
    {
        switch (currentLayer)
        {
            case FurnishCategory.Furniture:
                if (currentFurnitureInstance != null)
                {
                    Debug.Log("尝试放置家具："+currentFurnitureInstance.instanceId+" at "+currentFurnitureInstance.anchorPos);
                    if (roomData.roomManager.ConfirmMoveFurniture(currentFurnitureInstance))
                    {
                        if (isCreatingNewInstance) ComfirmNewInstance();
                        HideCancelBinOkButtons();
                        RefreshFrnitureLayer();
                        ShowHint("点击绿色格子选择相应部件");
                        currentFurnitureInstance = null;
                    }
                    else
                    {
                        SoundManager.Instance.PlaySfx("Error");
                        Vibrate(currentFurnitureInstance.furnitureObject);
                        ShowHint("当前位置不可放置此家具");
                    }
                }
                break;
            case FurnishCategory.Decor:
                if (currentDecorInstance != null) {
                    if (roomData.roomManager.ConfirmMoveDecor(currentDecorInstance))
                    {
                        if (isCreatingNewInstance) ComfirmNewInstance();
                        HideCancelBinOkButtons();
                        RefreshDecorLayer();
                        ShowHint("点击绿色格子选择相应部件");
                        currentDecorInstance = null;
                    }
                    else
                    {
                        SoundManager.Instance.PlaySfx("Error");
                        Vibrate(currentDecorInstance.decorObject);
                        ShowHint("当前位置不可放置此装饰");
                    }
                }
                break;
            case FurnishCategory.Floor:
                //roomData.roomManager.ShowFloorCells();
                break;
        }
        
    }

    public void ComfirmNewInstance() {
        isCreatingNewInstance = false;
        if (currentLayer == FurnishCategory.Furniture)
        {
            
        }
        else if (currentLayer == FurnishCategory.Decor)
        {
            //roomData.roomManager.ConfirmNewDecor(currentDecorInstance);
        }
        currentFurnitureInstance = null;
        currentDecorInstance = null;
        ShowHint("点击绿色格子选择相应部件");
    }



    private void ShowCancelBinOKButtons()
    {
        SoundManager.Instance.PlaySfx("Pop");
        ShowHint("点击其他格子移动该部件");
        layerChangeButtons.SetActive(false);
        furnishButton.interactable = false;
        cancelBinOkButton.SetActive(true);
    }

    public void DeleteInstance()
    {
        SoundManager.Instance.PlaySfx("Click");
        if (isCreatingNewInstance) { DeleteNewInstance(); return; }
        if (currentLayer == FurnishCategory.Furniture && currentFurnitureInstance != null)
        {
            roomData.roomManager.RemoveFurniture(currentFurnitureInstance);
            currentFurnitureInstance = null;
            HideCancelBinOkButtons();
            RefreshFrnitureLayer();
            ShowHint("点击绿色格子选择相应部件");

        }
        else if (currentLayer == FurnishCategory.Decor && currentDecorInstance != null)
        {
            roomData.roomManager.RemoveDecor(currentDecorInstance);
            currentDecorInstance = null;
            HideCancelBinOkButtons();
            RefreshDecorLayer();
            ShowHint("点击绿色格子选择相应部件");
        }
    }

    public void HideCancelBinOkButtons()
    {
        SoundManager.Instance.PlaySfx("Click");
        if (kuang.activeSelf) kuang.SetActive(false);
        isInstanceSelected = false;
        HomeDepotCover.enabled = false;
        furnishButton.interactable = true;
        layerChangeButtons.SetActive(true);
        cancelBinOkButton.SetActive(false);
    }

    public void Initialize(IRoomDataProvider provider)
    {
        roomData = provider;
        layerChangeButtons.SetActive(true);
        closetButton.interactable = false;
        isInEditMode = true;
        if(!hint.isActiveAndEnabled) hint.gameObject.SetActive(true);
        ShowHint("点击绿色格子选择相应部件");
        ShowFurnitureLayer();
        MoveUI(TopStateUI, offsetYForTopStateUI, 100);
        MoveUI(HomeDepotUI, 0,offsetYForHomeDepotUI);
    }

    public void MoveUI(RectTransform uiToBeMove, float destination, float from)
    {
        float targetAnchoredY = isInEditMode ? destination : from;
        uiToBeMove.DOAnchorPosY(targetAnchoredY, 0.5f)
            .SetEase(Ease.OutQuad) 
            .OnComplete(() =>
            {
                if (!Mathf.Approximately(uiToBeMove.anchoredPosition.y, targetAnchoredY))
                {
                    uiToBeMove.anchoredPosition = new Vector2(uiToBeMove.anchoredPosition.x, targetAnchoredY);
                }
            });
    }

    public void Close()
    {
        ClearHint();
        SoundManager.Instance.PlaySfx("Click");
        isInEditMode = false;
        layerChangeButtons.SetActive(false);
        roomData.roomManager.ClearAllCells();
        MoveUI(TopStateUI,offsetYForTopStateUI, 100);
        MoveUI(HomeDepotUI, 0,offsetYForHomeDepotUI);
        closetButton.interactable = true;
    }

    private void ClearHint()
    {
        hint.text = "";
        if (hint.isActiveAndEnabled) hint.gameObject.SetActive(false);
    }

    private void ShowHint(string t)
    {
        if (!hint.isActiveAndEnabled) hint.gameObject.SetActive(true);
        hint.text = "<wave>"+t;
    }

    public void ShowFurnitureLayer()
    {
        SoundManager.Instance.PlaySfx("Click");
        homeDepot.ShowFurnitureDepot();
        if (kuang.activeSelf) kuang.SetActive(false);
        currentLayer = FurnishCategory.Furniture;
        decorLayerSigh.enabled = false;
        furnitureLayerSigh.enabled = true;
        floorLayerSigh.enabled = false;
        RefreshFrnitureLayer();
    }

    private void RefreshFrnitureLayer()
    {
        roomData.roomManager.ShowFurnitureCells();
    }

    private void RefreshDecorLayer()
    {
        roomData.roomManager.ShowDecorCells();
    }


    public void ShowDecorLayer()
    {
        SoundManager.Instance.PlaySfx("Click");
        homeDepot.ShowDecorDepot();
        if (kuang.activeSelf) kuang.SetActive(false);
        currentLayer = FurnishCategory.Decor;
        decorLayerSigh.enabled = true;
        furnitureLayerSigh.enabled = false;
        floorLayerSigh.enabled = false;
        RefreshDecorLayer();
    }

    public void ShowFloorLayer()
    {
        SoundManager.Instance.PlaySfx("Click");
        homeDepot.ShowFloorDepot();
        if (kuang.activeSelf) kuang.SetActive(false);
        currentLayer = FurnishCategory.Floor;
        decorLayerSigh.enabled = false;
        furnitureLayerSigh.enabled = false;
        floorLayerSigh.enabled = true;
        roomData.roomManager.ShowFloorCells();
    }

    public void PreviewNewInstance(Vector3 hitPoint) { 
        switch (currentLayer)
        {
            case FurnishCategory.Furniture:
                if (currentFurnitureInstance != null)
                {
                    roomData.roomManager.PreviewNewFurniture(creatingInstanceDataId, hitPoint, currentFurnitureInstance.furnitureObject.transform.position, currentFurnitureInstance);
                }
                else {
                     currentFurnitureInstance = roomData.roomManager.PreviewNewFurniture(creatingInstanceDataId, hitPoint, null, null);
                }
                    
                break;
            case FurnishCategory.Decor:
                if (currentDecorInstance != null)
                {
                    roomData.roomManager.PreviewNewDecor(creatingInstanceDataId, hitPoint, currentDecorInstance.decorObject.transform.position, currentDecorInstance);
                }
                else
                {
                    currentDecorInstance = roomData.roomManager.PreviewNewDecor(creatingInstanceDataId, hitPoint, null, null);
                }
                break;
            case FurnishCategory.Floor:
                //roomData.roomManager.PreviewNewFloor(creatingInstanceDataId, hitPoint);
                break;
        }
        ShowCancelBinOKButtons();
        isInstanceSelected = true;
        HomeDepotCover.enabled = true;
    }

    public void SelectDepotItem(string id, FurnishCategory category) {
        if (category != currentLayer) return;
        ClearHint();
        switch (currentLayer)
        {
            case FurnishCategory.Furniture:
                ShowHint("点击格子放置该家具");
                break;
            case FurnishCategory.Decor:
                ShowHint("点击格子放置该装饰"); 
                break;
            case FurnishCategory.Floor:
                ShowHint("点击格子绘制地板");
                break;
        }
        isCreatingNewInstance = true;
        creatingInstanceDataId = id;
    }


    private void Vibrate(GameObject obj)
    {
        obj.transform.DOShakePosition(0.5f, 0.1f, 10, 90, false, true);
    }

}