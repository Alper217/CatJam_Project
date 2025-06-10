using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarlarý")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float acceleration = 10f;
    public float deceleration = 10f;

    [Header("Mouse Look Ayarlarý")]
    public float mouseSensitivity = 100f;
    public float maxLookUp = 90f;
    public float maxLookDown = -90f;
    public bool invertY = false;

    [Header("Kamera")]
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 moveDirection;
    private float currentSpeed;
    private float targetSpeed;

    // Mouse look deðiþkenleri
    private float xRotation = 0f;
    private float yRotation = 0f;

    // Input deðiþkenleri
    private float horizontal;
    private float vertical;
    private bool isRunning;
    public bool canRun;

    void Start()
    {
        InitializePlayer();
    }

    void InitializePlayer()
    {
        controller = GetComponent<CharacterController>();

        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.5f;
            controller.center = new Vector3(0, 0.9f, 0);
        }

        // Kamera yoksa ana kamerayý bul
        if (cameraTransform == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                cameraTransform = mainCam.transform;
            }
            else
            {
                Debug.LogWarning("Kamera bulunamadý! Lütfen cameraTransform'u atayýn.");
            }
        }

        // Mouse cursor'unu kilitle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Baþlangýç rotasyonunu kaydet
        yRotation = transform.eulerAngles.y;
        if (cameraTransform != null)
        {
            xRotation = cameraTransform.eulerAngles.x;
        }
    }

    void Update()
    {
        HandleInput();
        HandleMouseLook();
        HandleMovement();

        // ESC ile cursor'u serbest býrak
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //ToggleCursor();
        }
    }

    void HandleInput()
    {
        // Hareket inputlarý
        horizontal = Input.GetAxisRaw("Horizontal"); // A/D veya Sol/Sað ok tuþlarý
        vertical = Input.GetAxisRaw("Vertical");     // W/S veya Yukarý/Aþaðý ok tuþlarý

        // Koþma inputu
        isRunning = Input.GetKey(KeyCode.LeftShift);
    }

    void HandleMouseLook()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        // Mouse inputlarýný al
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Y ekseni ters çevrilsin mi?
        if (invertY)
            mouseY = -mouseY;

        // Yatay rotasyon (Y ekseni) - karakterin kendisi döner
        yRotation += mouseX;
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

        // Dikey rotasyon (X ekseni) - sadece kamera döner
        if (cameraTransform != null)
        {
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, maxLookDown, maxLookUp);
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }

    void HandleMovement()
    {
        // Hedef hýzý belirle
        if ((horizontal != 0 || vertical != 0))
        {
            targetSpeed = (isRunning && canRun) ? runSpeed : walkSpeed;
        }
        else
        {
            targetSpeed = 0f;
        }

        // Yumuþak hýz geçiþi
        if (targetSpeed > currentSpeed)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, deceleration * Time.deltaTime);
        }

        // Hareket yönünü hesapla (karakterin yönüne göre)
        moveDirection = transform.right * horizontal + transform.forward * vertical;
        moveDirection = moveDirection.normalized * currentSpeed;

        // Yerçekimi etkisi (basit)
        //if (!controller.isGrounded)
        //{
        //    velocity.y += Physics.gravity.y * Time.deltaTime;
        //}
        //else
        //{
        //    velocity.y = -2f; // Yere yapýþýk kalmasý için
        //}

        // Final hareket vektörü
        Vector3 finalMovement = moveDirection + Vector3.up * velocity.y;

        // Hareketi uygula
        controller.Move(finalMovement * Time.deltaTime);
    }

    //void ToggleCursor()
    //{
    //    if (Cursor.lockState == CursorLockMode.Locked)
    //    {
    //        Cursor.lockState = CursorLockMode.None;
    //        Cursor.visible = true;
    //    }
    //    else
    //    {
    //        Cursor.lockState = CursorLockMode.Locked;
    //        Cursor.visible = false;
    //    }
    //}

    // Public fonksiyonlar - dýþarýdan eriþim için
    public bool IsMoving()
    {
        return currentSpeed > 0.1f;
    }

    public bool IsRunning()
    {
        return isRunning && IsMoving();
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public Vector3 GetMoveDirection()
    {
        return moveDirection;
    }

    // Debug için
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 0.5f);

        if (moveDirection != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, moveDirection.normalized * 2f);
        }
    }
}

// Opsiyonel: Animasyon kontrolü için ek script
[System.Serializable]
public class PlayerAnimationController : MonoBehaviour
{
    [Header("Animasyon Ayarlarý")]
    public Animator animator;

    private PlayerController playerController;

    // Animasyon parametreleri
    private int speedHash;
    private int isWalkingHash;
    private int isRunningHash;

    void Start()
    {
        playerController = GetComponent<PlayerController>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator != null)
        {
            speedHash = Animator.StringToHash("Speed");
            isWalkingHash = Animator.StringToHash("IsWalking");
            isRunningHash = Animator.StringToHash("IsRunning");
        }
    }

    void Update()
    {
        if (animator == null || playerController == null) return;

        float currentSpeed = playerController.GetCurrentSpeed();
        bool isMoving = playerController.IsMoving();
        bool isRunning = playerController.IsRunning();

        // Animasyon parametrelerini güncelle
        animator.SetFloat(speedHash, currentSpeed);
        animator.SetBool(isWalkingHash, isMoving && !isRunning);
        animator.SetBool(isRunningHash, isRunning);
    }
}