using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioFileAnalyser))]
public class AudioFileAnalyserEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        AudioFileAnalyser analyser = (AudioFileAnalyser)target;
        if (GUILayout.Button("Find Markers"))
        {
            if (analyser.audioClip != null)
            {
                analyser.Analyse();
            }
        }

        if (GUILayout.Button("Clear Markers"))
        {
            if (analyser.audioClip != null)
            {
                analyser.ClearMarkers();
            }
        }
    }
}
