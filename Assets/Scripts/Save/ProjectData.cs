using SimpleSaver.Intern;
using System.Collections.Generic;
using UnityEngine;

public enum GridType { Rectangle, Hex, Isometric }

public class ProjectData
{
    public string projectName;
    public GridType gridType;

}

public class ResourceData
{
    public string resourceLocation;
    public int tileID;
}

public class LayerData
{
    public int layerIndex;
    public SerializableDictionary<string, int> tileMap;
}