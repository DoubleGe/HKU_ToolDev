using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProjectManager : GenericSingleton<ProjectManager>
{
    [Header("Creation Menu")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject creationMenu;
    [SerializeField] private GameObject loadMenu;
    [SerializeField] private GameObject closeBtn;

    [Header("Project Name")]
    [SerializeField] private GameObject projectNameWindow;
    [SerializeField] private TMP_InputField projectNameInput;

    private ProjectData projectData;
    [SerializeField] private SaveManager saveManager;

    private GridType usedGridType;

    private void Start()
    {
        closeBtn.SetActive(false);
    }

    public void CreateNewProject() => CreateNewProject((int)usedGridType);
    public void CreateNewProject(int gridType)
    {
        usedGridType = (GridType)gridType;

        projectNameInput.text = "";
        ResetCreationMenu();
        startPanel.SetActive(true);
        projectNameWindow.SetActive(true);
    }

    public void NewProjectButton()
    {
        ResetCreationMenu();
        startPanel.SetActive(true);
        creationMenu.SetActive(true);
        closeBtn.SetActive(true);
    }

    public void SetProjectName()
    {
        if (string.IsNullOrEmpty(projectNameInput.text)) return;

        projectData = new ProjectData();
        projectData.projectName = projectNameInput.text;
        projectData.gridType = usedGridType;

        LayerManager.Instance.SetGridType(usedGridType);
        TileGroup.Instance.RemoveAllTileButtons();

        ResetCreationMenu();
    }

    public void OpenLoadWindow()
    {
        ResetCreationMenu();
        startPanel.SetActive(true);
        loadMenu.SetActive(true);
    }

    public void LoadWindowButton()
    {
        ResetCreationMenu();
        startPanel.SetActive(true);
        creationMenu.SetActive(false);
        loadMenu.SetActive(true);
    }

    private void ResetCreationMenu()
    {
        startPanel.SetActive(false);
        creationMenu.SetActive(true);
        loadMenu.SetActive(false);
        projectNameWindow.SetActive(false);
    }

    public void CloseMenu()
    {
        ResetCreationMenu();
    }

    public void SaveProject()
    {
        if(saveManager == null) saveManager = new SaveManager();

        List<TileButton> tileButtons = TileGroup.Instance.GetAllTiles();

        List<TileLayer> layer = LayerManager.Instance.GetAllTileLayers();

        saveManager.SaveProject(projectData, tileButtons, layer);
    }

    public void LoadProject(string projectName)
    {
        if (saveManager == null) saveManager = new SaveManager();

        if (saveManager.LoadProject(projectName, out ProjectData projectData, out List<ResourceReturn> resourceReturns, out List<LayerData> layerData))
        {
            this.projectData = projectData;

            TileGroup.Instance.SetLoadedTiles(resourceReturns);
            LayerManager.Instance.LoadLayersFromSave(layerData);

            ResetCreationMenu();
        }
    }

    public void ExportProject()
    {
        if(saveManager == null) saveManager = new SaveManager();

        List<TileButton> tileButtons = TileGroup.Instance.GetAllTiles();

        List<TileLayer> layer = LayerManager.Instance.GetAllTileLayers();

        if (saveManager.ExportProject(projectData, tileButtons, layer))
        {
            Debug.Log("Project exported");
        }
    }
}
