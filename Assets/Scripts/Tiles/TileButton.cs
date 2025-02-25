using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TileButton : MonoBehaviour
{
    [SerializeField] private Image tileImg;

    private CustomTileData tileData;

    public void InitTileButton(CustomTileBase tile, Texture2D texture)
    {
        tileData = new CustomTileData(tile, texture, transform.GetSiblingIndex());
        tileImg.sprite = tile.sprite;
    }

    public void TileButtonClicked()
    {
        TilePainter.Instance.TileUpdate(tileData);
    }

    public CustomTileData GetCustomTile() => tileData;
}

public class CustomTileData
{
    public int tileID;
    public TileBase tile;
    public Texture2D texture;

    public CustomTileData(TileBase tile, Texture2D texture, int tileID)
    {
        this.tile = tile;
        this.texture = texture;
        this.tileID = tileID;
    }
}
