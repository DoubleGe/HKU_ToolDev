using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class PaintTool : MonoBehaviour
{
    protected TileLayer currentLayer;
    protected TilePainter tilePainter;
    protected CustomTileData tileData;

    protected virtual void Awake()
    {
        tilePainter = FindFirstObjectByType<TilePainter>();

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

    public void SetTool() => tilePainter.SetTool(this);
    public void SetTile(CustomTileData tile) => this.tileData = tile;
}
