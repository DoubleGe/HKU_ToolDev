using TMPro;
using UnityEngine;

public class TileLayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tileMapName;
    [SerializeField] private TextMeshProUGUI tempVisualText;

    private GameObject connectedTilemap;

    private bool isVisible = true;

    public void InitTileLayerButton(GameObject tileMap, string name)
    {
        tileMapName.text = name;
        connectedTilemap = tileMap;
    }

    public void ChangeVisibility()
    {
        isVisible = !isVisible;

        connectedTilemap.SetActive(isVisible);

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
}
