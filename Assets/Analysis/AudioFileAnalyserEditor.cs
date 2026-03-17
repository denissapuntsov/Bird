using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioFileAnalyser))]
public class AudioFileAnalyserEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        AudioFileAnalyser analyser = (AudioFileAnalyser)target;
        if (GUILayout.Button("Update Markers"))
        {
            analyser.UpdateCues();
        }

        if (GUILayout.Button("Clear All Markers"))
        {
            analyser.RemoveAllCues();
        }

        if (GUILayout.Button("Add Cues From List"))
        {
            analyser.AddCuesFromList();
        }
    }
}
