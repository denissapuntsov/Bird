using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Speaker))]
public class SpeakerEditor : Editor
{
    private string buttonText;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Speaker speaker = (Speaker)target;

        buttonText = speaker.speakerType == SpeakerType.Rhythm
            ? "Rhythm (press to switch to Drone)"
            : "Drone (press to switch to Rhythm)";


        GUILayout.Space(10);

        if (GUILayout.Button(buttonText))
        {
            speaker.speakerType = speaker.speakerType == SpeakerType.Rhythm ? SpeakerType.Drone : SpeakerType.Rhythm;
        }

        GUILayout.Space(10);

        if (speaker.speakerType == SpeakerType.Rhythm)
        {
            speaker.speakerRhythmInfo =
                EditorGUILayout.ObjectField(speaker.speakerRhythmInfo, typeof(SpeakerRhythmInfo), true) as
                    SpeakerRhythmInfo;
        }
        else
        {
            speaker.speakerDroneInfo = EditorGUILayout.ObjectField(speaker.speakerDroneInfo, typeof(SpeakerDroneInfo), true) as
                SpeakerDroneInfo;
        }

    GUILayout.Space(10);
    }
}
