using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileGroup : GenericSingleton<TileGroup> 
{
    private List<TileButton> tileButtons;
    [SerializeField] private TileButton tileButtonPrefab;
    [SerializeField] private Transform tileButtonParent;

    private void Start()
    {
        tileButtons = new List<TileButton>();
    }

    public void AddTileButton(CustomTileBase tileBase, Texture2D texture)
    {
        TileButton tempTileBtn = Instantiate(tileButtonPrefab, tileButtonParent);
        tempTileBtn.InitTileButton(tileBase, texture);
        tileButtons.Add(tempTileBtn);
    }

    public void RemoveAllTileButtons()
    {
        for (int i = tileButtons.Count - 1; i >= 0; i--)
        {
            TileButton tileButton = tileButtons[i];

            tileButtons.Remove(tileButton);
            DestroyImmediate(tileButton.gameObject);
        }
    }

    public List<TileButton> GetAllTiles() => tileButtons;

    public void SetLoadedTiles(List<ResourceReturn> loadedTiles)
    {
        RemoveAllTileButtons();

        loadedTiles = loadedTiles.OrderBy(t => t.tileID).ToList();

        foreach (ResourceReturn rr in loadedTiles)
        {
            CustomTileBase tile = (CustomTileBase)ScriptableObject.CreateInstance(typeof(CustomTileBase));
            tile.sprite = Sprite.Create(rr.texture, new Rect(0, 0, rr.texture.width, rr.texture.height), new Vector2(0, 0), Mathf.Max(rr.texture.height, rr.texture.width));

            AddTileButton(tile, rr.texture);
        }
    }

    public void SetTile(CustomTileData tileData)
    {
        TilePainter.Instance.TileUpdate(tileData);
    }

    public CustomTileData GetTileWithID(int id) => tileButtons.FirstOrDefault(t => t.GetCustomTile().tileID == id).GetCustomTile();
}
