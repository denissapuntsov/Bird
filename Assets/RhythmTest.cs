using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RhythmTest : MonoBehaviour
{
    [SerializeField] RhythmData currentRhythmData;

    private AudioSource _audioSource;
    private float _beforeSample = 1f;

    List<Cue> passedCues = new List<Cue>();
    
    [SerializeField] TextMeshProUGUI text;
    

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (currentRhythmData == null) return;
        _audioSource.clip = currentRhythmData.audioClip;
        Invoke(nameof(StartAudio), 1);
    }

    private void StartAudio()
    {
        _audioSource.Play();
    }

    void Update()
    {
        if (currentRhythmData == null) return;
        foreach (Cue cue in currentRhythmData.cuePoints)
        {
            CheckForPosition(cue);
            CheckForLoop();
        }
    }

    private void CheckForPosition(Cue cue)
    {
        if (_audioSource.timeSamples > cue.positionInSamples && !passedCues.Contains(cue))
        {
            var delay = (_audioSource.timeSamples - cue.positionInSamples) / 22050f * 1000;
            text.text = $"Delay: {delay} ms";
            passedCues.Add(cue);
        }
    }

    private void CheckForLoop()
    {
        if (_beforeSample < _audioSource.timeSamples)
        {
            _beforeSample =  _audioSource.timeSamples - 1f;
        }
        else if (_beforeSample > _audioSource.timeSamples)
        {
            passedCues.Clear();
            
            _beforeSample = _audioSource.timeSamples - 1f;
        }
    }
}
