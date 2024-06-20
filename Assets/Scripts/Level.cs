using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level : MonoBehaviour
{
    private Grid grid;
    [SerializeField] private Unit unit;
    private Unit activeUnit;
    [SerializeField] private LayerMask unitLayerMask;
    [SerializeField] Tilemap tilemap;
    private void Start()
    {
        grid = GetComponent<Grid>();
        unit.Move(GetTileCenterWorldPosition(unit.transform.position));
        Debug.Log($"Unit name: {unit.name}");
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Unit selectedUnit = SelectUnit();
            if (selectedUnit != null)
            {
                if (activeUnit == null)
                {
                    activeUnit = selectedUnit;
                    activeUnit.Select();
                }
                else if (activeUnit != selectedUnit)
                {
                    activeUnit.Deselect();
                    activeUnit = selectedUnit;
                    activeUnit.Select();
                } else // activeUnit already is the selected unit
                {
                    activeUnit.Deselect();
                    activeUnit = null;
                }
            }
            else // no unit selected
            {
                if (activeUnit != null)
                {
                    activeUnit.Move(GetTileCenterWorldPosition(GetMouseWorldPosition()));
                }
            }

        }
    }

    public Vector3Int GetGridPosition(Vector2 worldPosition)
    {
        return grid.WorldToCell(worldPosition);
    }

    public Vector2 GetMouseWorldPosition()
    {
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, 0);
    }

    public Vector2 GetTileCenterWorldPosition(Vector2 worldPosition)
    {
        Vector3Int gridPosition = GetGridPosition(worldPosition);
        return tilemap.GetCellCenterWorld(gridPosition);
    }

    public Unit SelectUnit()
    {
        RaycastHit2D hit = Physics2D.Raycast(GetMouseWorldPosition(), Vector2.zero, Mathf.Infinity, unitLayerMask);
        if (hit.collider != null)
        {
            Debug.Log($"Unit selected {hit.collider.gameObject.name}");
            Unit selectedUnit = hit.collider.GetComponent<Unit>();
            return selectedUnit;
        }
        return null;
    }
}
