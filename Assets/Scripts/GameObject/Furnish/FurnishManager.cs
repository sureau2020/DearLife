using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FurnishManager : MonoBehaviour
{
    [SerializeField] private Button furnishButton;
    [SerializeField] private FurnishInteractor furnishInteractor;
    private RoomManager roomManager;
    

    void Awake()
    {
        RoomManager.OnRoomManagerInitialized += OnRoomManagerInitialized;
    }

    void OnRoomManagerInitialized(RoomManager room) {
        roomManager = room;
        furnishButton.enabled = true;
    }

    void OnDestroy() {
        RoomManager.OnRoomManagerInitialized -= OnRoomManagerInitialized;
    }

    public void EnterEditMode() { 
        furnishInteractor.Initialize();
    }



}
