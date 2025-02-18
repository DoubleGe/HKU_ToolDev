using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePainter : GenericSingleton<TilePainter>
{
    private PaintTool currentTool;
    private TileBase tile;

    private void Update()
    {
        if (currentTool == null) return;

        currentTool.RunTool();
    }

    public void SetTool(PaintTool tool)
    {
        currentTool = tool;
        currentTool.SetTile(tile);
    }

    public void TileUpdate(TileBase tile)
    {
        this.tile = tile;
        currentTool?.SetTile(tile);
    }
}
