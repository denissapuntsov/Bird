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
            if (interactable.Listener)
            {
                interactable.DisconnectListener();
                return;
            }
            interactable.ConnectListener();
        }
        
        EditorGUILayout.Space(10);

        if (GUILayout.Button(speakerButtonText))
        {
            if (interactable.Speaker)
            {
                interactable.DisconnectSpeaker();
                return;
            }
            interactable.ConnectSpeaker();
        }
    }
}
