using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EventManager
{
    //Layer Actions
    public static Action<TileLayer> OnNewTileLayer;
    public static Action OnLayerReorder;
    public static Action OnTileLayerDeleted;

    public static Action<TileLayer> OnLayerSelected;
    public static Action<TileLayer> OnLayerDoubleClicked;

    public static Action<TileLayer> OnLayerSettingsChanged;

    public static Action<TileLayer> OnLayerChanged;

    //Tool
    public static Action OnToolSelected;

    //Tiles
    public static Action OnTileSelected;

    //Project
    public static Action OnProjectLoaded;
    public static Action OnProjectReset;
}
