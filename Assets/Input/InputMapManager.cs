using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputMapManager : MonoBehaviour
{
    private PlayerInput _playerInput;

    private static InputSystem_Actions _actions;

    private void Start()
    {
        _actions = new InputSystem_Actions();
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.actions = _actions.asset;
        
        SetCurrentActionMap(ActionMap.Player);
    }

    public static void SetCurrentActionMap(ActionMap actionMap)
    {
        ClearActionMaps();

        string enumMapName = Enum.GetName(typeof(ActionMap), actionMap);
        foreach (var map in _actions.asset.actionMaps)
        {
            if (map.name == enumMapName)
            {
                Debug.Log("Found ActionMap: " + map.name);
                map.Enable();
                return;
            }
        }

        Debug.LogWarning($"ActionMap not found: {enumMapName}. Check if ActionMap is present in InputSystem_Actions.asset");
}

    private static void ClearActionMaps()
    {
        foreach (var map in _actions.asset.actionMaps) 
        {
            map.Disable();
        }
    }
}

public enum ActionMap
{
    Player,
    UI,
    Listening,
}
