using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    // --- Variables de Movimiento ---
    [Header("Configuración del Jugador")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -9.81f;

    // --- Variables de la Cámara ---
    [Header("Configuración de la Cámara")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float lookSpeed = 2.0f;
    [SerializeField] private float lookXLimit = 45.0f; // Límite para mirar arriba/abajo

    // --- Componentes y Referencias ---
    private CharacterController characterController;
    private PlayerControls playerControls;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private float rotationX = 0;

    private void Awake()
    {
        // Inicializamos los controles generados
        playerControls = new PlayerControls();
        characterController = GetComponent<CharacterController>();

        // Ocultar y bloquear el cursor en el centro de la pantalla
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        // Habilitamos el mapa de acciones
        playerControls.Player.Enable();

        // Suscribimos los métodos a los eventos de las acciones
        playerControls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerControls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        playerControls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        playerControls.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        playerControls.Player.Jump.performed += ctx => Jump();
    }

    private void OnDisable()
    {
        // Deshabilitamos el mapa de acciones para evitar errores
        playerControls.Player.Disable();
    }

    void Update()
    {
        HandleMovement();
        HandleLook();
    }

    private void HandleMovement()
    {
        // Comprobamos si el personaje está en el suelo
        isGrounded = characterController.isGrounded;

        // Si estamos en el suelo y caemos, reseteamos la velocidad vertical
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; // Un pequeño valor para mantenerlo pegado al suelo
        }

        // Calculamos la dirección del movimiento basada en la entrada del jugador
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

        // Aplicamos la gravedad
        playerVelocity.y += gravity * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);
    }

    private void Jump()
    {
        // Solo saltamos si estamos en el suelo
        if (isGrounded)
        {
            // La fórmula del salto: v = sqrt(h * -2 * g)
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void HandleLook()
    {
        // Rotación horizontal (izquierda/derecha) - Afecta al cuerpo del personaje
        transform.Rotate(Vector3.up * lookInput.x * lookSpeed);

        // Rotación vertical (arriba/abajo) - Afecta solo a la cámara
        rotationX -= lookInput.y * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit); // Limitamos la rotación
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }
}