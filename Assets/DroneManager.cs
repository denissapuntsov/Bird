using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DroneManager : MonoBehaviour
{
    [SerializeField] private GameObject cursor;
    private Speaker _currentSpeaker;
    private Vector2 _input;
    
    public static DroneManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void Setup(Speaker newSpeaker)
    {
        _currentSpeaker = newSpeaker;
        UIManager.instance.ActiveUI = UIMode.ListeningDrone;
    }
    
    public void OnExit(InputAction.CallbackContext context)
    {
        if (UIManager.instance.ActiveUI != UIMode.ListeningDrone) return;
        if (!context.started) return;
        Close();
    }

    public void OnKey(InputAction.CallbackContext context)
    {
        Debug.Log("OnKey");
        if (UIManager.instance.ActiveUI != UIMode.ListeningDrone) return;
        _input = context.ReadValue<Vector2>();
        Debug.Log("Input: " + _input);
        _input = _input.normalized;
    }

    private void Update()
    {
        ApplyMovementVector(_input);
    }

    private void ApplyMovementVector(Vector2 input)
    {
        cursor.transform.position += new Vector3(input.x, input.y, 0);
    }
    
    private void Close()
    {
        UIManager.instance.Exit();
    }
}
