using System;
using UnityEngine;

public class SpeakerCooldown : MonoBehaviour
{
    private float _timeElapsed = 0f;
    
    private void Update()
    {
        if (UIManager.instance.ActiveUI != UIMode.None)
        {
            _timeElapsed = 0f;
            return;
        }
        _timeElapsed += Time.deltaTime;
    }
}
