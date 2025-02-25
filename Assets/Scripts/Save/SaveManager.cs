using SFB;
using System.Collections.Generic;
using System.IO;
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

    public void LoadProject(string projectPath)
    {

    }
}

