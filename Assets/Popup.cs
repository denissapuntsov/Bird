using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Popup : MonoBehaviour
{
    private TextMeshProUGUI _popupText;
    public Transform linkedTransform;

    private void OnEnable()
    {
        _popupText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (!linkedTransform) return;
        transform.position = Camera.main.WorldToScreenPoint(linkedTransform.position) + new Vector3(-40f, 100f, 0);
    }

    public string Text
    {
        set => _popupText.text = value;
    }
}
