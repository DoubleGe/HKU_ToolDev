using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class PaintTool : MonoBehaviour
{
    protected Tilemap currentTilemap;
    protected TilePainter tilePainter;
    protected TileBase tile;

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
        currentTilemap = tileLayer.GetTilemap();
    }

    public abstract void RunTool();

    public void SetTool() => tilePainter.SetTool(this);
    public void SetTile(TileBase tile) => this.tile = tile;
}
