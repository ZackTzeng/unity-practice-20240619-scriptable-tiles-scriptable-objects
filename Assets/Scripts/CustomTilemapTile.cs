using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Custom Tilemap Tile", menuName = "ScriptableTiles/Custom Tilemap Tile")]
public class CustomTilemapTile : TileBase
{
    // [SerializeField] private CustomTilemapTileSO customTilemapTileSO;
    [SerializeField] private Sprite tileSprite;
    [SerializeField] private TilemapTileType tileType;
    [SerializeField] private bool isWalkable;
    [HideInInspector] public bool showTranslucency = false;
    [SerializeField] private Color defaultColor = Color.white;
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = tileSprite;
        tileData.colliderType = Tile.ColliderType.Grid;
        // tileData.color = Color.white;
        tileData.flags = TileFlags.None;

        if (showTranslucency)
        {
            tileData.color = new Color(Color.white.r, Color.white.g, Color.white.b, 0.8f);
        }
        else
        {
            // tileData.color = defaultColor;
            tileData.color = Color.white;
        }
    }

    public void SetTranslucency(bool showTranslucency)
    {
        this.showTranslucency = showTranslucency;
    }

    public bool GetTranslucency()
    {
        return showTranslucency;
    }

    public bool IsWalkable()
    {
        return isWalkable;
    }
}
