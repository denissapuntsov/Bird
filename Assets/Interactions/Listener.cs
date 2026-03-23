using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

public class Listener : MonoBehaviour
{
    [SerializeField] private UnityEvent OnAcceptKey, OnRejectKey;
    
    [SerializeField] private AudioClip keyClip;

    public void ReactToKey()
    {
        if (PlayerInventory.instance.currentClip == keyClip)
        {
            Debug.Log("Accepting Key");
            OnAcceptKey.Invoke();
            return;
        }
        OnRejectKey.Invoke();                                                                                                                                                                                                                                      
    }
}
