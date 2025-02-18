using SFB;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public enum ImportType
{
    Single = 0,
    Tilemap = 1
}

public class TileImporter : MonoBehaviour
{
    [Header("Tile Importer")]
    [SerializeField] private RectTransform tileImporterWindow;
    [SerializeField] private Image tileViewer;

    [Header("Tilemap")]
    [SerializeField] private GameObject tileMapOptions;
    [SerializeField] private RectTransform importOptions;

    private Texture2D loadedTexture;

    public void OpenFileViewer()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Tile Importer", "", "png", false);

        if (paths.Length == 0) return;

        Texture2D loadedImg = SimpleSave.LoadPNG(paths[0], 2, 2);
        OpenTileImportEditor(loadedImg);
    }

    private void OpenTileImportEditor(Texture2D loadedImage)
    {
        loadedTexture = loadedImage;
        LoadSprite();

        tileImporterWindow.gameObject.SetActive(true);
    }

    private void LoadSprite()
    {
        Sprite viewSprite = Sprite.Create(loadedTexture, new Rect(0, 0, loadedTexture.width, loadedTexture.height), new Vector2(.5f, 5f));
        tileViewer.sprite = viewSprite;
    }

    public void TileImportTypeChanged(int importType)
    {
        tileMapOptions.SetActive(false);
        
        if((ImportType)importType == ImportType.Tilemap)
        {
            tileMapOptions.gameObject.SetActive(true);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(importOptions);
        LayoutRebuilder.ForceRebuildLayoutImmediate(tileImporterWindow);
    }

    public void FilterOptionChanged(int option)
    {
        loadedTexture.filterMode = (FilterMode)option;
    }
    
    public void QualityOptionChanged(int option)
    {
        
    }

    public void CloseImporter()
    {
        tileImporterWindow.gameObject.SetActive(false);
    }

    public void ImportTile()
    {
        CustomTileBase tile = (CustomTileBase)ScriptableObject.CreateInstance(typeof(CustomTileBase));
        tile.sprite = Sprite.Create(loadedTexture, new Rect(0, 0, loadedTexture.width, loadedTexture.height), new Vector2(0, 0), 16);

        TileGroup.Instance.AddTileButton(tile);

        tileImporterWindow.gameObject.SetActive(false);
    }
}
