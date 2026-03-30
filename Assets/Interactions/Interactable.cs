using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Interactable : MonoBehaviour
{
    [Header("Interactions")] 
    private UnityEvent _onCall;

    [HideInInspector] public UnityEvent OnListen;
    
    [SerializeField] private Listener _listener;
    public Listener Listener => _listener;
    
    [SerializeField] private Speaker _speaker;
    public Speaker Speaker => _speaker;
    
    public string defaultText;
    
    private Popup _popup;
    public Popup Popup
    {
        get => _popup;
        set
        {
            if (!value)
            {
                _popup?.Close(() => Destroy(_popup?.gameObject));
                _popup = null;
                return;
            }
            _popup = value;
            _popup.linkedTransform = transform;
            UpdateText(defaultText);
        }
    }
    
    public void UpdateText(string newText) => Popup.Text = newText;

    public void TryListen()
    {
        OnListen.Invoke();
    }

    public void TrySubscribe()
    {
        if (Listener)
        {
            Listener.SubscribeToPlayerAudio();
        }
    }

    public void TryUnsubscribe()
    {
        if (Listener)
        {
            Listener.UnsubscribeFromPlayerAudio();
        }
    }

    private void OnDisable()
    {
        if (!Popup) return;
        UIManager.instance.HidePopup(this);
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
    
    /*public void ConnectListenerEvent()
    {
        UnityEditor.Events.UnityEventTools.AddPersistentListener(OnCall, _listener.ReactToKey);
    }

    public void DisconnectListenerEvent()
    {
        UnityEditor.Events.UnityEventTools.RemovePersistentListener(OnCall, _listener.ReactToKey);
        _listener = null;
    }
    */

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