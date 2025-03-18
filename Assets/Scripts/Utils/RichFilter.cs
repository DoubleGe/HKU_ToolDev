using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RichFilter : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    public void UpdateText()
    {
        inputField.text = inputField.text.Replace("<", "");
        inputField.text = inputField.text.Replace(">", "");
    }
}
