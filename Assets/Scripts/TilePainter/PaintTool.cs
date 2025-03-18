using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public abstract class PaintTool : MonoBehaviour
{
    protected TileLayer currentLayer;
    protected TilePainter tilePainter;
    protected CustomTileData tileData;
    protected PreviewLayer previewLayer;

    protected virtual void Awake()
    {
        //I hate FindObjectOfType...
        tilePainter = FindFirstObjectByType<TilePainter>();
        previewLayer = FindFirstObjectByType<PreviewLayer>();

        EventManager.OnLayerChanged += LayerChanged;
    }

    protected virtual void OnDestroy()
    {
        EventManager.OnLayerChanged -= LayerChanged;
    }

    private void LayerChanged(TileLayer tileLayer)
    {
        currentLayer = tileLayer;
    }

    public abstract void RunTool();

    public virtual void SetTool() => tilePainter.SetTool(this);
    public void SetTile(CustomTileData tile) => this.tileData = tile;

    protected static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
