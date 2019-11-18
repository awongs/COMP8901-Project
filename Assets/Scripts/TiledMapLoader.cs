using UnityEditor;
using UnityEngine;

public class TiledMapLoader : MonoBehaviour
{
    private const int FLOOR_TILE = 1;

    // Path to the level .json file.
    public string levelPath;

    // Floor tile prefab.
    public GameObject floorTile;

    // Wall tile prefab.
    public GameObject wallTile;

    public void LoadLevel()
    {
        // Parse .json file into level information.
        string levelJson = System.IO.File.ReadAllText(levelPath);
        TiledMap level = JsonUtility.FromJson<TiledMap>(levelJson);
       
        // Fill with the level tiles.
        foreach (TileLayer tileLayer in level.layers)
        {
            for (int x = 0; x < tileLayer.width; x++)
            {
                for (int y = 0; y < tileLayer.height; y++)
                {
                    int data = tileLayer.data[x * tileLayer.width + y];

                    if (data == FLOOR_TILE)
                    {
                        GameObject tile = Instantiate(floorTile, transform);
                        tile.transform.position = new Vector3(x, -0.5f * tile.transform.localScale.y, y);
                    }
                    else
                    {
                        GameObject tile = Instantiate(wallTile, transform);
                        tile.transform.position = new Vector3(x, 0.5f * tile.transform.localScale.y - 1.0f, y);
                    }
                    
                }
            }
        }
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
