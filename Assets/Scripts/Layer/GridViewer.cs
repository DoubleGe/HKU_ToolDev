using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridViewer : MonoBehaviour
{
    private void Start()
    {
        EventManager.OnLayerChanged += OnLayerChanged;
        EventManager.OnLayerSettingsChanged += OnLayerChanged;
    }

    private void OnDestroy()
    {
        EventManager.OnLayerChanged -= OnLayerChanged;
        EventManager.OnLayerSettingsChanged -= OnLayerChanged;
    }
    private void OnLayerChanged(TileLayer layer)
    {
        transform.position = layer.GetOffset() + Vector2.one * .5f;
    }
}
