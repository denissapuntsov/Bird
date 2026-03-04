using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup pauseGroup, listeningGroup;
    
    private CanvasGroup _activeCanvasGroup;

    private UIMode _activeUI;

    public UIMode ActiveUI
    {
        get => _activeUI;
        set
        {
            switch (value)
            {
                case UIMode.Pause:
                    InputMapManager.SetCurrentActionMap(ActionMap.UI);
                    _activeCanvasGroup = pauseGroup;
                    break;
                case UIMode.Listening:
                    InputMapManager.SetCurrentActionMap(ActionMap.Listening);
                    _activeCanvasGroup = listeningGroup;
                    break;
                case UIMode.None:
                    InputMapManager.SetCurrentActionMap(ActionMap.Player);
                    _activeCanvasGroup = null;
                    break;
            }
            
            foreach (CanvasGroup group in GetComponentsInChildren<CanvasGroup>(true))
            {
                Debug.Log(group.name);
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
}

public enum UIMode
{
    Pause,
    Listening,
    None
}
