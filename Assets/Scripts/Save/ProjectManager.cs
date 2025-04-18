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

    [SerializeField] private SharedBoolScriptable allowInput;
    private bool opendLoadViaButton;

    private void Start()
    {
        closeBtn.SetActive(false);
        allowInput.value = false;
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
        allowInput.value = false;
    }

    public void SetProjectName()
    {
        if (string.IsNullOrEmpty(projectNameInput.text)) return;

        projectData = new ProjectData();
        projectData.projectName = projectNameInput.text;
        projectData.gridType = usedGridType;

        LayerManager.Instance.SetGridType(usedGridType);
        EventManager.OnProjectReset?.Invoke();

        ResetCreationMenu();
        allowInput.value = true;
    }

    public void OpenLoadWindow()
    {
        ResetCreationMenu();
        startPanel.SetActive(true);
        loadMenu.SetActive(true);
        opendLoadViaButton = false;
    }

    public void LoadWindowButton()
    {
        ResetCreationMenu();
        startPanel.SetActive(true);
        creationMenu.SetActive(false);
        loadMenu.SetActive(true);
        allowInput.value = false;
        opendLoadViaButton = true;
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
        allowInput.value = true;
    }

    public void CloseProjectWindow()
    {
        ResetCreationMenu();
        if (opendLoadViaButton) allowInput.value = true;
        else
        {
            startPanel.SetActive(true);
            creationMenu.SetActive(true);
        }
    }

    public void SaveProject()
    {
        if (saveManager == null) saveManager = new SaveManager();

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

            EventManager.OnProjectLoaded?.Invoke();

            allowInput.value = true;
        }
    }

    public void ExportProject()
    {
        if (saveManager == null) saveManager = new SaveManager();

        List<TileButton> tileButtons = TileGroup.Instance.GetAllTiles();

        List<TileLayer> layer = LayerManager.Instance.GetAllTileLayers();

        if (saveManager.ExportProject(projectData, tileButtons, layer))
        {
            Debug.Log("Project exported");
        }
    }
}
