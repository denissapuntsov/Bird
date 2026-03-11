using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class DroneCursor : MonoBehaviour
{
    public UnityEvent<Collider2D> OnTriggerEnterEvent, OnTriggerExitEvent;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        OnTriggerEnterEvent?.Invoke(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        OnTriggerExitEvent?.Invoke(other);
    }
}
