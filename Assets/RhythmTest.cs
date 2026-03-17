using System;
using UnityEngine;

public class RhythmTest : MonoBehaviour
{
    [SerializeField] RhythmData currentRhythmData;

    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (currentRhythmData == null) return;
        _audioSource.clip = currentRhythmData.audioClip;
        _audioSource.Play();
    }

    void Update()
    {
        if (currentRhythmData == null) return;
        foreach (Cue cue in currentRhythmData.cuePoints)
        {
            CheckForPosition(cue);
        }
    }

    private void CheckForPosition(Cue cue)
    {
        if (_audioSource.timeSamples == cue.positionInSamples)
        {
            Debug.Log(cue.positionInSamples);
        }
    }
}
