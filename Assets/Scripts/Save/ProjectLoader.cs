using System.Collections.Generic;
using System.IO;
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
        List<string> validFolder = new List<string>();

        foreach (string folderPath in foldersFound)
        {
            string[] folderSplit = folderPath.Split('\\');
            string folderName = folderSplit[folderSplit.Length - 1];

            if (SimpleSave.FileExist(Path.Combine("Projects", folderName, "ProjectData.step")))
            {
                validFolder.Add(folderName);               
            }
        }

        validFolder.ForEach(p =>
        {
            LoadButtonProject tempButotn = Instantiate(loadButtonPrefab, loadedButtonParent);
            loadedButtons.Add(tempButotn);
            tempButotn.InitLoadButton(p, SimpleSave.GetFolderLastWriteTime(p));
        });
    }
}
