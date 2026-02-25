using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Event = AK.Wwise.Event;

public class PlayerInteraction : MonoBehaviour
{
    private Interactable _closestInteractable;
    private List<Interactable> _availableInteractables;

    public Interactable ClosestInteractable
    {
        get => _closestInteractable;
        set  => _closestInteractable = value;
    }

    private void Start()
    {
        _availableInteractables = new List<Interactable>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<Interactable>()) return;
        
        var newInteractable = other.GetComponent<Interactable>();
        if (_availableInteractables.Contains(newInteractable)) return;
        _availableInteractables.Add(newInteractable);
    }

    private void OnTriggerStay(Collider other)
    {
        UpdateClosestInteractable();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<Interactable>()) return;
        _availableInteractables.Remove(ClosestInteractable);
    }

    private void UpdateClosestInteractable()
    {
        if (_availableInteractables.Count == 0)
        {
            ClosestInteractable = null;
            return;
        }

        if (!ClosestInteractable)
        {
            ClosestInteractable = _availableInteractables[0];
            return;
        }

        foreach (var interactable in _availableInteractables)
        {
            if (interactable == ClosestInteractable) continue;

            if (Vector3.Distance(
                    transform.position,
                    interactable.transform.position) >=
                Vector3.Distance(
                    transform.position,
                    ClosestInteractable.transform.position)) continue;
            
            ClosestInteractable = interactable;
        }
    }
    
    public void OnCall(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Debug.Log("sadaf");

        PlayerInventory.instance.currentVocalization.Post(gameObject);
        
        ClosestInteractable?.TryCall();
    }

    public void OnListen(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (!ClosestInteractable) 
        {
            Debug.Log("Nothing to listen to");
            return;
        }
        
        ClosestInteractable.TryListen();
    }
}
