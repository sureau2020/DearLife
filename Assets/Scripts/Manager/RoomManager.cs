using System;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GridMap gridMap { get; private set; }
    [SerializeField] private RoomView roomView;

    // 添加初始化完成事件
    public static event Action<GridMap> OnGridMapInitialized;
    
    // 标记是否已初始化
    public bool IsInitialized { get; private set; } = false;

    void Awake()
    {
        gridMap = new GridMap();// TODO 之后改成从存档里拿,现在先初始化一个默认房间，默认房间是无参数constructor
        roomView.Initialize(gridMap);
        
        // 标记初始化完成
        IsInitialized = true;
        
        // 触发事件通知其他组件
        OnGridMapInitialized?.Invoke(gridMap);
    }
}
