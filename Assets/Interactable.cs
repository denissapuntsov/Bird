using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public SoundListener SoundListener => _soundListener;
    private SoundListener _soundListener;

    private void Awake()
    {
        _soundListener = GetComponent<SoundListener>();
    }
}
