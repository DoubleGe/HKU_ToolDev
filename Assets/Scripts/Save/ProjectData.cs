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

    public LayerData(int layerINdex,  SerializableDictionary<Vector2Int, int> tileMap)
    {
        this.layerIndex = layerINdex;
        this.tileMap = tileMap;
    }
}

