using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputMapManager : MonoBehaviour
{
    private PlayerInput _playerInput;

    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        
        SetCurrentActionMap(ActionMap.UI);
    }

    public void SetCurrentActionMap(ActionMap actionMap)
    {
        
    }
}

public enum ActionMap
{
    Player,
    UI
}
