using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup pause, listeningRhythm, listeningDrone, world;

    private UIVisualGroup _pauseGroup;

    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private Canvas canvas;
    private Dictionary<int, Popup> _popups = new Dictionary<int, Popup>();
    
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
                    _activeCanvasGroup = pause;
                    break;
                case UIMode.ListeningRhythm:
                    InputMapManager.SetCurrentActionMap(ActionMap.Listening);
                    _activeCanvasGroup = listeningRhythm;
                    break;
                case UIMode.ListeningDrone:
                    InputMapManager.SetCurrentActionMap(ActionMap.Listening);
                    _activeCanvasGroup = listeningDrone;
                    break;
                case UIMode.None: 
                    InputMapManager.SetCurrentActionMap(ActionMap.Player);
                    _activeCanvasGroup = world;
                    break;
            }
            
            Time.timeScale = ActiveUI == UIMode.Pause ? 0f : 1f;
            
            foreach (CanvasGroup group in canvas.GetComponentsInChildren<CanvasGroup>(true))
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

    private void Start()
    {
        _pauseGroup = pause.GetComponent<UIVisualGroup>();
        Debug.Log(_pauseGroup);
    }

    public void Exit()
    {
        switch (ActiveUI)
        {
            case UIMode.ListeningDrone:
            case UIMode.ListeningRhythm:
                ActiveUI = UIMode.None;
                break;
            case UIMode.Pause:
                _pauseGroup.Close(() =>
                {
                    ActiveUI = UIMode.None;
                });
                break;
            case UIMode.None:
                ActiveUI = UIMode.Pause;
                _pauseGroup.Open();
                break;
        }
    }

    public void OnExit(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        Exit();
    }

    public void CreatePopup(Interactable interactable)
    {
        if (_popups.ContainsKey(interactable.GetHashCode())) return;
        var newPopup = Instantiate(popupPrefab, world.transform, true).GetComponent<Popup>();
        newPopup.name = $"Popup ({interactable.name})";
        newPopup.Text = interactable.defaultText;
        newPopup.Open();
        Link(interactable, newPopup);
    }

    private void Link(Interactable interactable, Popup newPopup)
    {
        newPopup.linkedTransform = interactable.transform;
        _popups.Add(interactable.GetHashCode(), newPopup);
    }
    
    public void HidePopup(Interactable interactable)
    {
        if (!interactable) return;
        _popups.TryGetValue(interactable.GetHashCode(), out var popupToClose);
        popupToClose?.Close(() => CleanUpPopup(interactable.GetHashCode()));
    }

    private void CleanUpPopup(int interactableHashCode)
    {
        var popupToCleanup = _popups[interactableHashCode];
        Destroy(popupToCleanup.gameObject);
        _popups.Remove(interactableHashCode);
    }
}

public enum UIMode
{
    Pause,
    ListeningRhythm,
    ListeningDrone,
    None
}
