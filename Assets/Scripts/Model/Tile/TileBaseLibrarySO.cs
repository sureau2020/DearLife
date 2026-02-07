
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileLibrary", menuName = "Map/TileBaseLibrary")]
public class TileBaseLibrarySO : ScriptableObject
{
    public List<TileEntry> tileResources = new List<TileEntry>();
}