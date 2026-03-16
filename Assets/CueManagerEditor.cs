using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CueManager))]
public class CueManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CueManager cueManager = (CueManager)target;
        if (GUILayout.Button("Find Cues"))
        {
            cueManager.GetCues();
        }
    }
}
