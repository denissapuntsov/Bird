using UnityEngine;

[CreateAssetMenu(fileName = "SpeakerRhythmInfo", menuName = "Scriptable Objects/SpeakerRhythmInfo")]
public class SpeakerRhythmInfo : ScriptableObject
{
    public AK.Wwise.Event interactionStartEvent, interactionEndEvent, extractedSound;
}
