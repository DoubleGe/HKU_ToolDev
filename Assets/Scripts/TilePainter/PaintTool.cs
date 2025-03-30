using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public abstract class PaintTool : MonoBehaviour
{
    protected TileLayer currentLayer;
    protected TilePainter tilePainter;
    protected CustomTileData tileData;
    protected PreviewLayer previewLayer;

    [SerializeField] private Image buttonImage;
    [SerializeField] private List<Image> iconImages;

    protected virtual void Awake()
    {
        //I hate FindObjectOfType...
        tilePainter = FindFirstObjectByType<TilePainter>();
        previewLayer = FindFirstObjectByType<PreviewLayer>();

        EventManager.OnLayerChanged += LayerChanged;
        EventManager.OnToolSelected += ResetSelection;
    }

    protected virtual void OnDestroy()
    {
        EventManager.OnLayerChanged -= LayerChanged;
        EventManager.OnToolSelected -= ResetSelection;
    }

    private void LayerChanged(TileLayer tileLayer)
    {
        currentLayer = tileLayer;
    }

    public abstract void RunTool();

    public virtual void SetTool()
    {
        tilePainter.SetTool(this);
        EventManager.OnToolSelected?.Invoke();

        buttonImage.color = new Color32(76, 108, 255, 255);
        iconImages.ForEach(i => i.color = Color.white);
    }

    private void ResetSelection()
    {
        buttonImage.color = Color.white;
        iconImages.ForEach(i => i.color = new Color32(47, 47, 47, 255));
    }

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
