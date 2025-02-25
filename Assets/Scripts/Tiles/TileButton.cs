using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TileButton : MonoBehaviour
{
    [SerializeField] private Image tileImg;

    private CustomTileData tileData;

    public void InitTileButton(CustomTileBase tile)
    {
        tileData = new CustomTileData(tile, transform.GetSiblingIndex());
        tileImg.sprite = tile.sprite;
    }

    public void TileButtonClicked()
    {
        TilePainter.Instance.TileUpdate(tileData);
    }
}

public class CustomTileData
{
    public int tileID;
    public TileBase tile;

    public CustomTileData(TileBase tile, int tileID)
    {
        this.tile = tile;
        this.tileID = tileID;
    }
}
