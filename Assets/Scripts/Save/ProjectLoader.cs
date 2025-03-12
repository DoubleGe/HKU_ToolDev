using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ProjectLoader : MonoBehaviour
{
    private List<LoadButtonProject> loadedButtons;
    [SerializeField] private LoadButtonProject loadButtonPrefab;
    [SerializeField] private Transform loadedButtonParent;

    private void Awake()
    {
        loadedButtons = new List<LoadButtonProject>();
    }

    private void OnEnable()
    {
        for (int i = loadedButtons.Count - 1; i >= 0; i--)
        {
            Destroy(loadedButtons[i].gameObject);
            loadedButtons.RemoveAt(i);
        }

        string[] foldersFound = SimpleSave.GetFolders("Projects");
        List<ProjectFolder> projectFolders = new List<ProjectFolder>();

        foreach (string folderPath in foldersFound)
        {
            string[] folderSplit = folderPath.Split('\\');
            string folderName = folderSplit[folderSplit.Length - 1];

            if (SimpleSave.FileExist(Path.Combine("Projects", folderName, "ProjectData.step")))
            {
                projectFolders.Add(new ProjectFolder(folderName, SimpleSave.GetFolderLastWriteTime(Path.Combine("Projects", folderName))));
            }
        }

        projectFolders = projectFolders.OrderByDescending(f => f.editDate).ToList();
         
        projectFolders.ForEach(p =>
        {
            LoadButtonProject tempButotn = Instantiate(loadButtonPrefab, loadedButtonParent);
            loadedButtons.Add(tempButotn);
            tempButotn.InitLoadButton(p.folderName, p.editDate);
        });
    }

    private struct ProjectFolder
    {
        public string folderName;
        public DateTime editDate;

        public ProjectFolder(string  folderName, DateTime editDate)
        {
            this.folderName = folderName;
            this.editDate = editDate;
        }
    }
}
