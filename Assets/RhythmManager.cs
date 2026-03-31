using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RhythmManager : MonoBehaviour, ISpeakerManager
{
    [SerializeField] HorizontalLayoutGroup layoutGroup;
    [SerializeField] private CueIndicator indicator;
    
    [SerializeField] RhythmData currentRhythmData;
    [SerializeField] AudioData currentAudioData;
    [SerializeField] private Letter w, a, s, d;
    [SerializeField] private Cue _currentCue;
    
    private AudioSource _audioSource;
    private float _beforeSample = 1f;

    [SerializeField] List<Cue> passedCues = new List<Cue>();
    
    
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

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    
    public void Setup(Speaker speaker)
    {
        if (!speaker) return;
        currentRhythmData = speaker.rhythmData;
        currentAudioData = speaker.audioData;

        foreach (Transform child in layoutGroup.transform)
        {
            Destroy(child.gameObject);
        }
        
        // populate layout group
        foreach (Cue cue in currentRhythmData.cuePoints)
        {
            var newIndicator = Instantiate(indicator, layoutGroup.transform);
            newIndicator.name = cue.key.ToString();
            newIndicator.transform.SetAsFirstSibling();
        }
        
        _currentCue = null;

        Invoke(nameof(Play), 1);
    }

    public void Play()
    {
        _audioSource.clip = currentRhythmData.audioClip;
        passedCues.Clear();
        _audioSource.Play();
    }
    
    void Update()
    {
        if (!_audioSource.clip || !_audioSource.isPlaying) return;
        foreach (Cue cue in currentRhythmData.cuePoints)
        {
            CheckForPosition(cue);
        }
        CheckForLoop();
    }

    private void CheckForPosition(Cue cue)
    {
        if (_audioSource.timeSamples > cue.position && !passedCues.Contains(cue))
        {
            //var delay = (_audioSource.timeSamples - cue.positionInSamples) / 22050f * 1000;
            _currentCue = cue;
            OnKeyRead(cue.key);
            passedCues.Add(cue);
        }
    }

    private void CheckForLoop()
    {
        if (_beforeSample < _audioSource.timeSamples)
        {
            _beforeSample =  _audioSource.timeSamples - 1f;
        }
        else if (_beforeSample > _audioSource.timeSamples)
        {
            Loop();

            if (!_audioSource.clip) return;
            
            foreach (Cue cue in currentRhythmData.cuePoints)
            {
                cue.isMatched = false;
                foreach (Transform child in layoutGroup.transform)
                {
                    child.GetComponent<CueIndicator>().IsMatched = false;
                }
            }
            
            _beforeSample = _audioSource.timeSamples - 1f;
        }
    }

    private void Loop()
    {
        if (passedCues.Count == 0) return;
        
        foreach (Cue cue in passedCues)
        {
            if (cue.isMatched) continue;
            passedCues.Clear();
            return;
        }
        
        Extract();
    }


    public void ProcessKeys(InputAction.CallbackContext context)
    {
        if (_currentCue == null)
        {
            ClearCueMatches();
            return;
        }

        CueKey pressedKey;
        var input = ((int)context.ReadValue<Vector2>().x, (int)context.ReadValue<Vector2>().y);

        pressedKey = input switch
        {
            (0, 1) => CueKey.W,
            (-1, 0) => CueKey.A,
            (0, -1) => CueKey.S,
            (1, 0) => CueKey.D,
            _ => CueKey.None
        };
        
        
        if (_currentCue.key == pressedKey)
        {
            _currentCue.isMatched = true;
            layoutGroup.transform.GetChild(currentRhythmData.cuePoints.IndexOf(_currentCue)).GetComponent<CueIndicator>().IsMatched = true;
            Debug.Log(pressedKey);
            return;
        }

        ClearCueMatches();
    }

    private void ClearCueMatches()
    {
        foreach (Cue cue in currentRhythmData.cuePoints)
        {
            cue.isMatched = false;
            foreach (CueIndicator child in layoutGroup.GetComponentsInChildren<CueIndicator>())
            {
                child.IsMatched = false;
            }
        }
    }

    private Letter GetLetterFromCueKey(CueKey cueKey)
    {
        return cueKey switch
        {
            CueKey.A => a,
            CueKey.W => w,
            CueKey.S => s,
            CueKey.D => d,
            _ => null
        };
    }

    public void Extract()
    {
        _audioSource.clip = null;
        PlayerInventory.instance.currentAudioData = currentAudioData;
        currentRhythmData = null;
        _audioSource.Stop();
        Close();
        
    }

    public void Close()
    {
        UIManager.instance.Exit();
    }
    
    private void OnKeyRead(CueKey key)
    {
        GetLetterFromCueKey(key).transform.DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), 0.5f, 0, 1f).OnComplete(() => _currentCue = null);
    }
}
