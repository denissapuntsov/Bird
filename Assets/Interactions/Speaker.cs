using UnityEngine;

public class Speaker : MonoBehaviour
{
    [SerializeField] private bool isSpeaking = false;
    [HideInInspector] public SpeakerType speakerType;
    [HideInInspector] public RhythmData rhythmData;
    [HideInInspector] public AudioData audioData;
    [HideInInspector] public SpeakerDroneInfo speakerDroneInfo;

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
