using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Custom Tilemap Tile", menuName = "ScriptableTiles/Custom Tilemap Tile")]
public class CustomTilemapTile : TileBase
{
    // [SerializeField] private CustomTilemapTileSO customTilemapTileSO;
    [SerializeField] private Sprite tileSprite;
    private Unit unit;
    [SerializeField] public bool showTranslucency = false;
    [SerializeField] private Color defaultColor = Color.white;
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = tileSprite;
        tileData.colliderType = Tile.ColliderType.Grid;
        tileData.color = Color.white;
        tileData.flags = TileFlags.None;

        if (showTranslucency)
        {
            tileData.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0.5f);
        }
        else
        {
            tileData.color = defaultColor;
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

    public void SetUnit(Unit unit)
    {
        this.unit = unit;
    }
    public Unit GetUnit()
    {
        return unit;
    }
    public bool HasUnit()
    {
        return unit != null;
    }
}
