using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject
{
    private Unit unit;
    public string testString;
    public TileObject()
    {
        
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
