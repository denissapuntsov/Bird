/*using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileListener))]
public class TileListenerEditor : Editor
{
    private TileListener _tileListener;
    private QueuePathFollower _parent;
    
    private void OnEnable()
    {
        _tileListener = (TileListener)target;
        _parent = _tileListener.GetComponentInParent<QueuePathFollower>();
    }

    public override void OnInspectorGUI()
    {
        if (_parent)
        {
            EditorGUILayout.LabelField("Tile Listening Events controlled by parent (Queue Path Follower)", EditorStyles.boldLabel);
            _tileListener.enabled = false;
            return;
        }
        _tileListener.enabled = true;
        DrawDefaultInspector();
    }
}*/
