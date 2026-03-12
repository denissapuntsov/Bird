using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Event = AK.Wwise.Event;

public class PlayerInteraction : MonoBehaviour
{
    private Interactable _closestInteractable, _closestSpeaker, _closestListener;
    private List<Interactable> _availableInteractables, _availableSpeakers, _availableListeners;

    private Interactable ClosestInteractable
    {
        get => _closestInteractable;
        set => _closestInteractable = value;
    }
    
    private Interactable ClosestSpeaker
    {
        get => _closestSpeaker;
        set => _closestSpeaker = value;
    }

    private Interactable ClosestListener
    {
        get => _closestListener;
        set => _closestListener = value;
    }

    private void Start()
    {
        _availableInteractables = new List<Interactable>();
        _availableSpeakers = new List<Interactable>();
        _availableListeners = new List<Interactable>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<Interactable>()) return;
        
        var newInteractable = other.GetComponent<Interactable>();

        if (!_availableInteractables.Contains(newInteractable))
        {
            _availableInteractables.Add(newInteractable);
        }

        if (newInteractable.Speaker && !_availableSpeakers.Contains(newInteractable))
        {
            _availableSpeakers.Add(newInteractable);
        }

        if (newInteractable.Listener&& !_availableListeners.Contains(newInteractable))
        {
            _availableListeners.Add(newInteractable);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        ClosestInteractable = UpdateClosest(_availableInteractables, _closestInteractable);
        ClosestSpeaker = UpdateClosest(_availableSpeakers, _closestSpeaker);
        ClosestListener = UpdateClosest(_availableListeners, _closestListener);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<Interactable>()) return;
        var exitingInteractable =  other.GetComponent<Interactable>();
        
        _availableListeners.Remove(exitingInteractable);
        _availableSpeakers.Remove(exitingInteractable);
    }

    private Interactable UpdateClosest(List<Interactable> group, Interactable closestInGroupField)
    {
        if (group.Count == 0)
        { 
            return null;
        }

        if (!closestInGroupField || group.Count == 1)
        {
            return group[0];
        }

        foreach (var interactable in group)
        {
            if (interactable == closestInGroupField) continue;

            if (Vector3.Distance(
                    transform.position,
                    interactable.transform.position) >=
                Vector3.Distance(
                    transform.position,
                    closestInGroupField.transform.position)) continue;
            
            return interactable;
        }

        return null;
    }
    
    public void OnCall(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        PlayerInventory.instance.currentVocalization.Post(gameObject);
        
        ClosestListener?.TryCall();
    }

    public void OnListen(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        ClosestSpeaker?.TryListen();
    }
}
