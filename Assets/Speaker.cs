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

    [HideInInspector] public bool playOnAwake = false;
    public UnityEvent onFinishSpeaking;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (!playOnAwake) return;
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
        Debug.Log("finished speaking");
        _isSpeaking = false;
    }
}
