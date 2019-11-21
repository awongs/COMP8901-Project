using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum TileType
    {
        FLOOR,
        WALL
    }

    public int x;
    public int y;
    public TileType tileType;

    public Dictionary<Tuple<int, int>, Tile> neighbours = new Dictionary<Tuple<int, int>, Tile>();
}
