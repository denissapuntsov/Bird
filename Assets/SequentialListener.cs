using UnityEngine;

public class SequentialListener : MonoBehaviour
{
    [SerializeField] private AudioClip keyPlayerClip;
    private AudioSource _playerAudioSource;

    public void SubscribeToPlayerAudio()
    {
        PlayerInventory.instance.OnAudioEnded.AddListener(React);
    }

    public void UnsubscribeFromPlayerAudio()
    {
        PlayerInventory.instance.OnAudioEnded.RemoveListener(React);
    }

    private void React()
    {
        Debug.Log("Reacted to audio");
    }
}
