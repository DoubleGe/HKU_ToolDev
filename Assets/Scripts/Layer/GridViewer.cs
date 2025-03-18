using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridViewer : MonoBehaviour
{
    private TileLayer currentLayer;
    [SerializeField] private Transform camPosition;

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

    private void Update()
    {
        Vector2 camPos = new Vector2(Mathf.RoundToInt(camPosition.transform.position.x), Mathf.RoundToInt(camPosition.transform.position.y));
        if (currentLayer != null)
        {
            Vector2 offset = currentLayer.GetOffset();
            offset = new Vector2(offset.x - Mathf.Floor(offset.x), offset.y - Mathf.Floor(offset.y));
            transform.position = camPos + offset + Vector2.one * .5f;
        }
    }

    private void OnLayerChanged(TileLayer layer)
    {
        currentLayer = layer;
    }
}
