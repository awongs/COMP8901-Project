using UnityEditor;
using UnityEngine;

public class TiledMapLoader : MonoBehaviour
{
    // Define Tiled data constants here.
    private const int FLOOR_TILE = 1;
    private const int WALL_TILE = 58;

    private const string TILE_LAYER = "Tile Layer";
    private const string ENEMY_LAYER = "Enemy Layer";

    private const int TILE_WIDTH = 32;
    private const int TILE_HEIGHT = 32;

    // Path to the level .json file.
    public string levelPath;

    // Floor tile prefab.
    public GameObject floorTile;

    // Wall tile prefab.
    public GameObject wallTile;

    // Enemy prefab.
    public GameObject enemy;

    public void LoadLevel()
    {
        // Parse .json file into level information.
        string levelJson = System.IO.File.ReadAllText(levelPath);
        TiledMap level = JsonUtility.FromJson<TiledMap>(levelJson);
       
        // Fill with the level tiles.
        foreach (Layer layer in level.layers)
        {
            switch (layer.name)
            {
                case TILE_LAYER:
                    for (int x = 0; x < layer.width; x++)
                    {
                        for (int y = 0; y < layer.height; y++)
                        {
                            SpawnTile(layer, x, y);
                        }
                    }

                    // Set dimensions in level.
                    Level.MaxX = layer.width;
                    Level.MaxY = layer.height;

                    break;
                case ENEMY_LAYER:
                    foreach (TileObject tileObj in layer.objects)
                    {
                        SpawnEnemy(tileObj);
                    }
                    break;
                default:
                    break;
            }
        }

        GetComponent<Level>().BuildTiles();
    }

    public void SpawnTile(Layer layer, int x, int y)
    {
        int data = layer.data[y * layer.width + x];

        if (data == FLOOR_TILE)
        {
            GameObject tileObj = Instantiate(floorTile, transform);
            tileObj.transform.position = new Vector3(x, -1.0f * tileObj.transform.localScale.y, -y);

            // Setup tile component.
            Tile tile = tileObj.GetComponent<Tile>();
            tile.tileType = Tile.TileType.FLOOR;
            tile.x = x;
            tile.y = y;
        }
        else if (data == WALL_TILE)
        {
            GameObject tileObj = Instantiate(wallTile, transform);
            tileObj.transform.position = new Vector3(x, 0.5f * tileObj.transform.localScale.y - 1.5f, -y);

            // Setup tile component.
            Tile tile = tileObj.GetComponent<Tile>();
            tile.tileType = Tile.TileType.WALL;
            tile.x = x;
            tile.y = y;
        }
    }

    public void SpawnEnemy(TileObject tileObj)
    {
        GameObject enemyObj = Instantiate(enemy, transform);
        enemyObj.transform.position = new Vector3(tileObj.x / TILE_WIDTH, enemyObj.transform.position.y, -tileObj.y / TILE_HEIGHT + 1.0f);
    }

    public void ClearLevel()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}

[CustomEditor(typeof(TiledMapLoader))]
public class TiledMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TiledMapLoader tiledMapLoader = (TiledMapLoader)target;
        if (GUILayout.Button("Load Level"))
        {
            tiledMapLoader.LoadLevel();
        }

        if (GUILayout.Button("Clear Level"))
        {
            tiledMapLoader.ClearLevel();
        }
    }
}
