using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct WorldPosition
{
    public float x;
    public float y;
    public WorldPosition(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
    public WorldPosition(Vector3 vector3)
    {
        x = vector3.x;
        y = vector3.y;
    }
    public WorldPosition(Vector2 vector2)
    {
        x = vector2.x;
        y = vector2.y;
    }

    // override equality
    public static bool operator ==(WorldPosition a, WorldPosition b)
    {
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(WorldPosition a, WorldPosition b)
    {
        return a.x != b.x || a.y != b.y;
    }

    public override readonly bool Equals(object obj)
    {
        if (obj is WorldPosition position)
        {
            return this == position;
        }
        return false;
    }

    public override readonly int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static WorldPosition operator +(WorldPosition a, WorldPosition b)
    {
        return new WorldPosition(a.x + b.x, a.y + b.y);
    }

    public static WorldPosition operator -(WorldPosition a, WorldPosition b)
    {
        return new WorldPosition(a.x - b.x, a.y - b.y);
    }

    public static WorldPosition operator *(WorldPosition a, float b)
    {
        return new WorldPosition(a.x * b, a.y * b);
    }

    public readonly Vector2 GetWorldPositionVector2()
    {
        return new Vector2(x, y);
    }

    public readonly Vector3 GetWorldPositionVector3()
    {
        return new Vector3(x, y, 0);
    }
}
