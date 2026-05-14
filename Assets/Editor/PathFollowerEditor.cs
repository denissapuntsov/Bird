using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathFollower))]
public class PathFollowerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PathFollower pathFollower = (PathFollower)target;
        if (!pathFollower) return;
        pathFollower.moveOnStart = EditorGUILayout.Toggle("Move on Start?", pathFollower.moveOnStart);
        if (pathFollower.moveOnStart)
        {
            pathFollower.startPathIndex = EditorGUILayout.IntField("Start Path Index", pathFollower.startPathIndex);
        }
    }
}
