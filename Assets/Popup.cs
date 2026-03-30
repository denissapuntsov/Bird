using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Popup : MonoBehaviour
{
    private TextMeshProUGUI _popupText;
    public Transform linkedTransform;

    private Tweener _scaleTweener;
    
    private void OnEnable()
    {
        _popupText = GetComponentInChildren<TextMeshProUGUI>();
        transform.localScale = Vector3.zero;
        _scaleTweener = transform.DOScale(1, 0.2f).SetAutoKill(false);
        _scaleTweener.PlayForward();
    }

    public void Close(Action callback)
    {
        _scaleTweener.PlayBackwards();
        _scaleTweener.OnRewind(() => callback?.Invoke());
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
