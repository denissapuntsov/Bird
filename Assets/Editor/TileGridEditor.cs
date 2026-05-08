using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEditor.TerrainTools;

[CustomEditor(typeof(TileGrid))]
public class TileGridEditor : Editor
{
    private TileGrid _grid;
    
    private void OnEnable()
    {
        _grid = target as TileGrid;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(10);
        var scan = GUILayout.Button("Scan");
        if (scan)
        {
            _grid.Scan();
            DisplayPositions();
        }
    }

    private string FormattedPosition(Vector3 position)
    {
        return $"{position.x} / {position.y} / {position.z}";
    } 

    private void OnSceneGUI()
    {
        DisplayPositions();
    }

    private void DisplayPositions()
    {
        foreach (KeyValuePair<Vector3, TileContainer> kvp in _grid.positions)
        {
            Handles.Label(kvp.Value.transform.position, FormattedPosition(kvp.Key), EditorStyles.boldLabel);
        }
    }
}
