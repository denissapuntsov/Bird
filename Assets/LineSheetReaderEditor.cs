using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LineSheetReader))]
public class LineSheetReaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        LineSheetReader reader = (LineSheetReader)target;
        
        GUILayout.Space(10);
        if (GUILayout.Button("Update"))
        {
            reader.UpdateValues();
        }
    }
}
