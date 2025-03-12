using UnityEngine;
using UnityEngine.Tilemaps;

public class PreviewLayer : MonoBehaviour
{
    [SerializeField] private Grid previewGrid;
    [SerializeField] private Tilemap previewTileMap;

    public void SetGridType(GridLayout.CellLayout layout)
    {
        previewGrid.cellLayout = layout;
    }

    public void SetPreviewTile(Vector2Int position, TileBase tile)
    {
        previewTileMap.SetTile((Vector3Int)position, tile);
    }

    public void ClearPreview()
    {
        previewTileMap.ClearAllTiles();
    }
}
