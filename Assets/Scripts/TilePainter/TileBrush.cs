using UnityEngine;
using UnityEngine.Tilemaps;

public class TileBrush : PaintTool
{
    public override void RunTool()
    {
        if (currentLayer == null) return;
        if (tileData == null) return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(InputHandler.controls.Player.MousePosition.ReadValue<Vector2>()) - new Vector3(.5f, .5f);
        Vector2Int tilePosition = new Vector2Int(Mathf.FloorToInt(mousePosition.x), Mathf.FloorToInt(mousePosition.y));

        previewLayer.ClearPreview();
        previewLayer.SetPreviewTile(tilePosition, tileData.tile);

        if (InputHandler.controls.Player.PlaceTile.IsPressed()) currentLayer.SetTile(tilePosition, tileData);
    }
}
