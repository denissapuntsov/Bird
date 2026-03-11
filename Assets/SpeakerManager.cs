using UnityEngine;
using UnityEngine.InputSystem;

public class SpeakerManager : MonoBehaviour
{
    public static SpeakerManager instance;
    private SpeakerType _speakerType;
    private ISpeakerManager _currentManager;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    
    public void Setup(Speaker newSpeaker)
    {
        InputMapManager.SetCurrentActionMap(ActionMap.Listening);
        _speakerType = newSpeaker.speakerType;
        switch (_speakerType)
        {
            case SpeakerType.Drone:
                UIManager.instance.ActiveUI = UIMode.ListeningDrone;
                _currentManager = DroneManager.instance;
                break;
            case SpeakerType.Rhythm:
                UIManager.instance.ActiveUI = UIMode.ListeningRhythm;
                _currentManager = RhythmManager.instance;
                break;
            default:
                Debug.LogError("Unknown SpeakerType: " + _speakerType);
                return;
        }
        _currentManager.Setup(newSpeaker);
    }

    public void OnKey(InputAction.CallbackContext context)
    {
        _currentManager.ProcessKeys(context);
    }

    public void OnExit(InputAction.CallbackContext context)
    {
        UIManager.instance.Exit();
        _currentManager.Close();
    }
}

public interface ISpeakerManager
{
    public void Setup(Speaker speaker);
    public void ProcessKeys(InputAction.CallbackContext context);
    public void Extract();
    public void Close();
}
