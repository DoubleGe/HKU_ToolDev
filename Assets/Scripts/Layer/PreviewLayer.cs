using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PreviewLayer : MonoBehaviour
{
    [SerializeField] private Grid previewGrid;
    [SerializeField] private Tilemap previewTileMap;
    [SerializeField] private TilemapRenderer tileMapRender;

    private void Start()
    {
        EventManager.OnLayerChanged += LayerChanged;
    }

    private void OnDestroy()
    {
        EventManager.OnLayerChanged -= LayerChanged;
    }

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

    public void LayerChanged(TileLayer layer)
    {
        tileMapRender.sortingOrder = layer.GetSortingOrder() + 1;
    }
}
