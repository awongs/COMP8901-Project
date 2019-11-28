using System;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public static Dictionary<Tuple<int, int>, Tile> tiles;

    public static Tile TileAt(int x, int y)
    {
        return tiles[new Tuple<int, int>(x, y)];
    }

    public static Tile RandomTileNear(Tile tile)
    {
        List<Tile> availableTiles = new List<Tile>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                

                Tile nearbyTile = TileAt(tile.x + x, tile.y + y);
                if (nearbyTile != null && nearbyTile.tileType == Tile.TileType.FLOOR)
                {
                    availableTiles.Add(nearbyTile);
                }
            }
        }

        if (availableTiles.Count == 0)
        {
            Debug.LogWarning("Couldn't find any neighbouring tiles for some reason.");
            return null;
        }

        // Return a random available tile.
        int randomIndex = UnityEngine.Random.Range(0, availableTiles.Count);
        return availableTiles[randomIndex];
    }

    private void Awake()
    {
        tiles = new Dictionary<Tuple<int, int>, Tile>();

        foreach (Transform child in transform)
        {
            Tile tile = child.GetComponent<Tile>();
            if (tile != null)
            {
                tiles.Add(new Tuple<int, int>(tile.x, tile.y), tile);
            }
        }

        foreach (Tile tile in tiles.Values)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    // Don't consider diagonal or center tiles.
                    if (Mathf.Abs(x) == Mathf.Abs(y))
                    {
                        continue;
                    }

                    Tuple<int, int> tilePos = new Tuple<int, int>(tile.x + x, tile.y + y);
                    tiles.TryGetValue(tilePos, out Tile value);

                    if (value != null) {
                        tile.neighbours.Add(new Tuple<int, int>(x, y), value);
                    }
                }
            }
        }
    }

}
