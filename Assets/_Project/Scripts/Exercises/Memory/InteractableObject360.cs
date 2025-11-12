using UnityEngine;
using System;

/// <summary>
/// Script de apoyo para cualquier objeto en la escena 360 con el que se pueda interactuar.
/// Se coloca en un GameObject que tenga un Collider.
/// </summary>
public class InteractableObject360 : MonoBehaviour
{

    public string objectId = "default_object";

    // Evento que notifica al controlador principal que este objeto ha sido clickeado.
    public event Action<InteractableObject360> OnObjectClicked;

    // GameObject para la retroalimentación visual (ej. un 'outline' o un 'check mark' verde)

    [SerializeField] private GameObject feedbackVisual;

    private void Start()
    {
        if (feedbackVisual != null)
        {
            feedbackVisual.SetActive(false); // Ocultar al inicio
        }
    }

    /// <summary>
    /// Muestra la retroalimentación visual.
    /// </summary>
    /// <param name="isCorrect">True si fue un acierto, False para un error.</param>
    public void ShowFeedback(bool isCorrect)
    {
        if (feedbackVisual == null) return;

        // Aquí puedes poner lógica más compleja (ej. color verde para acierto, rojo para error)
        if (isCorrect)
        {
            feedbackVisual.SetActive(true);
        }
        else
        {
            // Podrías instanciar un efecto de "X" roja temporal, por ejemplo.
            // Por ahora, solo lo activamos en acierto.
        }
    }
}