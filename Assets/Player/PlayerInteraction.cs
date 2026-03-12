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
        /*if (_availableInteractables.Contains(newInteractable)) return;
        _availableInteractables.Add(newInteractable);*/

        if (newInteractable.Speaker && !_availableSpeakers.Contains(newInteractable))
        {
            Debug.Log(newInteractable.Speaker);
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

    /*private void UpdateClosestInteractable()
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

    private void UpdateClosestSpeaker()
    {
        if (_availableSpeakers.Count == 0)
        {
            ClosestSpeaker = null;
            return;
        }

        if (!ClosestSpeaker)
        {
            ClosestSpeaker = _availableSpeakers[0];
            return;
        }

        foreach (var speaker in _availableSpeakers)
        {
            if (speaker == ClosestSpeaker) continue;

            if (Vector3.Distance(
                    transform.position,
                    speaker.transform.position) >=
                Vector3.Distance(
                    transform.position,
                    ClosestSpeaker.transform.position)) continue;
            
            ClosestSpeaker = speaker;
        }
    }

    private void UpdateClosestListener()
    {
        
    }*/
    
    public void OnCall(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        PlayerInventory.instance.currentVocalization.Post(gameObject);
        
        ClosestListener?.TryCall();
    }

    public void OnListen(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (!ClosestInteractable) 
        {
            Debug.Log("Nothing to listen to");
            return;
        }
        
        ClosestSpeaker.TryListen();
    }
}
