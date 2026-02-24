using UnityEngine;
using UnityEngine.Events;

public class Listener : MonoBehaviour
{
    [SerializeField] private UnityEvent OnAcceptKey, OnRejectKey;
    
    [SerializeField] private string keyWord;

    public void ReactToKey()
    {
        if (PlayerInventory.instance.currentVocalization == keyWord)
        {
            Debug.Log("Accepting Key");
            OnAcceptKey.Invoke();
            return;
        }
        OnRejectKey.Invoke();
    }
}
