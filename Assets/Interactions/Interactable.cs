using System;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    
    public UnityEvent OnCall, OnListen;

    [InspectorButton("ConnectSpeaker", ButtonWidth = 200)]
    public bool connectSpeaker;
    [InspectorButton("ConnectListener", ButtonWidth = 200)] 
    public bool connectListener;
    
    private Listener _listener;
    private Speaker _speaker;
    
    public void TryCall()
    {
        OnCall.Invoke();
    }

    public void TryListen()
    {
        OnListen.Invoke();
    }

    private void ConnectListener()
    {
        _listener = GetComponent<Listener>();
        if (_listener == null)
        {
            _listener = gameObject.AddComponent<Listener>();
        }

        for (int i = 0; i < OnCall.GetPersistentEventCount(); i++)
        {
            if (OnCall.GetPersistentMethodName(i).Equals(nameof(_listener.ReactToKey))) return;
        }
        UnityEditor.Events.UnityEventTools.AddPersistentListener(OnCall, _listener.ReactToKey);
    }

    private void ConnectSpeaker()
    {
        _speaker = GetComponent<Speaker>();
        if (_speaker == null)
        {
            _speaker = gameObject.AddComponent<Speaker>();
        }

        for (int i = 0; i < OnListen.GetPersistentEventCount(); i++)
        {
            if (OnListen.GetPersistentMethodName(i).Equals(nameof(_speaker.Listen))) return;
        } 
        UnityEditor.Events.UnityEventTools.AddPersistentListener(OnListen, _speaker.Listen);
    }
}
