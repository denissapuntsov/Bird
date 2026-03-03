using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class RhythmManager : MonoBehaviour
{
    [SerializeField] private AK.Wwise.Event soundEvent;
    [SerializeField] private Letter w, a, s, d;
    private object _myCookieObject;
    private Letter _currentLetter;

    public Letter CurrentLetter
    {
        get => _currentLetter;
        set
        {
            _currentLetter = value;
            Debug.Log($"Current Letter: {_currentLetter}");
            if (_currentLetter.IsMatched)
            {
                _currentLetter.IsMatched = false;
            }
            ExpandLetter(_currentLetter.transform);
        }
    }
    
    public static RhythmManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        AkUnitySoundEngine.PostEvent(soundEvent.Name, gameObject, (uint)AkCallbackType.AK_Marker, MyCallbackFunction, _myCookieObject);
    }

    private void MyCallbackFunction(object inCookie, AkCallbackType inType, object inInfo)
    {
        if (inType == AkCallbackType.AK_Marker)
        {
            AkMarkerCallbackInfo info = (AkMarkerCallbackInfo)inInfo;
            
            //Debug.Log((float)info.uPosition / AkUnitySoundEngine.GetSampleRate());
            Debug.Log(info.strLabel);

            MakeLetterActive(info.strLabel);
        }
    }

    private void MakeLetterActive(string key)
    {
        switch (key.ToUpper())
        {
            case "W":
                CurrentLetter = w;
                break;
            case "A":
                CurrentLetter = a;
                break;
            case "S":
                CurrentLetter = s;
                break;
            case "D":
                CurrentLetter = d;
                break;
        }
    }

    private void ExpandLetter(Transform letter)
    {
        letter.DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), 0.5f, 0, 1f);
    }

    public void OnTryLetter(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            
        }
    }
}
