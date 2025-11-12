using UnityEngine;
using UnityEngine.InputSystem;

public class CameraView360 : MonoBehaviour
{
    [Header("Sensibilidad del Mouse")]
    [Tooltip("Sensibilidad de rotación horizontal")]
    public float sensibilidadHorizontal = 5f;

    [Tooltip("Sensibilidad de rotación vertical")]
    public float sensibilidadVertical = 5f;

    [Header("Límites Horizontales (Eje Y)")]
    [Tooltip("Activar límite horizontal")]
    public bool limitarHorizontal = false;

    [Tooltip("Ángulo mínimo horizontal (-180 a 180)")]
    public float anguloMinHorizontal = -180f;

    [Tooltip("Ángulo máximo horizontal (-180 a 180)")]
    public float anguloMaxHorizontal = 180f;

    [Header("Límites Verticales (Eje X)")]
    [Tooltip("Activar límite vertical")]
    public bool limitarVertical = true;

    [Tooltip("Ángulo mínimo vertical (mirar hacia abajo)")]
    public float anguloMinVertical = -90f;

    [Tooltip("Ángulo máximo vertical (mirar hacia arriba)")]
    public float anguloMaxVertical = 90f;

    [Header("Opciones")]
    [Tooltip("Bloquear el cursor al centro de la pantalla")]
    public bool bloquearCursor = true;

    [Tooltip("Invertir el eje Y")]
    public bool invertirEjeY = false;

    // Variables privadas
    private float rotacionX = 0f;
    private float rotacionY = 0f;

    void Start()
    {
        // Bloquear y ocultar el cursor si está activado
        if (bloquearCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Inicializar con la rotación actual de la cámara
        Vector3 rotacionActual = transform.localEulerAngles;
        rotacionY = rotacionActual.y;
        rotacionX = rotacionActual.x;

        // Normalizar el ángulo X
        if (rotacionX > 180f)
            rotacionX -= 360f;
    }

    void Update()
    {
        // Verificar que el mouse existe
        if (Mouse.current == null)
            return;

        // Obtener el delta del mouse (movimiento desde el último frame)
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // Aplicar sensibilidad (sin multiplicar por Time.deltaTime porque delta ya es frame-independent)
        float mouseX = mouseDelta.x * sensibilidadHorizontal * 0.01f;
        float mouseY = mouseDelta.y * sensibilidadVertical * 0.01f;

        // Aplicar inversión del eje Y si está activado
        if (invertirEjeY)
            mouseY = -mouseY;

        // Calcular rotación horizontal (eje Y)
        rotacionY += mouseX;

        // Aplicar límites horizontales si están activados
        if (limitarHorizontal)
        {
            rotacionY = Mathf.Clamp(rotacionY, anguloMinHorizontal, anguloMaxHorizontal);
        }
        else
        {
            // Normalizar ángulo entre -180 y 180
            if (rotacionY > 180f)
                rotacionY -= 360f;
            else if (rotacionY < -180f)
                rotacionY += 360f;
        }

        // Calcular rotación vertical (eje X) - invertido para seguir convención de Unity
        rotacionX -= mouseY;

        // Aplicar límites verticales si están activados
        if (limitarVertical)
        {
            rotacionX = Mathf.Clamp(rotacionX, anguloMinVertical, anguloMaxVertical);
        }

        // Aplicar la rotación a la cámara
        transform.localEulerAngles = new Vector3(rotacionX, rotacionY, 0f);

        // Permitir desbloquear el cursor con ESC
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
