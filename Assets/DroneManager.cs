using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DroneManager : MonoBehaviour, ISpeakerManager
{
    [SerializeField] private DroneCursor cursor;
    [SerializeField] private GameObject objective;
    [SerializeField] private RectTransform puzzleArea;
    
    private Vector3[] _bounds = new Vector3[4];
    private float _minX, _minY, _maxX, _maxY;
    private const string OBJECTIVE_TAG = "DroneObjective";
    
    // DEMO PURPOSES

    [SerializeField] private AudioSource source1, source2;
    [SerializeField] private AudioClip ambience, fountain;
    
    [SerializeField] private AK.Wwise.Event objectiveEvent;
    
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

    private void Start()
    {
        SetTransformBounds();

        cursor.OnTriggerEnterEvent.AddListener(ProcessTriggerEnter);
        cursor.OnTriggerExitEvent.AddListener(ProcessTriggerExit);
    }

    private void SetTransformBounds()
    {
        puzzleArea.GetWorldCorners(_bounds);
        var cursorRectTransform = cursor.GetComponent<RectTransform>();
        _minX = _bounds[0].x + cursorRectTransform.rect.width;
        _minY = _bounds[0].y + cursorRectTransform.rect.height;
        _maxX = _bounds[2].x - cursorRectTransform.rect.width;
        _maxY = _bounds[2].y - cursorRectTransform.rect.height;
    }

    #region ISpeakerManager

    public void Setup(Speaker speaker)
    {
        
    }
    
    public void ProcessKeys(InputAction.CallbackContext context)
    {
        if (UIManager.instance.ActiveUI != UIMode.ListeningDrone) return;
        _input = context.ReadValue<Vector2>();
        _input = _input.normalized;
    }
    
    public void Extract()
    {
        PlayerInventory.instance.CurrentVocalization = objectiveEvent;
        source1.Stop();
        source2.Stop();
        Close();
    }
    
    public void Close()
    {
        UIManager.instance.ActiveUI = UIMode.None;
    }

    #endregion

    private void Update()
    {
        ApplyMovementVector(_input);
        UpdateDistance();

        if (source1.volume >= 0.5f) Extract();
    }


    private void ApplyMovementVector(Vector2 input)
    {
        cursor.transform.position += new Vector3(input.x, input.y, 0);
        var posX = cursor.transform.position.x;
        var posY = cursor.transform.position.y;
        
        posX = Mathf.Clamp(posX, _minX, _maxX);
        posY = Mathf.Clamp(posY, _minY, _maxY);
        
        cursor.transform.position = new Vector3(posX, posY, 0);
    }
    
    private void ProcessTriggerEnter(Collider2D other)
    {
        if (!other.gameObject.CompareTag(OBJECTIVE_TAG)) return;
        cursor.gameObject.SetActive(false);
        source1.Stop();
        source2.Stop();
        Extract();
    }
    
    private void ProcessTriggerExit(Collider2D other)
    {
        return;
    }

    private void UpdateDistance()
    {
        var distance = Vector2.Distance(cursor.transform.position, objective.transform.position);
        
        source1.volume = Mathf.Clamp01(distance > 500 ? 0f : (600 - distance) / 500) * 0.5f;
        source2.volume = (1 - source1.volume) * 0.3f;
    }
}
