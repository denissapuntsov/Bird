using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public AudioListener AudioListener => _audioListener;
    private AudioListener _audioListener;

    private void Awake()
    {
        _audioListener = GetComponent<AudioListener>();
    }
}
