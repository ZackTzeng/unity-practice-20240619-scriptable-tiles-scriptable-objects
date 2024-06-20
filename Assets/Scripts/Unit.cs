using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private GridPosition gridPosition;
    private Level level;
    [SerializeField] private int movementRange;
    [SerializeField] private SpriteRenderer idleVisual;
    [SerializeField] private SpriteRenderer selectedVisual;

    public void Spawn(Level level, GridPosition gridPosition)
    {
        this.level = level;
        this.gridPosition = gridPosition;
        idleVisual.enabled = true;
        selectedVisual.enabled = false;
    }
    public void Move(GridPosition targetGridPosition)
    {
        gridPosition = targetGridPosition;
    }
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
    public void Select()
    {
        idleVisual.enabled = false;
        selectedVisual.enabled = true;
    }

    public void Deselect()
    {
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

    public void SetGridPosition(GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
    }
    public void SetLevel(Level level)
    {
        this.level = level;
    }
    public Level GetLevel()
    {
        return level;
    }
}
