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

    private Tilemap connectedTilemap;
    private TilemapRenderer tilemapRenderer;

    private bool isVisible = true;

    public void InitTileLayerButton(Tilemap tileMap, string name)
    {
        tileMapName.text = name;
        connectedTilemap = tileMap;
        tilemapRenderer = tileMap.GetComponent<TilemapRenderer>();

        tilemapRenderer.sortingOrder = transform.parent.childCount - transform.GetSiblingIndex();
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

    private void OnDestroy()
    {
        Destroy(connectedTilemap);
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
            tilemapRenderer.sortingOrder = transform.parent.childCount - (childIndex - 1);
        }
        else
        {
            transform.SetSiblingIndex(childIndex + 1);
            connectedTilemap.transform.SetSiblingIndex(childIndex + 1);
            tilemapRenderer.sortingOrder = transform.parent.childCount - (childIndex + 1);
        }

        upButton.interactable = transform.GetSiblingIndex() != 0;
        downButton.interactable = transform.GetSiblingIndex() != transform.parent.childCount - 1;
    }
}
