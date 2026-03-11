using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    
    public UnityEvent OnCall, OnListen;
    
    private Listener _listener;
    public Listener Listener => _listener;
    private Speaker _speaker;
    public Speaker Speaker => _speaker;

    public void TryCall()
    {
        OnCall.Invoke();
    }

    public void TryListen()
    {
        OnListen.Invoke();
    }

    public void ConnectListener()
    {
        _listener = GetComponent<Listener>();
        if (_listener == null)
        {
            _listener = gameObject.AddComponent<Listener>();
        }

        /*for (int i = 0; i < OnCall.GetPersistentEventCount(); i++)
        {
            if (OnCall.GetPersistentMethodName(i).Equals(nameof(_listener.ReactToKey))) return;
        }*/
        UnityEditor.Events.UnityEventTools.AddPersistentListener(OnCall, _listener.ReactToKey);
    }

    public void ConnectSpeaker()
    {
        _speaker = GetComponent<Speaker>();
        if (_speaker == null)
        {
            _speaker = gameObject.AddComponent<Speaker>();
        }

        /*for (int i = 0; i < OnListen.GetPersistentEventCount(); i++)
        {
            if (OnListen.GetPersistentMethodName(i).Equals(nameof(_speaker.Listen))) return;
        } */
        UnityEditor.Events.UnityEventTools.AddPersistentListener(OnListen, _speaker.Listen);
    }

    public void DisconnectListener()
    {
        UnityEditor.Events.UnityEventTools.RemovePersistentListener(OnCall, _listener.ReactToKey);
        DestroyImmediate(_listener);
        _listener = null;
    }

    public void DisconnectSpeaker()
    {
        UnityEditor.Events.UnityEventTools.RemovePersistentListener(OnListen, _speaker.Listen);
        DestroyImmediate(_speaker);
        _speaker = null;
    }
}