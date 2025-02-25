using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProjectManager : GenericSingleton<ProjectManager>
{
    [Header("Creation Menu")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject creationMenu;

    [Header("Project Name")]
    [SerializeField] private GameObject projectNameWindow;
    [SerializeField] private TMP_InputField projectNameInput;

    private ProjectData projectData;
    [SerializeField] private SaveManager saveManager;

    private GridType usedGridType;

    public void CreateNewProject() => CreateNewProject((int)usedGridType);
    public void CreateNewProject(int gridType)
    {
        usedGridType = (GridType)gridType;

        projectNameInput.text = "";
        creationMenu.SetActive(false);
        projectNameWindow.SetActive(true);
    }

    public void NewProjectButton()
    {
        startPanel.SetActive(true);
        creationMenu.SetActive(true);
        projectNameWindow.SetActive(false);
    }

    public void SetProjectName()
    {
        if (string.IsNullOrEmpty(projectNameInput.text)) return;

        projectData = new ProjectData();
        projectData.projectName = projectNameInput.text;
        projectData.gridType = usedGridType;

        LayerManager.Instance.SetGridType(usedGridType);
        TileGroup.Instance.RemoveAllTileButtons();

        startPanel.SetActive(false);
        creationMenu.SetActive(true);
        projectNameWindow.SetActive(false);
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

        if (saveManager.LoadProject(projectName, out ProjectData projectData, out List<ResourceReturn> resourceReturns))
        {
            this.projectData = projectData;

            TileGroup.Instance.SetLoadedTiles(resourceReturns);
        }
    }
}
