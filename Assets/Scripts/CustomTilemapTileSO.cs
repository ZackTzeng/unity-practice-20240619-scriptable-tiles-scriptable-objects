using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Custom Tilemap Tile SO", menuName = "ScriptableObjects/Custom Tilemap Tile SO")]
public class CustomTilemapTileSO : ScriptableObject
{
    public TilemapTileType tileType;
}
