using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

public class Listener : MonoBehaviour
{
    [SerializeField] private UnityEvent OnAcceptKey, OnRejectKey;
    
    [SerializeField] private AudioClip keyClip;

    public void SubscribeToPlayerAudio()
    {
        PlayerInventory.instance.OnAudioEnded.AddListener(ReactToKey);
    }

    public void UnsubscribeFromPlayerAudio()
    {
        PlayerInventory.instance.OnAudioEnded.RemoveListener(ReactToKey);
    }
    
    private void ReactToKey()
    {
        Debug.Log($"{gameObject.name} Reacting to sound");
        if (PlayerInventory.instance.currentClip == keyClip)
        {
            Debug.Log("Accepting Key");
            OnAcceptKey.Invoke();
            return;
        }
        OnRejectKey.Invoke();                                                                                                                                                                                                                                      
    }
}
