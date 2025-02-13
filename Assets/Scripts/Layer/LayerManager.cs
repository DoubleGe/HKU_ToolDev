using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LayerManager : GenericSingleton<LayerManager>
{
    [Header("Layer Window")]
    [SerializeField] private TileLayer tileLayerButtonPrefab;
    [SerializeField] private Transform tileLayerParent;

    private List<TileLayer> tileButtons;
    private TileLayer selectedLayer;

    [Header("Grid")]
    [SerializeField] private Grid grid;


    protected override void Awake()
    {
        base.Awake();
        tileButtons = new List<TileLayer>();

        if(grid == null) grid = GetComponentInChildren<Grid>();

        SetGridType();
    }

    public void SetGridType()
    {
        grid.cellLayout = GridLayout.CellLayout.Rectangle;
    }

    public void CreateLayer()
    {
        TileLayer tempTileButton = Instantiate(tileLayerButtonPrefab, tileLayerParent);

        Tilemap tileMap = NewTileMap($"New Tilemap ({tileButtons.Count})");

        tempTileButton.InitTileLayerButton(tileMap, $"New Tilemap ({tileButtons.Count})");

        tileButtons.Add(tempTileButton);
        EventManager.OnNewTileLayer?.Invoke(tempTileButton);
    }

    private Tilemap NewTileMap(string name)
    {
        GameObject tilemapObj = new GameObject(name);
        Tilemap tilemap = tilemapObj.AddComponent<Tilemap>();
        tilemapObj.AddComponent<TilemapRenderer>();

        tilemapObj.transform.SetParent(grid.transform);
        return tilemap;
    }
    
    public void RemoveLayer()
    {
        if (selectedLayer == null) return;

        tileButtons.Remove(selectedLayer);
        Destroy(selectedLayer.gameObject);
        selectedLayer = null;
        EventManager.OnTileLayerDeleted?.Invoke();
    }

    public void LayerSelected(TileLayer layer)
    {
        if (selectedLayer == layer) return;

        selectedLayer = layer;

        tileButtons.ForEach(l => l.HighlightButton(layer));

        EventManager.OnLayerChanged?.Invoke(layer);
    }

    public TileLayer GetCurrentLayer() => selectedLayer;
}
