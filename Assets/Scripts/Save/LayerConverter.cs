using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public static class LayerConverter
{
    public static LayerExport ConvertLayer(LayerData layerData, Vector2Int basePosition, int width, int height)
    {
        int[] tileMap = new int[width * height];
        Array.Fill(tileMap, -1); 

        foreach (var tile in layerData.tileMap)
        {
            Vector2Int adjustedPos = tile.Key - basePosition;
            int index = adjustedPos.y * width + adjustedPos.x;
            tileMap[index] = tile.Value;
        }

        return new LayerExport(layerData.layerIndex, tileMap, width, height, layerData.layerName);
    }

    public static List<LayerExport> ConvertAllLayers(List<LayerData> layers)
    {
        if (layers.Count == 0)
            return new List<LayerExport>();

        int minX = layers.SelectMany(l => l.tileMap.Keys).Min(p => p.x);
        int minY = layers.SelectMany(l => l.tileMap.Keys).Min(p => p.y);

        Vector2Int basePosition = new Vector2Int(minX, minY);

        int maxX = layers.SelectMany(l => l.tileMap.Keys).Max(p => p.x);
        int maxY = layers.SelectMany(l => l.tileMap.Keys).Max(p => p.y);
        int width = maxX - minX + 1;
        int height = maxY - minY + 1;

        List<LayerExport> exports = new List<LayerExport>();
        foreach (var layer in layers)
        {
            exports.Add(ConvertLayer(layer, basePosition, width, height));
        }

        return exports;
    }

    public static List<LayerData> ConvertTileButton(List<TileLayer> tileButtons)
    {
        List<LayerData> layers = new List<LayerData>();

        tileButtons.ForEach(l => layers.Add(l.GetLayerData()));
        return layers;
    }
}