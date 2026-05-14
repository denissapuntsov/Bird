using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    private PlayerInventory _inventory;
    private MovingCharacter _character;

    private void Awake()
    {
        _inventory = GetComponentInChildren<PlayerInventory>();
        _character = GetComponent<MovingCharacter>();
    }

    public void OnSpeak(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!_inventory.currentSoundItem) return;
        foreach (var neighbor in AStar.instance.GetNeighborsInRange(_character.OccupiedTile, _inventory.Range))
        {
            if (neighbor.Owner && neighbor.Owner != _character)
            {
                var listener = neighbor.Owner.gameObject.GetComponent<SoundListener>();
                listener?.TryKey(_inventory.currentSoundItem);
            }
        }
    }
    
    public void OnListen(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        Debug.Log("Listening");
        
    }
}
