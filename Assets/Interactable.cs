using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Listener Listener => _listener;
    private Listener _listener;

    private void Awake()
    {
        _listener = GetComponent<Listener>();
    }
}
