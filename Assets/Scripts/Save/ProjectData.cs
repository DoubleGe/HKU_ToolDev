using SimpleSaver.Intern;
using System.Collections.Generic;
using UnityEngine;

public enum GridType { Rectangle, Hex, Isometric }

[System.Serializable]
public class ProjectData
{
    public string projectName;
    public GridType gridType;

}

[System.Serializable]
public class ResourceData
{
    public string fileLocation;
    public int tileID;
    public FilterMode filterMode;

    public ResourceData(int tileID, string fileLocation, FilterMode filterMode)
    {
        this.tileID = tileID;
        this.fileLocation = fileLocation;
        this.filterMode = filterMode;
    }
}

[System.Serializable]
public class TileResources
{
    public List<ResourceData> tiles;

    public TileResources()
    {
        tiles = new List<ResourceData>();
    }
}

[System.Serializable]
public class LayerData
{
    public int layerIndex;
    public SerializableDictionary<Vector2Int, int> tileMap;
    public string layerName;
    public Vector2 offset;

    public LayerData(int layerINdex, SerializableDictionary<Vector2Int, int> tileMap, string layerName, Vector2 offset)
    {
        this.layerIndex = layerINdex;
        this.tileMap = tileMap;
        this.layerName = layerName;
        this.offset = offset;
    }
}

[System.Serializable]
public class ResourceReturn
{
    public int tileID;
    public Texture2D texture;

    public ResourceReturn(Texture2D texture, int tileId)
    {
        this.tileID = tileId;
        this.texture = texture;
    }
}

[System.Serializable]
public class LayerExport
{
    public int layerIndex;
    public string layerName;
    public int width;
    public int height;
    public int[] tileMap;

    public LayerExport(int layerIndex, int[] tileMap, int width, int height, string layerName)
    {
        this.layerIndex = layerIndex;
        this.tileMap = tileMap;
        this.width = width;
        this.height = height;
        this.layerName = layerName;
    }
}