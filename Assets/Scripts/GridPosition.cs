using UnityEngine;

public struct GridPosition
{
    public int x;
    public int y;
    public GridPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public GridPosition(Vector3Int vector3Int)
    {
        x = vector3Int.x;
        y = vector3Int.y;
    }

    public static bool operator ==(GridPosition a, GridPosition b)
    {
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(GridPosition a, GridPosition b)
    {
        return a.x != b.x || a.y != b.y;
    }

    public override readonly bool Equals(object obj)
    {
        if (obj is GridPosition position)
        {
            return this == position;
        }
        return false;
    }

    public override readonly int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static GridPosition operator +(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x + b.x, a.y + b.y);
    }

    public static GridPosition operator -(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x - b.x, a.y - b.y);
    }

    public static GridPosition operator *(GridPosition a, int b)
    {
        return new GridPosition(a.x * b, a.y * b);
    }

    public readonly Vector3Int GetGridPositionVector3Int()
    {
        return new Vector3Int(x, y, 0);
    }
}