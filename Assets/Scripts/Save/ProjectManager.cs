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

    private GridType usedGridType;

    public void CreateNewProject() => CreateNewProject((int)usedGridType);
    public void CreateNewProject(int gridType)
    {
        usedGridType = (GridType)gridType;

        projectNameInput.text = "";
        creationMenu.SetActive(false);
        projectNameWindow.SetActive(true);
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

    public void LoadProject()
    {

    }
}
