using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadButtonProject : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fileNameText;
    [SerializeField] private TextMeshProUGUI lastEditedText;

    public void InitLoadButton(string fileName, DateTime lastEdited)
    {
        fileNameText.text = fileName;
        lastEditedText.text = lastEdited.ToString("HH-mm dd-MM-yyyy");
    }

    public void LoadProject()
    {

    }
}
