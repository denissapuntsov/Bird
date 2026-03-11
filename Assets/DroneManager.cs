using UnityEngine;
using UnityEngine.InputSystem;

public class DroneManager : MonoBehaviour, ISpeakerManager
{
    [SerializeField] private GameObject cursor;
    private Speaker _currentSpeaker;
    private Vector2 _input;
    
    public static DroneManager instance;

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

    #region ISpeakerManager

    public void Setup(Speaker speaker)
    {
        _currentSpeaker = speaker;
    }
    
    public void ProcessKeys(InputAction.CallbackContext context)
    {
        if (UIManager.instance.ActiveUI != UIMode.ListeningDrone) return;
        _input = context.ReadValue<Vector2>();
        _input = _input.normalized;
    }
    
    public void Extract()
    {
        UIManager.instance.Exit();
        Close();
    }
    
    public void Close()
    {
        return;
    }

    #endregion
    
    private void Update()
    {
        ApplyMovementVector(_input);
    }

    private void ApplyMovementVector(Vector2 input)
    {
        cursor.transform.position += new Vector3(input.x, input.y, 0);
    }
}
