using UnityEngine;
using UnityEngine.Tilemaps;

public class TileLine : PaintTool
{
    private bool startDrag;
    private Vector2Int dragStart;

    public override void RunTool()
    {
        if (currentLayer == null || tileData == null) return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(InputHandler.controls.Player.MousePosition.ReadValue<Vector2>()) - new Vector3(.5f, .5f); ;
        Vector2Int tilePosition = new Vector2Int(Mathf.FloorToInt(mousePosition.x), Mathf.FloorToInt(mousePosition.y));

        if (InputHandler.controls.Player.PlaceTile.triggered)
        {
            startDrag = true;
            dragStart = tilePosition;
        }

        if (startDrag && InputHandler.controls.Player.PlaceTile.WasReleasedThisFrame())
        {
            DrawLine(dragStart, tilePosition);
            startDrag = false;
        }
    }

    private void DrawLine(Vector2Int start, Vector2Int end)
    {
        int dx = Mathf.Abs(end.x - start.x);
        int dy = Mathf.Abs(end.y - start.y);
        int sx = (start.x < end.x) ? 1 : -1;
        int sy = (start.y < end.y) ? 1 : -1;
        int err = dx - dy;

        int x = start.x;
        int y = start.y;

        while (true)
        {
            currentLayer.SetTile(new Vector2Int(x, y), tileData);
            if (x == end.x && y == end.y) break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y += sy;
            }
        }
    }

}
