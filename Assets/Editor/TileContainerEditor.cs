using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileContainer)), CanEditMultipleObjects]
public class TileContainerEditor : Editor
{
    private TileContainer _tileContainer;
    
    private void OnEnable()
    {
        _tileContainer = (TileContainer)target;
        _tileContainer.gameObject.name = $"[{_tileContainer.GridPosition.x:N0}, {_tileContainer.GridPosition.y:N0}, {_tileContainer.GridPosition.z:N0}]";
    }

    public override void OnInspectorGUI()
    {
        _tileContainer = (TileContainer)target;
        
        EditorGUI.BeginChangeCheck();
        var tile = EditorGUILayout.ObjectField(_tileContainer.tile, typeof(Tile), true) as Tile;
        if (EditorGUI.EndChangeCheck()) 
        {
            _tileContainer.tile = tile;
            Undo.SetCurrentGroupName("Change Tile Type to " + (_tileContainer.tile ? _tileContainer.tile.name : "None"));
            int group = Undo.GetCurrentGroup();
            
            Undo.RecordObject(target, "Change Tile Type");
            
            Undo.RecordObject(_tileContainer, "Change Tile Type");
            
            if (_tileContainer.tileChild)
            {
                Undo.DestroyObjectImmediate(_tileContainer.tileChild);
            }
            if (tile && tile.prefab)
            {
                _tileContainer.tileChild = Instantiate(tile.prefab, _tileContainer.transform);
                _tileContainer.tileChild.name = _tileContainer.tile.prefab.name;
                _tileContainer.tileChild.transform.SetAsFirstSibling();
                Undo.RegisterCreatedObjectUndo(_tileContainer.tileChild, $"Create {_tileContainer.tileType} tile");
            }
            Undo.CollapseUndoOperations(group);
        }
    }
}
