using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : MonoBehaviour
{
    private const float Gravity = -9.81f;

    private Vector2 _input;
    private Vector3 _horizontalInput, _verticalInput;
    private Vector3 _move;

    [Header("Movement")] [SerializeField] private float playerSpeed = 5.0f;
    
    [SerializeField] private float playerJumpHeight = 2.0f;
    [SerializeField] private float gravityMultiplier = 2.0f;
    
    private CharacterController _characterController;
    private Vector3 _finalMove, _playerVelocity;
    private Camera _mainCamera;

    private bool _isGrounded;

    public static PlayerMovement instance;


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
        _characterController = GetComponent<CharacterController>();
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        CheckIfGrounded();
        ProcessMovement();
    }

    private void CheckIfGrounded()
    {
        _isGrounded = Physics.Raycast(
            new Vector3(transform.position.x, transform.position.y,
                transform.position.z),
            Vector3.down,
            0.01f,
            1 << LayerMask.NameToLayer("Ground"));
        
        if (_isGrounded & _playerVelocity.y < -3f) _playerVelocity.y = -3f;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
        
        GetCameraInputOffset(out var cameraForward, out var cameraRight);

        cameraForward *= _input.y;
        cameraRight *= _input.x;

        _move = cameraForward + cameraRight;
    }

    private void ProcessMovement()
    {
        _playerVelocity.y += Gravity * gravityMultiplier * Time.deltaTime;

        transform.forward = _move.magnitude <= 0.2f ? transform.forward : _move;

        _finalMove = _move * playerSpeed + Vector3.up * _playerVelocity.y;
        _characterController.Move(_finalMove * Time.deltaTime);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (_isGrounded)
        {
            _playerVelocity.y = Mathf.Sqrt(playerJumpHeight * -2f * Gravity * gravityMultiplier);
        }
    }

    private void GetCameraInputOffset(out Vector3 forward, out Vector3 right)
    {
        forward = _mainCamera.transform.forward;
        right = _mainCamera.transform.right;
        
        forward.y = 0;
        right.y = 0;
        
        forward.Normalize();
        right.Normalize();
    }
}
