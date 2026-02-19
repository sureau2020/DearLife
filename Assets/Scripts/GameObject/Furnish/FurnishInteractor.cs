using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FurnishInteractor : MonoBehaviour
{
    [SerializeField] private GameObject layerChangeButtons;
    [SerializeField] private Image decorLayerSigh;
    [SerializeField] private Image furnitureLayerSigh;
    [SerializeField] private Image floorLayerSigh;
    


    public void Initialize() { 
        layerChangeButtons.SetActive(true);
        ShowFurnitureLayer();
    }
    public void ShowFurnitureLayer() { 
        SoundManager.Instance.PlaySfx("Click");
        decorLayerSigh.enabled = false;
        furnitureLayerSigh.enabled = true;
        floorLayerSigh.enabled = false;
    }

    public void ShowDecorLayer()
    {
        SoundManager.Instance.PlaySfx("Click");
        decorLayerSigh.enabled = true;
        furnitureLayerSigh.enabled = false;
        floorLayerSigh.enabled = false;
    }

    public void ShowFloorLayer()
    {
        SoundManager.Instance.PlaySfx("Click");
        decorLayerSigh.enabled = false;
        furnitureLayerSigh.enabled = false;
        floorLayerSigh.enabled = true;
    }



}
