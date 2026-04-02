using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SignCreator))]
public class SignCreatorEditor : Editor
{
    [SerializeField] private GameObject signPivotGroupPrefab;
    private const float NEW_ROW_OFFSET = -3.8f;
    
    public override void OnInspectorGUI()
    {
        SignCreator creator = (SignCreator)target;
        var so = new  SerializedObject(creator);
        SerializedProperty materialsProperty = so.FindProperty("materials");
        EditorGUILayout.PropertyField(materialsProperty, new GUIContent("Materials"));
        so.ApplyModifiedProperties();
        
        var materials = creator.materials;
        
        var groups = creator.groups;
        
        GUI.enabled = groups.Count < 4;
        if (GUILayout.Button("Add Sign"))
        {
            GameObject group = Instantiate(signPivotGroupPrefab, creator.transform);
            
            groups.Add(group);

            group.transform.position = new Vector3(
                creator.transform.position.x,
                creator.transform.position.y + NEW_ROW_OFFSET * groups.IndexOf(group) * creator.transform.lossyScale.y,
                creator.transform.position.z);

            if (materials.Count <= 0) return;
            group.GetComponent<SignPivotGroup>().Material = materials[Random.Range(0, materials.Count)];
        }

        GUI.enabled = true;

        GUI.enabled = groups.Count > 0;
        if (GUILayout.Button("Remove Last Sign"))
        {
            var last = groups[^1];
            groups.Remove(last);
            DestroyImmediate(last);
        }
        GUI.enabled = true;

        foreach (GameObject gameObject in groups)
        {
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginVertical("box");
            
            EditorGUILayout.LabelField($"Sign {groups.IndexOf(gameObject)}", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            var group = gameObject.GetComponent<SignPivotGroup>();
            group.Direction = (Direction)EditorGUILayout.EnumPopup("Direction", group.Direction);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Icon Sprite");
            group.icon = (Sprite)EditorGUILayout.ObjectField(group.icon, typeof(Sprite), false);
            EditorGUILayout.EndHorizontal();
            group.IsFlipped = EditorGUILayout.Toggle("Is Flipped?", group.IsFlipped);
            
            EditorGUILayout.EndVertical();
            
        }
    }
}
