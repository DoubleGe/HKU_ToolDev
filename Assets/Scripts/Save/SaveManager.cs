using SFB;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Rendering.Universal.ShaderGUI;
using UnityEngine;

//STEP = Save Tile Editor Project
//STER = Save Tile Editor Resourec
//STEL = Save Tile Editor Layer
public class SaveManager
{
    public void SaveProject(ProjectData projectData, List<TileButton> tiles, List<TileLayer> tileLayers)
    {
        string projectPath = "Projects";
        if (!SimpleSave.FolderExist(projectPath)) SimpleSave.CreateFolder(projectPath);

        //Project Data
        string projectDataPath = Path.Combine(projectPath, projectData.projectName);
        if (!SimpleSave.FolderExist(projectDataPath)) SimpleSave.CreateFolder(projectDataPath);

        SimpleSave.SaveJson<ProjectData>(projectData, Path.Combine(projectDataPath, "ProjectData.step"), SimpleSave.JSONMode.prettyprint);

        //Tile Resources
        string resourcePath = Path.Combine(projectDataPath, "Resources");
        if (!SimpleSave.FolderExist(resourcePath)) SimpleSave.CreateFolder(resourcePath);
        else SimpleSave.DeleteAllFilesInFolder(resourcePath);

        TileResources tileResources = new TileResources();
        foreach (TileButton tile in tiles)
        {
            CustomTileData tileData = tile.GetCustomTile();

            string tileLocation = $"Tile-{tileData.tileID}.png";
            tileResources.tiles.Add(new ResourceData(tileData.tileID, tileLocation, tileData.texture.filterMode));

            SimpleSave.SavePNG(tileData.texture, Path.Combine(resourcePath, tileLocation));
        }

        SimpleSave.SaveJson<TileResources>(tileResources, Path.Combine(projectDataPath, "TileResources.ster"), SimpleSave.JSONMode.prettyprint);

        //Layers
        string layerPath = Path.Combine(projectDataPath, "Layers");
        if (!SimpleSave.FolderExist(layerPath)) SimpleSave.CreateFolder(layerPath);
        else SimpleSave.DeleteAllFilesInFolder(layerPath);

        foreach (TileLayer layer in tileLayers)
        {
            SimpleSave.SaveJson<LayerData>(layer.GetLayerData(), Path.Combine(layerPath, layer.GetName() + ".stel"), SimpleSave.JSONMode.prettyprint);
        }
    }

    public bool LoadProject(string project, out ProjectData projectData, out List<ResourceReturn> resourceReturns, out List<LayerData> layerDatas)
    {
        projectData = null;
        resourceReturns = null;
        layerDatas = null;

        string projectPath = "Projects";
        if (!SimpleSave.FolderExist(projectPath)) return false;

        string projectDataPath = Path.Combine(projectPath, project);
        if (!SimpleSave.FolderExist(projectDataPath)) return false;

        projectData = SimpleSave.LoadJson<ProjectData>(Path.Combine(projectDataPath, "ProjectData.step"));

        string resourcePath = Path.Combine(projectDataPath, "Resources");
        if (!SimpleSave.FolderExist(resourcePath)) return false;

        TileResources tileResources = SimpleSave.LoadJson<TileResources>(Path.Combine(projectDataPath, "TileResources.ster"));

        List<ResourceReturn> rr = new List<ResourceReturn>();
        foreach (ResourceData tr in tileResources.tiles)
        {
            string tileLocation = Path.Combine(resourcePath, $"Tile-{tr.tileID}.png");
            Texture2D texture = SimpleSave.LoadPNG(tileLocation, 1, 1);
            texture.filterMode = tr.filterMode;

            rr.Add(new ResourceReturn(texture, tr.tileID));
        }
        resourceReturns = rr;

        string layerPath = Path.Combine(projectDataPath, "Layers");
        if (!SimpleSave.FolderExist(layerPath)) return false;

        string[] layerFiles = SimpleSave.GetFiles(layerPath);

        layerDatas = new List<LayerData>();
        foreach(string layer in layerFiles)
        {
            string[] layerSplit = layer.Split('\\');
            string layerName = layerSplit[layerSplit.Length - 1];

            LayerData layerData = SimpleSave.LoadJson<LayerData>(Path.Combine(layerPath, layerName));
            layerDatas.Add(layerData);
        }

        return true;
    }
}

