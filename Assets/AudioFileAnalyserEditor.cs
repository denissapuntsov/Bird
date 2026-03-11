using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioFileAnalyser))]
public class AudioFileAnalyserEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        AudioFileAnalyser analyser = (AudioFileAnalyser)target;
        if (GUILayout.Button("Analyse"))
        {
            analyser.Analyse();
        }

        else if (GUI.changed)
        {
            if (analyser.audioClip != null)
            {
                analyser.Analyse();
            }
        }
    }
}
