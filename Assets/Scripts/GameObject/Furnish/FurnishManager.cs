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

    void OnEnable() => FurnishItemEvents.OnItemClicked += HandleDepotItemClick;
    void OnDisable()
    {
        FurnishItemEvents.OnItemClicked -= HandleDepotItemClick;
        RoomManager.OnRoomManagerInitialized -= OnRoomManagerInitialized;
    }

    void Start()
    {
        _roomManager = GetComponent<RoomManager>();
        if (_roomManager != null)
            OnRoomManagerInitialized(_roomManager);
    }

    void OnRoomManagerInitialized(RoomManager room)
    {
        _roomManager = room;
        furnishButton.interactable = true;
    }



    public void EnterEditMode()
    {
        isInFurnishMode = !isInFurnishMode;
        if (!isInFurnishMode)
        {
            furnishInteractor.Close();
            onEnterFurnishMode?.Invoke(false);
        }
        else
        { // 进入编辑模式
            furnishInteractor.Initialize(this);
            onEnterFurnishMode?.Invoke(true);
        }

    }

    public void HandleDepotItemClick(string id, FurnishCategory category, int index)
    {
        if (!isInFurnishMode)
            return;
        furnishInteractor.SelectDepotItem(id, category);
    }
}

public interface IRoomDataProvider
{
    RoomManager roomManager { get; }
}