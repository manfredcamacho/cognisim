using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    [SerializeField] private FeedbackToast toastPanel; // Panel que se mostrará
    [SerializeField] private bool isCorrectCollider; // Configuración externa para determinar si el collider es correcto o incorrecto

    private void OnMouseDown()
    {
        if (toastPanel != null)
        {
            if (isCorrectCollider)
            {
                toastPanel.ShowMessage("¡Correcto!", MessageType.Success);
                Debug.Log("ClickHandler: Correct collider clicked.");
            }
            else
            {
                toastPanel.ShowMessage("¡Incorrecto!", MessageType.Error);
                Debug.Log("ClickHandler: Incorrect collider clicked.");
            }
        }
    }
}