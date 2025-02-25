using SimpleSaver.Intern;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TileLayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tileMapName;
    [SerializeField] private TextMeshProUGUI tempVisualText;

    [Header("Selected")]
    [SerializeField] private Button layerButton;
    [SerializeField] private Color32 highlightColor;
    [SerializeField] private Color32 normalColor;

    [Header("Tilemap Move")]
    [SerializeField] private GameObject moveButtons;
    [SerializeField] private Button upButton, downButton;

    //TileMap Data
    private Tilemap connectedTilemap;
    private TilemapRenderer tilemapRenderer;
    private SerializableDictionary<Vector2Int, int> tileMapData;

    private bool isVisible = true;

    private Action<TileLayer> layerHandler;

    private void Awake()
    {
        layerHandler = (TileLayer l) => UpdateLayerIndex();

        EventManager.OnNewTileLayer += layerHandler;
        EventManager.OnTileLayerDeleted += UpdateLayerIndex;
        EventManager.OnLayerReorder += UpdateLayerIndex;

        tileMapData = new SerializableDictionary<Vector2Int, int>();
    }

    private void OnDestroy()
    {
        EventManager.OnNewTileLayer -= layerHandler;
        EventManager.OnTileLayerDeleted -= UpdateLayerIndex;
        EventManager.OnLayerReorder -= UpdateLayerIndex;

        if(connectedTilemap != null) Destroy(connectedTilemap.gameObject);
    }

    public void InitTileLayerButton(Tilemap tileMap, string name)
    {
        tileMapName.text = name;
        connectedTilemap = tileMap;
        tilemapRenderer = tileMap.GetComponent<TilemapRenderer>();

        UpdateLayerIndex();
    }

    public void ChangeVisibility()
    {
        isVisible = !isVisible;

        connectedTilemap.gameObject.SetActive(isVisible);

        tempVisualText.text = isVisible ? "V" : "O";
    }

    public void LayerButtonClicked()
    {
        LayerManager.Instance.LayerSelected(this);
    }

    public void HighlightButton(TileLayer layer)
    {
        if(layer == this) layerButton.image.color = highlightColor;
        else layerButton.image.color = normalColor;
        moveButtons.SetActive(layer == this);

        upButton.interactable = transform.GetSiblingIndex() != 0;
        downButton.interactable = transform.GetSiblingIndex() != transform.parent.childCount -1;
    }

    public void MoveButton(bool moveUp)
    {
        int childIndex = transform.GetSiblingIndex();

        if (moveUp)
        {
            transform.SetSiblingIndex(childIndex - 1);
            connectedTilemap.transform.SetSiblingIndex(childIndex - 1);
        }
        else
        {
            transform.SetSiblingIndex(childIndex + 1);
            connectedTilemap.transform.SetSiblingIndex(childIndex + 1);
        }

        EventManager.OnLayerReorder?.Invoke();
        upButton.interactable = transform.GetSiblingIndex() != 0;
        downButton.interactable = transform.GetSiblingIndex() != transform.parent.childCount - 1;
    }

    private void UpdateLayerIndex()
    {
        tilemapRenderer.sortingOrder = (transform.parent.childCount - transform.GetSiblingIndex()) * 2;
    }

    public void SetTile(Vector2Int position, CustomTileData tiledata)
    {
        connectedTilemap.SetTile((Vector3Int)position, tiledata.tile);

        if (tileMapData.ContainsKey(position))
        {
            if(tiledata == null) tileMapData.Remove(position);
            else tileMapData[position] = tiledata.tileID;
            
        }
        else if(tiledata != null) tileMapData.Add(position, tiledata.tileID);
    }

    public Tilemap GetTilemap() => connectedTilemap;
    public int GetSortingOrder() => tilemapRenderer.sortingOrder;
}
