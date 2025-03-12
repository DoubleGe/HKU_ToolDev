using SFB;
using SimpleSaver.Intern;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static SimpleSave;

//STEP = Save Tile Editor Project
//STER = Save Tile Editor Resource
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
        layerDatas = layerDatas.OrderByDescending(l => l.layerIndex).ToList();

        return true;
    }

    public bool ExportProject(ProjectData projectData, List<TileButton> tiles, List<TileLayer> tileLayers)
    {
        //Not using SimpleSave because of full custom path.
        string[] folderLocations = StandaloneFileBrowser.OpenFolderPanel("Export", "", false);
        string folderLoc = folderLocations[0]; 

        if (Directory.GetFiles(folderLoc).Length > 0 || Directory.GetDirectories(folderLoc).Length > 0)
        {
            folderLoc = Path.Combine(folderLoc, "TileEditor Export - " + projectData.projectName);

            Directory.CreateDirectory(folderLoc);
        }

        SaveJson<ProjectData>(projectData, Path.Combine(folderLoc, "ProjectData.step"), SimpleSave.JSONMode.prettyprint);

        string resourcePath = Path.Combine(folderLoc, "Resources");
        if (!Directory.Exists(resourcePath)) Directory.CreateDirectory(resourcePath);
        else Directory.Delete(resourcePath, true);

        TileResources tileResources = new TileResources();
        foreach (TileButton tile in tiles)
        {
            CustomTileData tileData = tile.GetCustomTile();

            string tileLocation = $"Tile-{tileData.tileID}.png";
            tileResources.tiles.Add(new ResourceData(tileData.tileID, tileLocation, tileData.texture.filterMode));

            SavePNG(tileData.texture, Path.Combine(resourcePath, tileLocation));
        }

        SaveJson<TileResources>(tileResources, Path.Combine(folderLoc, "TileResources.ster"), SimpleSave.JSONMode.prettyprint);

        string layerPath = Path.Combine(folderLoc, "Layers");
        if (!Directory.Exists(layerPath)) Directory.CreateDirectory(layerPath);
        else Directory.Delete(layerPath, true);

        List<LayerExport> layerExports = LayerConverter.ConvertAllLayers(LayerConverter.ConvertTileButton(tileLayers));
        foreach (LayerExport layer in layerExports)
        {
            SaveJson<LayerExport>(layer, Path.Combine(layerPath, layer.layerName + ".stel"), SimpleSave.JSONMode.prettyprint);
        }

        return true;
    }

    private void SaveJson<T>(T data, string path, JSONMode jsonMode = JSONMode.none)
    {
        string saveData = JsonUtility.ToJson(data, jsonMode == JSONMode.prettyprint);

        if (jsonMode == JSONMode.encrypted) saveData = EncryptorDecryptor.EncryptDecrypt(saveData);

        File.WriteAllText(path, saveData);
    }

    private void SavePNG(Texture2D texture2D, string path)
    {
        if (!Path.HasExtension(path) || Path.GetExtension(path) != ".png") path += ".png";

        byte[] png = texture2D.EncodeToPNG();
        File.WriteAllBytes(path, png);
    }

}

