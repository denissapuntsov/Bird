using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    public AudioData currentAudioData;
    public AudioClip CurrentClip => currentAudioData.audioClip;
    public Sprite CurrentAudioSprite => currentAudioData.sprite;

    private AudioSource _audioSource;
    [HideInInspector] public UnityEvent OnAudioEnded;
    
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
        _audioSource.clip = CurrentClip;
        _audioSource.Play();
        UIManager.instance.CreatePopup();
        StartCoroutine(nameof(SpeakCoroutine));
    }

    IEnumerator SpeakCoroutine()
    {
        yield return new WaitUntil(() => !_audioSource.isPlaying);
        Debug.Log("clip ended");
        _audioSource.clip = null;
        UIManager.instance.HidePopup();
        OnAudioEnded.Invoke();
    }
}
