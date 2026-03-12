using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Interactable))]
public class InteractableEditor : Editor
{
    private string listenerButtonText = "";
    private string speakerButtonText = "";
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Interactable interactable = (Interactable)target;
        listenerButtonText = interactable.Listener ? "Disconnect Listener" : "Connect Listener";
        speakerButtonText = interactable.Speaker ? "Disconnect Speaker" : "Connect Speaker";
        
        EditorGUILayout.Space(10);

        if (GUILayout.Button(listenerButtonText))
        {
            var serializedInteractable = new SerializedObject(interactable);
            
            if (interactable.Listener)
            {
                DestroyImmediate(interactable.Listener);
                serializedInteractable.FindProperty("_listener").objectReferenceValue = null;
            }
            else
            {
                serializedInteractable.FindProperty("_listener").objectReferenceValue = Undo.AddComponent<Listener>(interactable.gameObject); 
            }

            serializedInteractable.ApplyModifiedProperties();
        }
        
        EditorGUILayout.Space(10);

        if (GUILayout.Button(speakerButtonText))
        {
            var serializedInteractable = new SerializedObject(interactable);

            if (interactable.Speaker)
            {
                DestroyImmediate(interactable.Speaker);
                serializedInteractable.FindProperty("_speaker").objectReferenceValue = null;
            }
            else
            {
                serializedInteractable.FindProperty("_speaker").objectReferenceValue = Undo.AddComponent<Speaker>(interactable.gameObject);
            }
            
            serializedInteractable.ApplyModifiedProperties();
        }
        
        EditorGUILayout.Space(10);
    }
}
