using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level : MonoBehaviour
{
    private Grid grid;
    // [SerializeField] private Unit unit;
    private Unit activeUnit;
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private LayerMask unitLayerMask;
    [SerializeField] Tilemap tilemap;
    private void Start()
    {
        grid = GetComponent<Grid>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            GridPosition mousePositionGridPosition = GetGridPosition(GetMouseWorldPosition());
            Unit selectedUnit = SelectUnit();
            if (selectedUnit != null)
            {

                if (activeUnit == null)
                {
                    activeUnit = selectedUnit;
                    activeUnit.Select();
                    HighlightMovableCellsForUnit(activeUnit);
                }
                else if (activeUnit != selectedUnit)
                {
                    UnhighlightMovableCellsForUnit(activeUnit);
                    activeUnit.Deselect();
                    activeUnit = selectedUnit;
                    activeUnit.Select();
                    HighlightMovableCellsForUnit(activeUnit);

                }
                else // activeUnit already is the selected unit
                {
                    UnhighlightMovableCellsForUnit(activeUnit);
                    activeUnit.Deselect();
                    activeUnit = null;
                }
            }
            else // no unit selected
            {
                if (activeUnit != null) // with an active unit
                {
                    MoveUnit(activeUnit, mousePositionGridPosition);
                }
                else // with no active unit
                {
                    Debug.Log("No unit selected");
                }
            }

        }
    }

    public GridPosition GetGridPosition(WorldPosition worldPosition)
    {
        Vector3 worldPositionVector3 = worldPosition.GetWorldPositionVector3();
        Vector3Int gridPositionVector3Int = grid.WorldToCell(worldPositionVector3);
        GridPosition gridPosition = new(gridPositionVector3Int);
        return gridPosition;
    }

    public void HighlightMovableCellsForUnit(Unit unit)
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        SetMovableCellsTranslucency(unit, unitGridPosition, true);

    }

    public void UnhighlightMovableCellsForUnit(Unit unit)
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        SetMovableCellsTranslucency(unit, unitGridPosition, false);
    }

    public WorldPosition GetMouseWorldPosition()
    {
        Vector3 mouseWorldPositionVector3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        WorldPosition mouseWorldPosition = new(mouseWorldPositionVector3);
        return mouseWorldPosition;
    }

    public WorldPosition GetTileCenter(WorldPosition worldPosition)
    {
        GridPosition gridPosition = GetGridPosition(worldPosition);
        Vector3Int gridPositionVector3Int = gridPosition.GetGridPositionVector3Int();
        Vector3 tileCenterWorldPositionVector3 = tilemap.GetCellCenterWorld(gridPositionVector3Int);
        WorldPosition tileCenter = new(tileCenterWorldPositionVector3);
        return tileCenter;
    }

    public Vector2 GetWorldPosition(Vector3Int gridPosition)
    {
        Vector2 worldPosition = grid.GetCellCenterWorld(gridPosition);
        return worldPosition;
    }

    public WorldPosition GetWorldPosition(GridPosition gridPosition)
    {
        Vector3Int gridPositionVector3Int = new Vector3Int(
            gridPosition.x,
            gridPosition.y,
            0
        );
        Vector3 worldPositionVector3 = grid.GetCellCenterWorld(gridPositionVector3Int);
        return new WorldPosition(
            worldPositionVector3.x,
            worldPositionVector3.y
        );
    }

    public Unit SelectUnit()
    {
        Vector2 mouseWorldPositionVector2 = GetMouseWorldPosition().GetWorldPositionVector2();
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPositionVector2, Vector2.zero, Mathf.Infinity, unitLayerMask);
        if (hit.collider != null)
        {
            Unit selectedUnit = hit.collider.GetComponent<Unit>();
            return selectedUnit;
        }
        return null;
    }

    public void SpawnUnit()
    {
        GridPosition spawnPosition = new(0, 0);
        WorldPosition spawnPositionWorldPosition = GetWorldPosition(spawnPosition);
        Vector3Int spawnPositionVector3Int = spawnPosition.GetGridPositionVector3Int();

        GameObject unitGameObject = Instantiate(
            unitPrefab,
            spawnPositionWorldPosition.GetWorldPositionVector3(),
            Quaternion.identity
        );

        Unit unit = unitGameObject.GetComponent<Unit>();
        unit.Spawn(this, spawnPosition);
        SetUnitOnGridPosition(spawnPosition, unit);
    }

    public void UpdateTileTranslucency(bool showTranslucency, GridPosition tilemapPosition)
    {
        Vector3Int tilemapPositionVector3Int = tilemapPosition.GetGridPositionVector3Int();
        TileBase tile = tilemap.GetTile(tilemapPositionVector3Int);

        if (tile != null && tile is CustomTilemapTile tile1)
        {
            CustomTilemapTile customTile = tile1;

            // Toggle the translucency
            customTile.SetTranslucency(showTranslucency);

            // Set the tile back to the tilemap to apply changes
            tilemap.SetTile(tilemapPositionVector3Int, null); // Clear the tile first
            tilemap.SetTile(tilemapPositionVector3Int, customTile); // Set the modified tile
            tilemap.RefreshTile(tilemapPositionVector3Int);
        }
    }

    public void SetMovableCellsTranslucency(Unit unit, GridPosition gridPosition, bool showTranslucency)
    {
        List<GridPosition> movableCells = GetMovableCells(gridPosition, unit.GetMovementRange());
        foreach (GridPosition movableCell in movableCells)
        {
            UpdateTileTranslucency(showTranslucency, movableCell);
        }
    }

    public List<GridPosition> GetMovableCells(GridPosition position, int distance)
    {
        HashSet<GridPosition> movableCells = new();
        Queue<GridPosition> queue = new();

        movableCells.Add(position);
        queue.Enqueue(position);

        for (int step = 0; step < distance; step++)
        {
            int count = queue.Count;
            for (int i = 0; i < count; i++)
            {
                GridPosition current = queue.Dequeue();

                GridPosition[] directions = {
                    new(current.x, current.y + 1), // Up
                    new(current.x, current.y - 1), // Down
                    new(current.x - 1, current.y), // Left
                    new(current.x + 1, current.y)  // Right
                };

                foreach (GridPosition direction in directions)
                {
                    if (!movableCells.Contains(direction))
                    {
                        movableCells.Add(direction);
                        queue.Enqueue(direction);
                        // if (!HasAnyUnitOnGridPosition(direction) || GetUnitOnGridPosition(direction) == activeUnit)
                        // {
                        //     movableCells.Add(direction);
                        //     queue.Enqueue(direction);
                        // }
                        // else
                        // {
                        //     Debug.Log($"Unit on grid position: {direction}");
                        // }
                    }
                }
            }
        }

        return new List<GridPosition>(movableCells);
    }

    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
    {
        Vector3Int gridPositionVector3Int = gridPosition.GetGridPositionVector3Int();
        TileBase tile = tilemap.GetTile(gridPositionVector3Int);
        if (tile is CustomTilemapTile customTile)
        {
            return customTile.HasUnit();
        }
        return false;
    }

    public Unit GetUnitOnGridPosition(GridPosition gridPosition)
    {
        Vector3Int gridPositionVector3Int = gridPosition.GetGridPositionVector3Int();
        TileBase tile = tilemap.GetTile(gridPositionVector3Int);
        if (tile is CustomTilemapTile customTile)
        {
            return customTile.GetUnit();
        }
        return null;
    }

    public void SetUnitOnGridPosition(GridPosition gridPosition, Unit unit)
    {
        Vector3Int gridPositionVector3Int = gridPosition.GetGridPositionVector3Int();
        TileBase tile = tilemap.GetTile(gridPositionVector3Int);
        if (tile is CustomTilemapTile customTile)
        {
            customTile.SetUnit(unit);
        }
    }

    public void ClearUnitOnGridPosition(GridPosition gridPosition)
    {
        SetUnitOnGridPosition(gridPosition, null);
    }

    public void MoveUnit(Unit unit, GridPosition targetGridPosition)
    {
        UnhighlightMovableCellsForUnit(unit);
        ClearUnitOnGridPosition(unit.GetGridPosition());
        unit.Move(targetGridPosition);
        HighlightMovableCellsForUnit(unit);
        SetUnitOnGridPosition(targetGridPosition, unit);
    }
}
