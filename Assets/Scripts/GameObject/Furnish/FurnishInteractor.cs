using DG.Tweening;
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
    public int offsetYForTopStateUI = 520;
    public int offsetYForHomeDepotUI = -720;
    [SerializeField] private GameObject layerChangeButtons;
    [SerializeField] private Button furnishButton;
    [SerializeField] private GameObject kuang;
    [SerializeField] private Image decorLayerSigh;
    [SerializeField] private Image furnitureLayerSigh;
    [SerializeField] private Image floorLayerSigh;
    [SerializeField] private TextMeshProUGUI hint;

    private FurnitureInstance currentFurnitureInstance;
    private DecorInstance currentDecorInstance;
    private enum Layer { Furniture, Decor, Floor }
    private Layer currentLayer = Layer.Furniture;
    private bool isInstanceSelected = false;
    private bool isInEditMode = false;


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
                if (isInstanceSelected)
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
            case Layer.Furniture:
                CheckFurnitureMove(hitPoint);
                break;
            case Layer.Decor:
                CheckDecorMove(hitPoint);
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
        }
    }

    public void CancelSelection()
    {
        HideCancelBinOkButtons();
        switch (currentLayer)
        {
            case Layer.Furniture:
                RefreshFrnitureLayer();
                currentFurnitureInstance.furnitureObject.transform.position = roomData.roomManager.GetCellWorldLeftBottomPosition(currentFurnitureInstance.anchorPos);
                currentFurnitureInstance = null;
                ShowHint("点击绿色格子选择相应部件");
                break;
            case Layer.Decor:
                RefreshDecorLayer();
                currentDecorInstance.decorObject.transform.position = roomData.roomManager.GetCellWorldLeftBottomPosition(currentDecorInstance.position);
                currentDecorInstance = null;
                ShowHint("点击绿色格子选择相应部件");
                break;
            case Layer.Floor:
                //roomData.roomManager.ShowFloorCells();
                break;
        }
    }

    public void ConfirmMove()
    {
        switch (currentLayer)
        {
            case Layer.Furniture:
                if (currentFurnitureInstance != null)
                {
                    if (roomData.roomManager.ConfirmMoveFurniture(currentFurnitureInstance))
                    {
                        HideCancelBinOkButtons();
                        RefreshFrnitureLayer();
                        ShowHint("点击绿色格子选择相应部件");
                    }
                    else
                    {
                        SoundManager.Instance.PlaySfx("Error");
                        Vibrate(currentFurnitureInstance.furnitureObject);
                        ShowHint("当前位置不可放置此家具");
                    }
                }
                break;
            case Layer.Decor:
                if (currentDecorInstance != null) {
                    if (roomData.roomManager.ConfirmMoveDecor(currentDecorInstance))
                    {
                        HideCancelBinOkButtons();
                        RefreshDecorLayer();
                        ShowHint("点击绿色格子选择相应部件");
                    }
                    else
                    {
                        SoundManager.Instance.PlaySfx("Error");
                        Vibrate(currentDecorInstance.decorObject);
                        ShowHint("当前位置不可放置此装饰");
                    }
                }
                break;
            case Layer.Floor:
                //roomData.roomManager.ShowFloorCells();
                break;
        }
        
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
        if (currentLayer == Layer.Furniture && currentFurnitureInstance != null)
        {
            roomData.roomManager.RemoveFurniture(currentFurnitureInstance);
            currentFurnitureInstance = null;
            HideCancelBinOkButtons();
            RefreshFrnitureLayer();
            ShowHint("点击绿色格子选择相应部件");

        }
        else if (currentLayer == Layer.Decor && currentDecorInstance != null)
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
        furnishButton.interactable = true;
        layerChangeButtons.SetActive(true);
        cancelBinOkButton.SetActive(false);
    }

    public void Initialize(IRoomDataProvider provider)
    {
        roomData = provider;
        layerChangeButtons.SetActive(true);
        isInEditMode = true;
        if(!hint.isActiveAndEnabled) hint.gameObject.SetActive(true);
        ShowHint("点击绿色格子选择相应部件");
        ShowFurnitureLayer();
        MoveUI(TopStateUI, offsetYForTopStateUI, 0);
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
        MoveUI(TopStateUI,offsetYForTopStateUI, 0);
        MoveUI(HomeDepotUI, 0,offsetYForHomeDepotUI);
    }

    private void ClearHint()
    {
        hint.text = "";
        if (hint.isActiveAndEnabled) hint.gameObject.SetActive(false);
    }

    private void ShowHint(string t)
    {
        hint.text = "<wave>"+t;
    }

    public void ShowFurnitureLayer()
    {
        SoundManager.Instance.PlaySfx("Click");
        if (kuang.activeSelf) kuang.SetActive(false);
        currentLayer = Layer.Furniture;
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


    private void Vibrate(GameObject obj)
    {
        obj.transform.DOShakePosition(0.5f, 0.1f, 10, 90, false, true);
    }

}