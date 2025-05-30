using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;
    [Header("MOVEMENT INPUT")]
    [SerializeField] Vector2 movementInput;
    public float horizontalInput;
    public float verticalInput; 
    public float moveAmount;
    [Header("CAMERA INPUT")]
    [SerializeField] Vector2 cameraInput;
    public float cameraVerticalInput;
    public float cameraHorizontalInput;
    

    PlayerInput playerInput;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        HandleMovementInput();
        CameraMovementInput();
    }
    private void OnEnable()
    {
        if (playerInput == null)
        {
            playerInput = new PlayerInput();            
            playerInput.PlayerMovement.Move.performed += i => movementInput = i.ReadValue<Vector2>();
            playerInput.PlayerCamera.CameraControls.performed += i => cameraInput = i.ReadValue<Vector2>();
        }
        playerInput.Enable();
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;         
        horizontalInput = movementInput.x;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));

        if (moveAmount <= 0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5 && moveAmount < 1)
        {
            moveAmount = 1f;
        }
    }

    private void CameraMovementInput() 
    {
        cameraVerticalInput = cameraInput.y;
        cameraHorizontalInput = cameraInput.x;
    }
}
