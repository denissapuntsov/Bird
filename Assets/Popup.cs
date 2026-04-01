using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    private TextMeshProUGUI _popupText;
    public Transform linkedTransform;

    private Tweener _scaleTweener;

    [SerializeField] private Image icon;
    
    private void OnEnable()
    {
        _popupText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Open()
    {
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
        transform.position = Camera.main.WorldToScreenPoint(linkedTransform.position);
    }

    public string Text
    {
        set => _popupText.text = value;
    }

    public Sprite Icon
    {
        set
        {
            if (!value)
            {
                Debug.LogWarning("New icon value is null");
                return;
            }
            icon.sprite = value;
        }
    }
}
