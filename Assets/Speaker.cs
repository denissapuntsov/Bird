using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class Speaker : MonoBehaviour
{
    [SerializeField] private SoundItem currentSoundItem;
    public SoundItem CurrentSoundItem => currentSoundItem;
    private bool _isSpeaking;
    private AudioSource _audioSource;

    public UnityEvent onFinishSpeaking;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (CurrentSoundItem == null) return;
        Speak();
    }

    public void Speak()
    {
        _audioSource.clip = CurrentSoundItem.clip;
        _audioSource.Play();
        _isSpeaking = true;
        StartCoroutine(Test(_audioSource));
    }
    
    private IEnumerator Test(AudioSource source)
    {
        var waitForClipRemainingTime = new WaitForSeconds(source.GetClipRemainingTime());
        yield return waitForClipRemainingTime;
        onFinishSpeaking?.Invoke();
        _isSpeaking = false;
    }
}
