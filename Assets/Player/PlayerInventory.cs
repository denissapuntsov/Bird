using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    public AudioClip currentClip;
    private AudioSource _audioSource;
    public UnityEvent OnAudioEnded;
    
    public static PlayerInventory instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        
        _audioSource = GetComponent<AudioSource>();
    }

    public void Speak()
    {
        if (_audioSource.isPlaying) return;
        _audioSource.clip = currentClip;
        _audioSource.Play();
        StartCoroutine(nameof(SpeakCoroutine));
    }

    IEnumerator SpeakCoroutine()
    {
        yield return new WaitUntil(() => !_audioSource.isPlaying);
        Debug.Log("clip ended");
        _audioSource.clip = null;
        OnAudioEnded.Invoke();
    }
}
