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

            GridPosition gridPosition = GetGridPosition(GetMouseWorldPosition());
            Unit selectedUnit = SelectUnit();
            if (selectedUnit != null)
            {

                if (activeUnit == null)
                {
                    activeUnit = selectedUnit;
                    activeUnit.Select();

                    // SetMovableCellsTranslucency(activeUnit, gridPosition, true);
                }
                else if (activeUnit != selectedUnit)
                {
                    activeUnit.Deselect();
                    activeUnit = selectedUnit;
                    activeUnit.Select();

                    // SetMovableCellsTranslucency(activeUnit, gridPosition, true);

                }
                else // activeUnit already is the selected unit
                {
                    // SetMovableCellsTranslucency(activeUnit, gridPosition, false);
                    activeUnit.Deselect();
                    activeUnit = null;
                }
            }
            else // no unit selected
            {
                if (activeUnit != null) // with an active unit
                {
                    MoveUnit(activeUnit, gridPosition);
                }
                else // with no active unit
                {
                    Debug.Log("No unit selected");
                }
            }

        }

        // if (Input.GetMouseButtonDown(0))
        // {
        //     Vector3Int gridPosition = GetGridPosition(GetMouseWorldPosition());
        //     Unit selectedUnit = SelectUnit();
        //     if (selectedUnit != null)
        //     {

        //         if (activeUnit == null)
        //         {
        //             activeUnit = selectedUnit;
        //             activeUnit.Select();

        //             // SetMovableCellsTranslucency(activeUnit, gridPosition, true);
        //         }
        //         else if (activeUnit != selectedUnit)
        //         {
        //             activeUnit.Deselect();
        //             activeUnit = selectedUnit;
        //             activeUnit.Select();

        //             // SetMovableCellsTranslucency(activeUnit, gridPosition, true);

        //         }
        //         else // activeUnit already is the selected unit
        //         {
        //             // SetMovableCellsTranslucency(activeUnit, gridPosition, false);
        //             activeUnit.Deselect();
        //             activeUnit = null;
        //         }
        //     }
        //     else // no unit selected
        //     {
        //         if (activeUnit != null) // with an active unit
        //         {
        //             MoveUnit(activeUnit, gridPosition);
        //             SetMovableCellsTranslucency(activeUnit, activeUnit.GetGridPosition(), false);
        //             SetUnitOnGridPosition(activeUnit.GetGridPosition(), null);
        //             activeUnit.Move(GetTileCenterWorldPosition(GetMouseWorldPosition()));
        //             activeUnit.SetGridPosition(gridPosition);
        //             SetMovableCellsTranslucency(activeUnit, activeUnit.GetGridPosition(), true);
        //             SetUnitOnGridPosition(gridPosition, activeUnit);
        //         }
        //         else
        //         {
        //             Debug.Log("No unit selected");
        //             // CustomTilemapTile tile = (CustomTilemapTile)tilemap.GetTile(GetGridPosition(GetMouseWorldPosition()));
        //             // UpdateTileTranslucency(!tile.GetTranslucency(), GetGridPosition(GetMouseWorldPosition()));
        //         }
        //     }

        // }
    }

    public GridPosition GetGridPosition(WorldPosition worldPosition)
    {
        Vector3 worldPositionVector3 = worldPosition.GetWorldPositionVector3();
        Vector3Int gridPositionVector3Int = grid.WorldToCell(worldPositionVector3);
        GridPosition gridPosition = new(gridPositionVector3Int);
        return gridPosition;
    }
    // public Vector3Int GetGridPosition(Vector2 worldPosition)
    // {
    //     return grid.WorldToCell(worldPosition);
    // }

    public WorldPosition GetMouseWorldPosition()
    {
        Vector3 mouseWorldPositionVector3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        WorldPosition mouseWorldPosition = new(mouseWorldPositionVector3);
        return mouseWorldPosition;
    }
    // public Vector2 GetMouseWorldPosition()
    // {
    //     Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //     return new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, 0);
    // }

    public WorldPosition GetTileCenter(WorldPosition worldPosition)
    {
        GridPosition gridPosition = GetGridPosition(worldPosition);
        Vector3Int gridPositionVector3Int = gridPosition.GetGridPositionVector3Int();
        Vector3 tileCenterWorldPositionVector3 = tilemap.GetCellCenterWorld(gridPositionVector3Int);
        WorldPosition tileCenter = new(tileCenterWorldPositionVector3);
        return tileCenter;
    }


    // public Vector2 GetTileCenterWorldPosition(Vector2 worldPosition)
    // {
    //     Vector3Int gridPosition = GetGridPosition(worldPosition);
    //     return tilemap.GetCellCenterWorld(gridPosition);
    // }

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

    // public void SpawnUnit()
    // {
    //     Vector3Int spawnPositionGridPosition = new Vector3Int(0, 0, 0);
    //     Vector2 spawnPositionWorldPosition = GetWorldPosition(spawnPositionGridPosition);
    //     Debug.Log($"Spawn position: {spawnPositionWorldPosition}");
    //     GameObject unitGameObject = Instantiate(unitPrefab, spawnPositionWorldPosition, Quaternion.identity);
    //     Unit unit = unitGameObject.GetComponent<Unit>();
    //     unit.Spawn(this, (CustomTilemapTile)tilemap.GetTile(spawnPositionGridPosition), spawnPositionGridPosition);
    //     // unit.SetLevel(this);
    //     // unit.SetTile((CustomTilemapTile)tilemap.GetTile(spawnPositionGridPosition));
    //     // unit.Move(spawnPositionWorldPosition);
    //     // unit.SetGridPosition(spawnPositionGridPosition);
    //     SetUnitOnGridPosition(spawnPositionGridPosition, unit);
    // }

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

    // public void UpdateTileTranslucency(Boolean showTranslucency, Vector3Int tilemapPosition)
    // {
    //     TileBase tile = tilemap.GetTile(tilemapPosition);

    //     if (tile != null && tile is CustomTilemapTile tile1)
    //     {
    //         Debug.Log("Tile is CustomTilemapTile");
    //         CustomTilemapTile customTile = tile1;

    //         // Toggle the translucency
    //         customTile.SetTranslucency(showTranslucency);

    //         // Set the tile back to the tilemap to apply changes
    //         tilemap.SetTile(tilemapPosition, null); // Clear the tile first
    //         tilemap.SetTile(tilemapPosition, customTile); // Set the modified tile
    //         tilemap.RefreshTile(tilemapPosition);
    //     }
    // }

    public void SetMovableCellsTranslucency(Unit unit, GridPosition gridPosition, bool showTranslucency)
    {
        List<GridPosition> movableCells = GetMovableCells(gridPosition, unit.GetMovementRange());
        foreach (GridPosition movableCell in movableCells)
        {
            UpdateTileTranslucency(showTranslucency, movableCell);
        }
    }

    // public void SetMovableCellsTranslucency(Unit unit, Vector3Int gridPosition, bool showTranslucency)
    // {
    //     List<Vector3Int> movableCells = GetMovableCells(gridPosition, unit.GetMovementRange());
    //     foreach (Vector3Int movableCell in movableCells)
    //     {
    //         UpdateTileTranslucency(showTranslucency, movableCell);
    //     }
    // }

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
                        if (!HasAnyUnitOnGridPosition(direction))
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
    // public List<Vector3Int> GetMovableCells(Vector3Int position, int distance)
    // {
    //     HashSet<Vector3Int> movableCells = new HashSet<Vector3Int>();
    //     Queue<Vector3Int> queue = new Queue<Vector3Int>();

    //     movableCells.Add(position);
    //     queue.Enqueue(position);

    //     for (int step = 0; step < distance; step++)
    //     {
    //         int count = queue.Count;
    //         for (int i = 0; i < count; i++)
    //         {
    //             Vector3Int current = queue.Dequeue();

    //             Vector3Int[] directions = {
    //                 new Vector3Int(current.x, current.y + 1), // Up
    //                 new Vector3Int(current.x, current.y - 1), // Down
    //                 new Vector3Int(current.x - 1, current.y), // Left
    //                 new Vector3Int(current.x + 1, current.y)  // Right
    //             };

    //             foreach (Vector3Int direction in directions)
    //             {
    //                 if (!movableCells.Contains(direction))
    //                 {
    //                     if (!HasAnyUnitOnGridPosition(direction))
    //                     {
    //                         movableCells.Add(direction);
    //                         queue.Enqueue(direction);
    //                     }
    //                 }
    //             }
    //         }
    //     }

    //     return new List<Vector3Int>(movableCells);
    // }

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
    // public bool HasAnyUnitOnGridPosition(Vector3Int gridPosition)
    // {
    //     TileBase tile = tilemap.GetTile(gridPosition);
    //     if (tile is CustomTilemapTile customTile)
    //     {
    //         return customTile.HasUnit();
    //     }
    //     return false;
    // }

    public void SetUnitOnGridPosition(GridPosition gridPosition, Unit unit)
    {
        Vector3Int gridPositionVector3Int = gridPosition.GetGridPositionVector3Int();
        TileBase tile = tilemap.GetTile(gridPositionVector3Int);
        if (tile is CustomTilemapTile customTile)
        {
            customTile.SetUnit(unit);
        }
    }
    // public void SetUnitOnGridPosition(Vector3Int gridPosition, Unit unit)
    // {
    //     TileBase tile = tilemap.GetTile(gridPosition);
    //     if (tile is CustomTilemapTile customTile)
    //     {
    //         customTile.SetUnit(unit);
    //     }
    // }

    public void ClearUnitOnGridPosition(GridPosition gridPosition)
    {
        SetUnitOnGridPosition(gridPosition, null);
    }
    // public void ClearUnitOnGridPosition(Vector3Int gridPosition)
    // {
    //     SetUnitOnGridPosition(gridPosition, null);
    // }

    public void MoveUnit(Unit unit, GridPosition targetGridPosition)
    {
        SetMovableCellsTranslucency(unit, unit.GetGridPosition(), false);
        ClearUnitOnGridPosition(unit.GetGridPosition());
        unit.Move(targetGridPosition);
        SetMovableCellsTranslucency(unit, unit.GetGridPosition(), true);
        SetUnitOnGridPosition(targetGridPosition, unit);
    }

    // public void MoveUnit(Unit unit, Vector3Int targetGridPosition)
    // {
    //     SetMovableCellsTranslucency(unit, unit.GetGridPosition(), false);
    //     ClearUnitOnGridPosition(unit.GetGridPosition());
    //     unit.Move(GetTileCenterWorldPosition(GetWorldPosition(targetGridPosition)));
    //     unit.SetGridPosition(targetGridPosition);
    //     SetMovableCellsTranslucency(unit, unit.GetGridPosition(), true);
    //     SetUnitOnGridPosition(targetGridPosition, unit);
    // }
}
