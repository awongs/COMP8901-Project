[System.Serializable]
public class TiledMap
{
   public int compressionlevel;
   public int height;
   public bool infinite;
   public TileLayer[] layers;
   public int nextlayerid;
   public int nextobjectid;
   public string orientation;
   public string renderorder;
   public string tiledversion;
   public int tileheight;
   public TileSet[] tilesets;
   public int tilewidth;
   public string map;
   public float version;
   public int width;
}

[System.Serializable]
public class TileLayer
{
    public int[] data;
    public int height;
    public int id;
    public string name;
    public int opacity;
    public string type;
    public bool visible;
    public int width;
    public int x;
    public int y;
}

[System.Serializable]
public class TileSet
{
    public int firstgid;
    public string source;
}