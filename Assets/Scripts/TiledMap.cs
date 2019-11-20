[System.Serializable]
public class TiledMap
{
    public int height;
    public int width;
    public int tileheight;
    public int tilewidth;
    public Layer[] layers;
}

[System.Serializable]
public class Layer
{
    public string name;
    public int height;
    public int width;
    public int[] data;
    public TileObject[] objects;
}

[System.Serializable]
public class TileObject
{
    public TileProperty[] properties;
    public int x;
    public int y;
}

[System.Serializable]
public class TileProperty
{
    public string name;
    public string type;
    public bool value;
}