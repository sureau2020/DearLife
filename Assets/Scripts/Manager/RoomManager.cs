
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    private GridMap gridMap;
    private TileDataBase tileDataBase;
    [SerializeField] private RoomView roomView;

    void Awake()
    {
        tileDataBase = new TileDataBase();// TODO 之后改成从LoaderManager里拿
        gridMap = new GridMap();// TODO 之后改成从存档里拿,现在先初始化一个默认房间，默认房间是无参数constructor
        roomView.Initialize(gridMap);
    }
}
