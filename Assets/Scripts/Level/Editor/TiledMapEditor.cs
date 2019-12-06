using UnityEditor;
using UnityEngine;

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
