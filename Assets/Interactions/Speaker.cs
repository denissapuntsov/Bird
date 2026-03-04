using UnityEngine;

public class Speaker : MonoBehaviour
{
    [SerializeField] private bool isSpeaking = false;
    public AK.Wwise.Event interactionStartEvent, interactionEndEvent, extractedSound;
}
