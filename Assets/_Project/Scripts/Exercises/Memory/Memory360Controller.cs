using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class Memory360Controller : MonoBehaviour, IExerciseController
{
    // --- Inspector Fields ---
    [SerializeField] private CameraView360 cameraMouse; // El script que creaste para mover la cámara
    
    // --- IExerciseController ---
    public event Action OnExerciseComplete;
    
    // --- Private Fields ---
    private string exerciseId;
    private List<string> targetsToFind;
    private List<string> targetsFound;
    private bool exerciseIsActive = false;

    // El diálogo de inicio llamará a este método cuando el usuario presione "Iniciar"
    public void StartExercise()
    {
        exerciseIsActive = true;
        cameraMouse.enabled = true;
        MetricsManager.Instance.LogEvent(exerciseId, "ExerciseStart", null, $"Targets: {string.Join(", ", targetsToFind)}");
    }

    // El SessionController llama a este método PRIMERO
    public void Initialize(string exerciseId, ExerciseParameters parameters)
    {
        this.exerciseId = exerciseId;
        targetsToFind = new List<string>(parameters.targetObjects);
        targetsFound = new List<string>();

        // Deshabilitar la cámara al inicio, hasta que el usuario lea las instrucciones
        cameraMouse.enabled = false;

        // 1. Encontrar y suscribirse a todos los objetos interactivos en la escena
        InteractableObject360[] allObjects = GetComponentsInChildren<InteractableObject360>();
        foreach (InteractableObject360 obj in allObjects)
        {
            obj.OnObjectClicked += HandleObjectClicked;
        }
    }

    /// <summary>
    /// Este método se llama CADA VEZ que se hace clic en un InteractableObject360.
    /// </summary>
    private void HandleObjectClicked(InteractableObject360 clickedObject)
    {
        if (!exerciseIsActive) return; // No hacer nada si el ejercicio no ha comenzado

        string id = clickedObject.objectId;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(clickedObject.transform.position);

        // Registrar CADA clic. Esta es una métrica de proceso clave.
        MetricsManager.Instance.LogEvent(exerciseId, "UserClick", screenPos, $"Clicked: {id}");

        // --- Lógica de Acierto / Error ---

        // 1. ¿Es un objeto que estábamos buscando?
        if (targetsToFind.Contains(id))
        {
            // 2. ¿Es la primera vez que lo encontramos?
            if (!targetsFound.Contains(id))
            {
                // ¡Acierto!
                targetsFound.Add(id);
                clickedObject.ShowFeedback(true); // Mostrar feedback positivo
                MetricsManager.Instance.LogEvent(exerciseId, "CorrectSelection", screenPos, id);

                // 3. ¿Hemos encontrado todos?
                if (targetsFound.Count == targetsToFind.Count)
                {
                    EndExercise("Completed");
                }
            }
            else
            {
                // Clic redundante (ya lo había encontrado)
                MetricsManager.Instance.LogEvent(exerciseId, "Click_Redundant", screenPos, id);
            }
        }
        else
        {
            // Error (clic en un distractor)
            MetricsManager.Instance.LogEvent(exerciseId, "ErrorCommission", screenPos, id);
            // (Opcional) Dar feedback negativo, como un sonido de "error"
        }
    }

    private void EndExercise(string reason)
    {
        if (!exerciseIsActive) return;
        exerciseIsActive = false;

        //cameraMouseLook.enabled = false; // Deshabilitar movimiento de cámara
        MetricsManager.Instance.LogEvent(exerciseId, "ExerciseEnd", null, $"Reason: {reason}");

        // Notificar al SessionController que terminamos
        OnExerciseComplete?.Invoke();
    }
}