using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

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
                interactable.DisconnectListenerEvent();
            }
            else
            {
                Listener newListener =
                    interactable.GetComponent<Listener>()
                        ? interactable.GetComponent<Listener>()
                        : interactable.AddComponent<Listener>();
                
                serializedInteractable.FindProperty("_listener").objectReferenceValue = newListener;
                serializedInteractable.ApplyModifiedProperties();
                interactable.ConnectListenerEvent();
            }
        }
        
        EditorGUILayout.Space(10);

        if (GUILayout.Button(speakerButtonText))
        {
            var serializedInteractable = new SerializedObject(interactable);

            if (interactable.Speaker)
            {
                DestroyImmediate(interactable.Speaker);
                interactable.DisconnectSpeakerEvent();
            }
            else
            {
                Speaker newSpeaker = 
                    interactable.GetComponent<Speaker>() 
                        ? interactable.GetComponent<Speaker>() 
                        : interactable.AddComponent<Speaker>();
                
                serializedInteractable.FindProperty("_speaker").objectReferenceValue = newSpeaker;
                serializedInteractable.ApplyModifiedProperties();
                interactable.ConnectSpeakerEvent();
            }
        }
        
        EditorGUILayout.Space(10);
    }
}
