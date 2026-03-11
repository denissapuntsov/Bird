using UnityEngine;

[CreateAssetMenu(fileName = "SpeakerDroneInfo", menuName = "Scriptable Objects/SpeakerDroneInfo")]
public class SpeakerDroneInfo : ScriptableObject
{
    public AK.Wwise.Event interactionStartEvent, interactionEndEvent, extractedSound;
}
