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

        GameObject tileMap = NewTileMap($"New Tilemap ({tileButtons.Count})");

        tempTileButton.InitTileLayerButton(tileMap, $"New Tilemap ({tileButtons.Count})");

        tileButtons.Add(tempTileButton);
    }

    private GameObject NewTileMap(string name)
    {
        GameObject tilemapObj = new GameObject(name);
        tilemapObj.AddComponent<Tilemap>();
        tilemapObj.AddComponent<TilemapRenderer>();

        tilemapObj.transform.SetParent(grid.transform);
        return tilemapObj;
    }
    
    public void RemoveLayer()
    {
        if (selectedLayer == null) return;

        tileButtons.Remove(selectedLayer);
        Destroy(selectedLayer.gameObject);
        selectedLayer = null;
    }

    public void LayerSelected(TileLayer layer)
    {
        selectedLayer = layer;
    }
}
