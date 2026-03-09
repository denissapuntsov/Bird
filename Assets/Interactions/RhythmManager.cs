using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class RhythmManager : MonoBehaviour
{
    private Speaker currentSpeaker = null;
    private AK.Wwise.Event _playEvent, _stopEvent;
    [SerializeField] private GameObject w, a, s, d;
    private object _myCookieObject;
    private Marker _currentMarker;
    [SerializeField] private List<Marker> markers;

    public Marker CurrentMarker
    {
        get => _currentMarker;
        set
        {
            _currentMarker = value;
            if (_currentMarker.isMatched)
            {
                _currentMarker.isMatched = false;
            }
            ExpandLetter(_currentMarker.key);
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

    public void Setup(Speaker newSpeaker)
    {
        currentSpeaker = newSpeaker;
        UIManager.instance.ActiveUI = UIMode.Listening;
        _playEvent = currentSpeaker.InteractionStartEvent;
        _stopEvent = currentSpeaker.InteractionEndEvent;
        AkUnitySoundEngine.PostEvent(_playEvent.Name, gameObject, (uint)AkCallbackType.AK_Marker, ProcessMarkerCallback, _myCookieObject);
        markers = new List<Marker>();
    }

    private void ProcessMarkerCallback(object inCookie, AkCallbackType inType, object inInfo)
    {
        if (inType != AkCallbackType.AK_Marker) return;
        AkMarkerCallbackInfo info = (AkMarkerCallbackInfo)inInfo;

        if (info.strLabel == "end")
        {

            if (!AreAllMarkersMatched())
            {
                ResetMarkerMatches();
                return;
            }
            ExtractAndClose();
            return;
        }
        
        UpdateCurrentMarker(info);
    }

    private void UpdateCurrentMarker(AkMarkerCallbackInfo info)
    {
        Marker incomingMarker = new Marker(info.uPosition, info.strLabel);
        if (!markers.Contains(incomingMarker))
        {
            markers.Add(incomingMarker);
        }

        foreach (Marker marker in markers)
        {
            if (marker.Equals(incomingMarker)) CurrentMarker = marker;
        }
    }

    private void ResetMarkerMatches()
    {
        foreach (Marker marker in markers) marker.isMatched = false;
    }

    private void ExpandLetter(string letter)
    {
        var letterGameObject = GetLetterGameObject(letter);

        letterGameObject?.transform.DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), 0.5f, 0, 1f);
    }

    private GameObject GetLetterGameObject(string letter)
    {
        GameObject letterGameObject = letter switch
        {
            "a" => a.gameObject,
            "w" => w.gameObject,
            "s" => s.gameObject,
            "d" => d.gameObject,
            _ => null
        };
        return letterGameObject;
    }

    public void OnTryKey(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        string letter = "";
        var input = ((int)context.ReadValue<Vector2>().x, (int)context.ReadValue<Vector2>().y);

        letter = input switch
        {
            (0, 1) => "w",
            (-1, 0) => "a",
            (0, -1) => "s",
            (1, 0) => "d",
            _ => "",
        };

        if (letter != CurrentMarker.key || CurrentMarker.isMatched)
        {
            ResetMarkerMatches();
            return;
        }
        
        CurrentMarker.isMatched = true;
    }

    private void ExtractAndClose()
    {
        AkUnitySoundEngine.PostEvent(_stopEvent.Name, gameObject);
        UIManager.instance.ActiveUI = UIMode.None;
        PlayerInventory.instance.currentVocalization = currentSpeaker.ExtractedSoundEvent;
    }

    private bool AreAllMarkersMatched()
    {
        if (markers.Count < 1) return false;
        
        foreach (Marker marker in markers)
        {
            if (!marker.isMatched) return false;
        }

        return true;
    }
}
