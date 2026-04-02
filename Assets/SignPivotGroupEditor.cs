using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SignPivotGroup))]
public class SignPivotGroupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SignPivotGroup sg = (SignPivotGroup)target;
        
        sg.Direction = (Direction)EditorGUILayout.EnumPopup(sg.Direction);
        sg.IsFlipped = EditorGUILayout.Toggle(sg.IsFlipped);
        serializedObject.ApplyModifiedProperties();
    }
}
