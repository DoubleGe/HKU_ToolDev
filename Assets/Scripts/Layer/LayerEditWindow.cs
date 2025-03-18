using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LayerEditWindow : MonoBehaviour
{
    private TileLayer layerToEdit;

    [SerializeField] private GameObject layerEditorPanel;
    [SerializeField] private TMP_InputField layerNameInput;
    [SerializeField] private TMP_InputField layerOffsetX;
    [SerializeField] private TMP_InputField layerOffsetY;


    private void Start()
    {
        EventManager.OnLayerDoubleClicked += OpenLayerEditor;

        layerEditorPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        EventManager.OnLayerDoubleClicked -= OpenLayerEditor;
    }

    private void OpenLayerEditor(TileLayer layer)
    {
        layerToEdit = layer;
        
        layerNameInput.text = layer.GetName();
        layerOffsetX.text = layer.GetOffset().x.ToString();
        layerOffsetY.text = layer.GetOffset().y.ToString();

        layerEditorPanel.SetActive(true);
    }

    public void CancelButton() => layerEditorPanel.SetActive(false);

    public void ChangeButton()
    {
        if (string.IsNullOrEmpty(layerNameInput.text)) return;

        layerToEdit.SetName(layerNameInput.text);
        
        float offSetX = float.Parse(layerOffsetX.text);
        float offsetY = float.Parse(layerOffsetY.text);

        layerToEdit.SetOffset(new Vector2(offSetX, offsetY));

        EventManager.OnLayerSettingsChanged?.Invoke(layerToEdit);

        layerEditorPanel.SetActive(false);
    }
}
