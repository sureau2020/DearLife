using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomView : MonoBehaviour
{
    [SerializeField] private Tilemap decorMap;
    [SerializeField] private Tilemap fornitureMap;
    [SerializeField] private Tilemap groundMap;
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //TODO: 完成RoomView的Initialize方法
    public void Initialize(GridMap gridMap, TileDataBase tileDataBase)
    {
    }
}
