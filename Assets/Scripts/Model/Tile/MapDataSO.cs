using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TileInstanceData
{
    public Vector3Int position; // 格子坐标
    public string tileId;      // 对应 TileLogicData 的 id
}

[CreateAssetMenu(fileName = "MapData", menuName = "Map/MapData")]
public class MapDataSO : ScriptableObject
{
    public List<TileInstanceData> tileInstances = new List<TileInstanceData>();
}