using System;
using UnityEngine;
using UnityEngine.Events;

public class TriggerTile : MonoBehaviour
{
    private TileContainer _tileContainer;    
    
    public UnityEvent<MovingCharacter> OnTileEnter;
    public UnityEvent<MovingCharacter> OnTileExit;

    private void Awake()
    {
        _tileContainer = GetComponent<TileContainer>();
    }

    private void OnTileOwnerChange(MovingCharacter newOwner)
    {
        if (!newOwner)
        {
            OnTileExit?.Invoke(newOwner);
        }
        else
        {
            OnTileEnter?.Invoke(newOwner);
        }
    }
}
