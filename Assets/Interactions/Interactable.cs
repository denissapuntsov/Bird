using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Interactable : MonoBehaviour
{
    [Header("Interactions")] 
    public UnityEvent OnCall;
    public UnityEvent OnListen;
    
    [SerializeField] private Listener _listener;
    public Listener Listener => _listener;
    
    [SerializeField] private Speaker _speaker;
    public Speaker Speaker => _speaker;
    
    [Header("Popups")]
    [SerializeField] private string defaultText;
    private Popup _popup;
    public Popup Popup
    {
        get => _popup;
        set
        {
            if (!value)
            {
                Destroy(_popup.gameObject);
                return;
            }
            _popup = value;
            _popup.linkedTransform = transform;
            UpdateText(defaultText);
        }
    }
    
    public void UpdateText(string newText) => Popup.Text = newText;

    public void TryCall()
    {
        OnCall.Invoke();
    }

    public void TryListen()
    {
        OnListen.Invoke();
    }

    public static bool operator == (Interactable a, Interactable b)
    {
        if (ReferenceEquals(a, b)) return true;
        return false;
    }

    public static bool operator != (Interactable a, Interactable b)
    {
        if (ReferenceEquals(a, b)) return false;
        return true;
    }

    #if UNITY_EDITOR
    
    public void ConnectListenerEvent()
    {
        UnityEditor.Events.UnityEventTools.AddPersistentListener(OnCall, _listener.ReactToKey);
    }

    public void DisconnectListenerEvent()
    {
        UnityEditor.Events.UnityEventTools.RemovePersistentListener(OnCall, _listener.ReactToKey);
        _listener = null;
    }

    public void ConnectSpeakerEvent()
    {
        UnityEditor.Events.UnityEventTools.AddPersistentListener(OnListen, _speaker.Listen);
    }

    public void DisconnectSpeakerEvent()
    {
        UnityEditor.Events.UnityEventTools.RemovePersistentListener(OnListen, _speaker.Listen);
        _speaker = null;
    }
    
    #endif
}