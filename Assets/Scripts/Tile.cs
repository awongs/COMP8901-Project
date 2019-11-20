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
}
