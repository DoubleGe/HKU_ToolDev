using UnityEngine;
using UnityEngine.Tilemaps;

public class TileDrag : PaintTool
{
    [SerializeField] private Tile tempTile;

    private bool startDrag;
    private Vector2Int dragStart;

    public override void RunTool()
    {
        if (currentTilemap == null) return;
        if (tempTile == null) return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(InputHandler.controls.Player.MousePosition.ReadValue<Vector2>());

        if (InputHandler.controls.Player.PlaceTile.triggered)
        {
            startDrag = true;
            dragStart = new Vector2Int(Mathf.FloorToInt(mousePosition.x), Mathf.FloorToInt(mousePosition.y));
        }

        if(startDrag && InputHandler.controls.Player.PlaceTile.WasReleasedThisFrame())
        {
            Vector2Int endDrag = new Vector2Int(Mathf.FloorToInt(mousePosition.x), Mathf.FloorToInt(mousePosition.y));
            startDrag = false;

            int startX = Mathf.Min(dragStart.x, endDrag.x);
            int endX = Mathf.Max(dragStart.x, endDrag.x);
            int startY = Mathf.Min(dragStart.y, endDrag.y);
            int endY = Mathf.Max(dragStart.y, endDrag.y);

            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    currentTilemap.SetTile(tilePosition, tempTile);
                }
            }
        }
    }
}
