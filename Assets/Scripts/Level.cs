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
            Debug.Log($"Active unit: {activeUnit}");
            Debug.Log($"Selected unit: {selectedUnit}");
            if (selectedUnit != null)
            {

                if (activeUnit == null)
                {
                    Debug.Log("Selected a unit but no current active unit");
                    activeUnit = selectedUnit;
                    activeUnit.Select();
                    SetMovableCellsTranslucency(activeUnit, mousePositionGridPosition, true);
                }
                else if (activeUnit != selectedUnit)
                {
                    SetMovableCellsTranslucency(activeUnit, mousePositionGridPosition, false);
                    activeUnit.Deselect();
                    activeUnit = selectedUnit;
                    activeUnit.Select();
                    SetMovableCellsTranslucency(activeUnit, mousePositionGridPosition, true);

                }
                else // activeUnit already is the selected unit
                {
                    SetMovableCellsTranslucency(activeUnit, mousePositionGridPosition, false);
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
        Debug.Log($"World position: {worldPosition}");
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
        Debug.Log($"Spawn position world position: {spawnPositionWorldPosition}");
        Vector3Int spawnPositionVector3Int = spawnPosition.GetGridPositionVector3Int();

        GameObject unitGameObject = Instantiate(
            unitPrefab, 
            spawnPositionWorldPosition.GetWorldPositionVector3(), 
            Quaternion.identity
        );

        Unit unit = unitGameObject.GetComponent<Unit>();
        unit.Spawn(this, (CustomTilemapTile)tilemap.GetTile(spawnPositionVector3Int), spawnPosition);
        SetUnitOnGridPosition(spawnPosition, unit);
    }

    public void UpdateTileTranslucency(Boolean showTranslucency, GridPosition tilemapPosition)
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
            Debug.Log($"Movable cell: {movableCell}");
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
                        if (!HasAnyUnitOnGridPosition(direction) || GetUnitOnGridPosition(direction) == activeUnit)
                        {
                            movableCells.Add(direction);
                            queue.Enqueue(direction);
                        }
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
        SetMovableCellsTranslucency(unit, unit.GetGridPosition(), false);
        ClearUnitOnGridPosition(unit.GetGridPosition());
        unit.Move(targetGridPosition);
        SetMovableCellsTranslucency(unit, unit.GetGridPosition(), true);
        SetUnitOnGridPosition(targetGridPosition, unit);
    }
}
