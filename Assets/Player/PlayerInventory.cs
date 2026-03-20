using System;
using TMPro;
using UnityEditor;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentVocalizationText;
    private AK.Wwise.Event _currentVocalization;

    [SerializeField] private AK.Wwise.Event startingVocalization;
    public AK.Wwise.Event CurrentVocalization
    {
        get { return _currentVocalization; }
        set
        {
            _currentVocalization = value;
            currentVocalizationText.text = $"Current Sound: {value.Name.Replace("Play_Player_", "")}";
        }
    }
    
    public static PlayerInventory instance;

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
    }

    private void Start()
    {
        CurrentVocalization = startingVocalization;
    }
}
