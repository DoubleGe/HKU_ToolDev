using System.Collections.Generic;
using UnityEngine;

public class TilePicker : PaintTool
{
    private PaintTool prevTool;

    public override void SetTool()
    {
        prevTool = tilePainter.GetTool();

        if (prevTool as TilePicker != null) prevTool = null;

        base.SetTool();
    }

    public override void RunTool()
    {
        if (currentLayer == null) return;

        if (IsPointerOverUIObject()) return;

        if (InputHandler.controls.Player.PlaceTile.triggered)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(InputHandler.controls.Player.MousePosition.ReadValue<Vector2>()) - new Vector3(.5f, .5f) - (Vector3)currentLayer.GetOffset();
            Vector2Int tilePosition = new Vector2Int(Mathf.FloorToInt(mousePosition.x), Mathf.FloorToInt(mousePosition.y));

            int tileID = currentLayer.GetTile(tilePosition);

            if (tileID == -1) return;

            TileGroup.Instance.SetTile(TileGroup.Instance.GetTileWithID(tileID));

            if (prevTool != null)
            {
                tilePainter.SetTool(prevTool);
                prevTool = null;
            }
        }
    }
}
