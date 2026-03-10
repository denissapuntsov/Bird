using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup pauseGroup, listeningRhythmGroup, listeningDroneGroup;
    
    private CanvasGroup _activeCanvasGroup;

    private UIMode _activeUI = UIMode.None;

    public UIMode ActiveUI
    {
        get => _activeUI;
        set
        {
            _activeUI = value;
            switch (value)
            {
                case UIMode.Pause:
                    InputMapManager.SetCurrentActionMap(ActionMap.UI);
                    _activeCanvasGroup = pauseGroup;
                    break;
                case UIMode.ListeningRhythm:
                    InputMapManager.SetCurrentActionMap(ActionMap.Listening);
                    _activeCanvasGroup = listeningRhythmGroup;
                    break;
                case UIMode.ListeningDrone:
                    InputMapManager.SetCurrentActionMap(ActionMap.Listening);
                    _activeCanvasGroup = listeningDroneGroup;
                    break;
                case UIMode.None:
                    InputMapManager.SetCurrentActionMap(ActionMap.Player);
                    _activeCanvasGroup = null;
                    break;
            }
            
            foreach (CanvasGroup group in GetComponentsInChildren<CanvasGroup>(true))
            {
                group.gameObject.SetActive(group == _activeCanvasGroup);
            }
        }
    }

    public static UIManager instance;

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

    public void Exit()
    {
        switch (ActiveUI)
        {
            case UIMode.ListeningDrone:
            case UIMode.ListeningRhythm:
            case UIMode.Pause:
                ActiveUI = UIMode.None;
                break;
            case UIMode.None:
                ActiveUI = UIMode.Pause;
                break;
        }
    }

    public void OnExit(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        Exit();
    }
}

public enum UIMode
{
    Pause,
    ListeningRhythm,
    ListeningDrone,
    None
}
