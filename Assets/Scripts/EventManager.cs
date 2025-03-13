using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EventManager
{
    public static Action<TileLayer> OnNewTileLayer;
    public static Action OnLayerReorder;
    public static Action OnTileLayerDeleted;

    public static Action<TileLayer> OnLayerChanged;

    public static Action OnProjectReset;
}
