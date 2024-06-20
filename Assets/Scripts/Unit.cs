using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private GridPosition gridPosition;
    private CustomTilemapTile tile;
    private Level level;
    [SerializeField] private int movementRange;
    [SerializeField] private SpriteRenderer idleVisual;
    [SerializeField] private SpriteRenderer selectedVisual;

    public void Spawn(Level level, CustomTilemapTile tile, GridPosition gridPosition)
    {
        this.level = level;
        this.tile = tile;
        this.gridPosition = gridPosition;
        idleVisual.enabled = false;
        selectedVisual.enabled = true;
    }
    public void SetTile(CustomTilemapTile tile)
    {
        this.tile = tile;
    }
    public void Move(GridPosition targetGridPosition)
    {
        gridPosition = targetGridPosition;
    }
    // public void Move(WorldPosition targetPosition)
    // {
    //     this.targetPosition = targetPosition;
    // }
    private void Update()
    {
        UpdateUnitPosition();
    }

    private void UpdateUnitPosition()
    {
        Vector3 currentTransformPosition = transform.position;
        WorldPosition currentTransformWorldPosition = new(
            transform.position
        );
        GridPosition currentTransformGridPosition = level.GetGridPosition(currentTransformWorldPosition);
        if (currentTransformGridPosition != gridPosition)
        {
            transform.position = level.GetWorldPosition(gridPosition).GetWorldPositionVector3();
        }
    }
    // private void UpdateUnitPosition()
    // {
    //     Vector3 currentTransformPosition = transform.position;
    //     GridPosition currentGridPosition = new(level.GetGridPosition(currentTransformPosition));
    //     if (currentGridPosition != gridPosition)
    //     {
    //         transform.position = level.GetWorldPosition(gridPosition).GetWorldPositionVector3();
    //     }
    // }
    public void Select()
    {
        idleVisual.enabled = false;
        selectedVisual.enabled = true;
        level.SetMovableCellsTranslucency(this, gridPosition, true);

    }

    public void Deselect()
    {
        level.SetMovableCellsTranslucency(this, gridPosition, false);
        idleVisual.enabled = true;
        selectedVisual.enabled = false;
    }
    public int GetMovementRange()
    {
        return movementRange;
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    // public Vector3Int GetGridPosition()
    // {
    //     return gridPosition;
    // }

    public void SetGridPosition(GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
    }
    // public void SetGridPosition(Vector3Int gridPosition)
    // {
    //     this.gridPosition = gridPosition;
    // }
    public void SetLevel(Level level)
    {
        this.level = level;
    }
    public Level GetLevel()
    {
        return level;
    }
}
