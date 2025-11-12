using UnityEngine;
using UnityEngine.InputSystem; 

// Asegura que este script esté siempre junto al RayPointer
[RequireComponent(typeof(RayPointer))]
public class PlayerInteractor : MonoBehaviour
{
    private RayPointer rayPointer;

    void Start()
    {
        // Obtener la referencia al script que ya tienes en la cámara
        rayPointer = GetComponent<RayPointer>();
    }

    void Update()
    {
        // 1. Detectar el clic usando el NUEVO Input System
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // 2. Preguntarle al RayPointer si está golpeando algo
            if (rayPointer.IsPointingAtObject())
            {
                // 3. Obtener el objeto golpeado
                GameObject detectedObject = rayPointer.GetDetectedObject();

                Debug.Log("RayPointer golpeó a: " + detectedObject.name);

                // 4. Intentar obtener el script interactivo de ese objeto
                InteractableObject360 interactable = detectedObject.GetComponent<InteractableObject360>();

                if (interactable != null)
                {
                    // 5. ¡ÉXITO! Llamar a la función pública del objeto
                    Debug.Log("¡Objeto interactivo encontrado! Mostrando feedback.");

                    // Aquí puedes invocar el evento si lo necesitas, o llamar a ShowFeedback
                    // interactable.OnObjectClicked?.Invoke(interactable); // Si otro script lo necesita

                    // O simplemente mostrar el feedback
                    interactable.ShowFeedback(true);
                }
            }
        }
    }
}