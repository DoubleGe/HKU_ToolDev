using System.Collections.Generic;
using UnityEngine;

public class TileGroup : GenericSingleton<TileGroup> 
{
    private List<TileButton> tileButtons;
    [SerializeField] private TileButton tileButtonPrefab;
    [SerializeField] private Transform tileButtonParent;

    private void Start()
    {
        tileButtons = new List<TileButton>();
    }

    public void AddTileButton(CustomTileBase tileBase)
    {
        TileButton tempTileBtn = Instantiate(tileButtonPrefab, tileButtonParent);
        tempTileBtn.InitTileButton(tileBase);
        tileButtons.Add(tempTileBtn);
    }

    public void RemoveAllTileButtons()
    {
        for (int i = tileButtons.Count - 1; i >= 0; i--)
        {
            TileButton tileButton = tileButtons[i];

            tileButtons.Remove(tileButton);
            Destroy(tileButton.gameObject);
        }
    }
}
