using UnityEngine;
using UnityEngine.UI;

public class TileButton : MonoBehaviour
{
    private CustomTileBase tile;

    [SerializeField] private Image tileImg;
    private int tileID;

    public void InitTileButton(CustomTileBase tile)
    {
        this.tile = tile;
        tileImg.sprite = tile.sprite;
        tileID = transform.GetSiblingIndex();
    }

    public void TileButtonClicked()
    {
        //Ref tile setter
    }
}
