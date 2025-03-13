using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileFill : PaintTool
{
    [SerializeField] private int maxTileFill = 150;

    public override void RunTool()
    {
        if (currentLayer == null) return;
        if (tileData == null) return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(InputHandler.controls.Player.MousePosition.ReadValue<Vector2>()) - new Vector3(.5f, .5f); ;
        Vector3Int startPos = new Vector3Int(Mathf.FloorToInt(mousePosition.x), Mathf.FloorToInt(mousePosition.y), 0);

        previewLayer.ClearPreview();

        //Blocks rendering of preview tile when layer is inactive.
        if (!currentLayer.IsVisible) return;

        previewLayer.SetPreviewTile((Vector2Int)startPos, tileData.tile);

        if (currentLayer.GetTilemap().GetTile(startPos) != null) return;

        //Blocks if max tile count is exceeded
        if (!CanFillArea(startPos, maxTileFill)) return;

        FloodFillPreview(startPos);

        if (InputHandler.controls.Player.PlaceTile.triggered)
        {
            FloodFill(startPos);
        }
    }

    private bool CanFillArea(Vector3Int startPos, int maxTiles)
    {
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        queue.Enqueue(startPos);
        visited.Add(startPos);

        int count = 0;

        while (queue.Count > 0)
        {
            Vector3Int pos = queue.Dequeue();
            count++;

            if (count > maxTiles) return false;

            Vector3Int[] neighbors = {
            new Vector3Int(pos.x + 1, pos.y, 0),
            new Vector3Int(pos.x - 1, pos.y, 0),
            new Vector3Int(pos.x, pos.y + 1, 0),
            new Vector3Int(pos.x, pos.y - 1, 0)
        };

            foreach (var neighbor in neighbors)
            {
                if (!visited.Contains(neighbor) && currentLayer.GetTilemap().GetTile(neighbor) == null)
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        return true;
    }

    private void FloodFill(Vector3Int startPos)
    {
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        queue.Enqueue(startPos);
        visited.Add(startPos);

        while (queue.Count > 0)
        {
            Vector3Int pos = queue.Dequeue();
            currentLayer.SetTile((Vector2Int)pos, tileData);

            Vector3Int[] neighbors = {
            new Vector3Int(pos.x + 1, pos.y, 0),
            new Vector3Int(pos.x - 1, pos.y, 0),
            new Vector3Int(pos.x, pos.y + 1, 0),
            new Vector3Int(pos.x, pos.y - 1, 0)
        };

            foreach (var neighbor in neighbors)
            {
                if (!visited.Contains(neighbor) && currentLayer.GetTilemap().GetTile(neighbor) == null)
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }
    }

    private void FloodFillPreview(Vector3Int startPos)
    {
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        queue.Enqueue(startPos);
        visited.Add(startPos);

        while (queue.Count > 0)
        {
            Vector3Int pos = queue.Dequeue();
            previewLayer.SetPreviewTile((Vector2Int)pos, tileData.tile);

            Vector3Int[] neighbors = {
                new Vector3Int(pos.x + 1, pos.y, 0),
                new Vector3Int(pos.x - 1, pos.y, 0),
                new Vector3Int(pos.x, pos.y + 1, 0),
                new Vector3Int(pos.x, pos.y - 1, 0)
            };

            foreach (var neighbor in neighbors)
            {
                if (!visited.Contains(neighbor) && currentLayer.GetTilemap().GetTile(neighbor) == null)
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }
    }
}