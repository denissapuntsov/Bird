using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Speaker))]
public class SpeakerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Speaker speaker = (Speaker)target;
        if (!speaker) return;
        
        speaker.playOnAwake = EditorGUILayout.Toggle("Plays on Awake?", speaker.playOnAwake);
    }
}
