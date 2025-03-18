using UnityEngine;

public class TileErase : PaintTool
{
    private bool startDrag;
    private Vector2Int dragStart;

    public override void RunTool()
    {
        if (currentLayer == null) return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(InputHandler.controls.Player.MousePosition.ReadValue<Vector2>()) - new Vector3(.5f, .5f) - (Vector3)currentLayer.GetOffset();
        Vector2Int tilePosition = new Vector2Int(Mathf.FloorToInt(mousePosition.x), Mathf.FloorToInt(mousePosition.y));

        previewLayer.ClearPreview();

        if (InputHandler.controls.Player.PlaceTile.triggered)
        {
            startDrag = true;
            dragStart = tilePosition;
        }

        if (startDrag)
        {
            Vector2Int endDrag = tilePosition;

            int startX = Mathf.Min(dragStart.x, endDrag.x);
            int endX = Mathf.Max(dragStart.x, endDrag.x);
            int startY = Mathf.Min(dragStart.y, endDrag.y);
            int endY = Mathf.Max(dragStart.y, endDrag.y);

            if (InputHandler.controls.Player.PlaceTile.WasReleasedThisFrame())
            {
                startDrag = false;

                for (int x = startX; x <= endX; x++)
                {
                    for (int y = startY; y <= endY; y++)
                    {
                        Vector2Int newTilePos = new Vector2Int(x, y);
                        currentLayer.SetTile(newTilePos, null);
                    }
                }
            }
        }
    }
}
