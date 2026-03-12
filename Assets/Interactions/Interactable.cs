using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    
    public UnityEvent OnCall, OnListen;
    
    [SerializeField] private Listener _listener;
    public Listener Listener => _listener;
    
    [SerializeField] private Speaker _speaker;
    public Speaker Speaker => _speaker;

    public void TryCall()
    {
        OnCall.Invoke();
    }

    public void TryListen()
    {
        OnListen.Invoke();
    }

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
}