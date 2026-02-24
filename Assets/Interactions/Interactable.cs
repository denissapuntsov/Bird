using System;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent OnCall, OnListen;

    public void TryCall()
    {
        OnCall.Invoke();
    }

    public void TryListen()
    {
        OnListen.Invoke();
    }
}
