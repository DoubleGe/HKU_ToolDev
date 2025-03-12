using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePainter : GenericSingleton<TilePainter>
{
    private PaintTool currentTool;
    private CustomTileData tileData;

    private void Update()
    {
        if (currentTool == null) return;

        currentTool.RunTool();
    }

    public void SetTool(PaintTool tool)
    {
        currentTool = tool;
        currentTool.SetTile(tileData);
    }

    public void TileUpdate(CustomTileData tile)
    {
        this.tileData = tile;
        currentTool?.SetTile(tileData);
    }
}
