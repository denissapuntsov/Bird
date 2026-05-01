using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Tile))]
public class TileEditor : Editor
{
    public GameObject groundPrefab, waterPrefab, grassPrefab;
    private Tile _tile;
    
    private void OnEnable()
    {
        _tile = (Tile)target;
        _tile.gameObject.name = "Tile";
    }

    public override void OnInspectorGUI()
    {
        _tile = (Tile)target;
        
        EditorGUI.BeginChangeCheck();
        var tileType = (TileType)EditorGUILayout.EnumPopup("Tile Type", _tile.tileType);
        if (EditorGUI.EndChangeCheck()) 
        {
            Undo.SetCurrentGroupName("Change Tile Type to  " + _tile.tileType);
            int group = Undo.GetCurrentGroup();
            
            Undo.RecordObject(target, "Change Tile Type");
            _tile.tileType = tileType;
            GameObject prefab = _tile.tileType switch
            {
                TileType.Ground => groundPrefab,
                TileType.Grass => grassPrefab,
                TileType.Water => waterPrefab,                                                             
                _ => null
            }; 
            
            
            Undo.RecordObject(_tile, "Change Tile Type");
            
            if (_tile.tileChild)
            {
                Undo.DestroyObjectImmediate(_tile.tileChild);
            }
            if (prefab)
            {
                _tile.tileChild = Instantiate(prefab, _tile.transform);
                _tile.tileChild.name = _tile.tileType.ToString();
                _tile.tileChild.transform.SetAsFirstSibling();
                Undo.RegisterCreatedObjectUndo(_tile.tileChild, $"Create {_tile.tileType} tile");
            }
            Undo.CollapseUndoOperations(group);
        }
    }
}
