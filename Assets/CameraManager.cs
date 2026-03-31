using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Vector3 offset;
    private bool _isRotating = false;
    private bool _isCalled = false;
    private Vector2 _input;
    
    private void Update()
    {
        transform.position = player.transform.position + offset;
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (_isRotating) return;
            _input = context.ReadValue<Vector2>();
            if (_input.x == 0) return;
            _isRotating = true;
            _isCalled = true;
            transform
                .DOBlendableRotateBy(new Vector3(0f, 45f * (_input.x > 0 ? 1 : -1), 0f), 0.75f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear)
                .OnComplete(CheckForEnd);
        }
        
        else if (context.canceled)
        {
            _isCalled = false;
        }
    }

    private void CheckForEnd()
    {
        if (_isCalled)
        {
            transform
                .DOBlendableRotateBy(new Vector3(0f, 90f * (_input.x > 0 ? 1 : -1), 0f), 1.5f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear)
                .OnComplete(CheckForEnd);
        }

        else
        {
            transform.DOBlendableRotateBy(new Vector3(0f, 45f * _input.x > 1 ? 1 : -1, 0f), 0.75f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => _isRotating = false);
        }
    }
}
