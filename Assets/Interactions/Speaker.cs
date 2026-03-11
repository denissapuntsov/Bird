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
        SpeakerManager.instance.Setup(this);
    }
}

public enum SpeakerType
{
    Rhythm,
    Drone
}
