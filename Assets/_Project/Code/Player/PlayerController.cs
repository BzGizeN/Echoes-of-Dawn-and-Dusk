using UnityEngine;
using UnityEngine.InputSystem;
using Echoes.Inputs;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 6f;
    public float rotationSpeed = 15f;

    [Header("References")]
    public CharacterController characterController;
    
    private InputSystem_Actions _inputActions;
    private Camera _mainCamera;
    private Vector2 _moveInput;
    private Vector2 _mousePos;

    private void Awake()
    {
        _inputActions = new InputSystem_Actions();
        _mainCamera = Camera.main;

        if (characterController == null) characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Player.Disable();
    }

    private void Update()
    {
        ReadInput();
        HandleMovement();
        HandleRotation();
    }

    private void ReadInput()
    {
        _moveInput = _inputActions.Player.Move.ReadValue<Vector2>();
        _mousePos = Mouse.current.position.ReadValue();
    }

    private void HandleMovement()
    {
        if (_moveInput.magnitude < 0.1f) return;

        Vector3 camForward = _mainCamera.transform.forward;
        Vector3 camRight = _mainCamera.transform.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = (camForward * _moveInput.y + camRight * _moveInput.x).normalized;

        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        Ray ray = _mainCamera.ScreenPointToRay(_mousePos);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            
            Vector3 lookDir = hitPoint - transform.position;
            lookDir.y = 0;

            if (lookDir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}