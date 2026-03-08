using UnityEngine;

[CreateAssetMenu(fileName = "SpeakerInfo", menuName = "Scriptable Objects/SpeakerInfo")]
public class SpeakerInfo : ScriptableObject
{
    public AK.Wwise.Event interactionStartEvent, interactionEndEvent, extractedSound;
}
