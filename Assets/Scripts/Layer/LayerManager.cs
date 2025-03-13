using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] private SpriteRenderer gridViewRenderer;
    [SerializeField] private PreviewLayer layerPreview;


    protected override void Awake()
    {
        base.Awake();
        tileButtons = new List<TileLayer>();

        if (grid == null) grid = GetComponentInChildren<Grid>();
    }

    public void SetGridType(GridType gridType)
    {
        switch (gridType)
        {
            case GridType.Rectangle:
                grid.cellLayout = GridLayout.CellLayout.Rectangle;
                layerPreview.SetGridType(GridLayout.CellLayout.Rectangle);
                break;
            case GridType.Hex:
                grid.cellLayout = GridLayout.CellLayout.Hexagon;
                layerPreview.SetGridType(GridLayout.CellLayout.Hexagon);
                break;
            case GridType.Isometric:
                grid.cellLayout = GridLayout.CellLayout.Isometric;
                layerPreview.SetGridType(GridLayout.CellLayout.Isometric);
                break;
        }

        ClearLayers();
        CreateLayer();
    }


    //Unity buttons don't like return types. I don't know why they don't just ignore them...
    public void CreateLayerNon() => CreateLayer();

    public TileLayer CreateLayer()
    {
        TileLayer tempTileButton = Instantiate(tileLayerButtonPrefab, tileLayerParent);

        Tilemap tileMap = NewTileMap($"New Tilemap ({tileButtons.Count})");

        tempTileButton.InitTileLayerButton(tileMap, $"New Tilemap ({tileButtons.Count})");

        tileButtons.Add(tempTileButton);
        EventManager.OnNewTileLayer?.Invoke(tempTileButton);
        LayerSelected(tempTileButton);

        return tempTileButton;
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
        if (RemoveLayer(selectedLayer))
        {
            selectedLayer = null;
            EventManager.OnTileLayerDeleted?.Invoke();
        }
    }

    public bool RemoveLayer(TileLayer tileLayer)
    {
        if (tileLayer == null) return false;

        tileButtons.Remove(tileLayer);
        DestroyImmediate(tileLayer.gameObject);
        return true;
    }

    public void ClearLayers()
    {
        for (int i = tileButtons.Count - 1; i >= 0; i--)
        {
            RemoveLayer(tileButtons[i]);
        }
    }

    public void LayerSelected(TileLayer layer)
    {
        if (selectedLayer == layer) return;

        selectedLayer = layer;

        tileButtons.ForEach(l => l.HighlightButton(layer));

        EventManager.OnLayerChanged?.Invoke(layer);
        gridViewRenderer.sortingOrder = layer.GetSortingOrder() + 1;
    }

    public TileLayer GetCurrentLayer() => selectedLayer;
    public List<TileLayer> GetAllTileLayers() => tileButtons;

    public void LoadLayersFromSave(List<LayerData> layerDatas)
    {
        ClearLayers();

        foreach (LayerData layerData in layerDatas)
        {
            TileLayer newLayer = CreateLayer();
            newLayer.SetLayerIndex(layerData.layerIndex);
            newLayer.LoadTilemapData(layerData.tileMap);
        }
    }
}
