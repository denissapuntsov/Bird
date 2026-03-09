using System;
using UnityEngine;

public class Speaker : MonoBehaviour
{
    [SerializeField] private bool isSpeaking = false;
    [SerializeField] private SpeakerInfo speakerInfo;
    public AK.Wwise.Event InteractionStartEvent => speakerInfo.interactionStartEvent;
    public AK.Wwise.Event InteractionEndEvent => speakerInfo.interactionEndEvent;
    public AK.Wwise.Event ExtractedSoundEvent => speakerInfo.extractedSound;

    public void Listen()
    {
        InputMapManager.SetCurrentActionMap(ActionMap.Listening);
        RhythmManager.instance.Setup(this);
    }

    private void Start()
    {
        
    }
}
