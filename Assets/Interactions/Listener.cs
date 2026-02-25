using UnityEngine;
using UnityEngine.Events;

public class Listener : MonoBehaviour
{
    [SerializeField] private UnityEvent OnAcceptKey, OnRejectKey;
    
    [SerializeField] private AK.Wwise.Event keyEvent;

    public void ReactToKey()
    {
        if (PlayerInventory.instance.currentVocalization.ObjectReference == keyEvent.ObjectReference)
        {
            Debug.Log("Accepting Key");
            OnAcceptKey.Invoke();
            return;
        }
        OnRejectKey.Invoke();                                                                                                                                                                                                                                      
    }
}
