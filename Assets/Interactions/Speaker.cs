using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;

public class Speaker : MonoBehaviour
{
    [SerializeField] private bool isSpeaking = false;
    [HideInInspector] public SpeakerType speakerType;
    [HideInInspector] public SpeakerRhythmInfo speakerRhythmInfo;
    [HideInInspector] public SpeakerDroneInfo speakerDroneInfo;
    public AK.Wwise.Event InteractionStartEvent => speakerRhythmInfo.interactionStartEvent;
    public AK.Wwise.Event InteractionEndEvent => speakerRhythmInfo.interactionEndEvent;
    public AK.Wwise.Event ExtractedSoundEvent => speakerRhythmInfo.extractedSound;

    public void Listen()
    {
        InputMapManager.SetCurrentActionMap(ActionMap.Listening);
        switch (speakerType)
        {
            case SpeakerType.Rhythm:
                RhythmManager.instance.Setup(this);
                break;
            case SpeakerType.Drone:
                DroneManager.instance.Setup(this);
                break;
        }
    }

   
}

public enum SpeakerType
{
    Rhythm,
    Drone
}
