using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public AudioClip currentClip;
    private AudioSource _audioSource;
    
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
        _audioSource.PlayOneShot(currentClip);
    }
}
