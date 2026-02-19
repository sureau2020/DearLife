using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FurnishManager : MonoBehaviour, IRoomDataProvider
{
    [SerializeField] private Button furnishButton;
    [SerializeField] private FurnishInteractor furnishInteractor;
    private RoomManager _roomManager;
    public RoomManager roomManager => _roomManager;
    public static Action<bool> onEnterFurnishMode;
    private bool isInFurnishMode = false;


    void Start()
    {
        _roomManager = GetComponent<RoomManager>();
        if (_roomManager != null)
            OnRoomManagerInitialized(_roomManager);
    }

    void OnRoomManagerInitialized(RoomManager room) {
        _roomManager = room;
        furnishButton.interactable = true;
    }

    void OnDestroy() {
        RoomManager.OnRoomManagerInitialized -= OnRoomManagerInitialized;
    }

    public void EnterEditMode() { 
        isInFurnishMode = !isInFurnishMode;
        if (!isInFurnishMode) {
            onEnterFurnishMode?.Invoke(false);
        }else
        { // 进入编辑模式
            furnishInteractor.Initialize(this);
            onEnterFurnishMode?.Invoke(true);
        }
        
    }



}


public interface IRoomDataProvider
{
    RoomManager roomManager { get; }
}