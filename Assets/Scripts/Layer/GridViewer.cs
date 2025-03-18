using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Experimental.GraphView.GraphView;

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
        if(currentLayer != null) transform.position = camPos + currentLayer.GetOffset() + Vector2.one * .5f;
    }

    private void OnLayerChanged(TileLayer layer)
    {
        currentLayer = layer;
        transform.position = (Vector2)camPosition.transform.position + layer.GetOffset() + Vector2.one * .5f;
    }
}
