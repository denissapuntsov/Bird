using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileGrid))]
public class TileGridEditor : Editor
{
    private List<TileContainer> _tiles;
    private TileGrid _grid;
    
    private void OnEnable()
    {
        _grid = target as TileGrid;
        if (!_grid) return;

        _tiles = _grid.GetComponentsInChildren<TileContainer>().ToList();
    }

    private string FormattedPosition(Vector3 position)
    {
        return $"{position.x:0.00}\r\n" +
               $"{position.y:0.00}\r\n" +
               $"{position.z:0.00}";
    } 

    private void OnSceneGUI()
    {
        foreach (var tile in _tiles)
        {
            Handles.Label(tile.transform.position, FormattedPosition(tile.transform.localPosition), EditorStyles.boldLabel);
        }
    }
}
