
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileConfig", menuName = "Map/TileConfig")]
public class TileConfigSO : ScriptableObject
{
    public List<TileData> tileProperties = new List<TileData>();
}


