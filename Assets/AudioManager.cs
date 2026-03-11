using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private SpeakerType _speakerType;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void Setup(Speaker newSpeaker)
    {
        _speakerType = newSpeaker.speakerType;
    }

    public void OnExit(InputAction.CallbackContext context)
    {
        
    }
}
