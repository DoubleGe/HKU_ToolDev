using UnityEngine;
using UnityEngine.Tilemaps;

public class TileBrush : MonoBehaviour
{
    private Tilemap currentTilemap;

    [SerializeField] private Tile tempTile;

    private void Awake()
    {
        EventManager.OnLayerChanged += LayerChanged;
    }

    private void OnDestroy()
    {
        EventManager.OnLayerChanged -= LayerChanged;
    }

    private void Update()
    {
        if (currentTilemap == null) return;
        if (tempTile == null) return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(InputHandler.controls.Player.MousePosition.ReadValue<Vector2>());

        if (InputHandler.controls.Player.PlaceTile.IsPressed())
        {
            Vector2Int tilePosition = new Vector2Int(Mathf.FloorToInt(mousePosition.x), Mathf.FloorToInt(mousePosition.y));

            currentTilemap.SetTile((Vector3Int)tilePosition, tempTile);
        }
    }

    private void LayerChanged(TileLayer tileLayer)
    {
        currentTilemap = tileLayer.GetTilemap();
    }
}
