using UnityEngine;
using TMPro; // Necesario para TextMeshPro

public class ResultsDisplay : MonoBehaviour
{
    // Asigna estos campos en el Inspector de Unity
    public TextMeshProUGUI roundsText;
    public TextMeshProUGUI attemptsText;
    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI reactionTimeText;
    public TextMeshProUGUI durationText;

    // (Opcional) Un botón para volver al menú
    // public Button mainMenuButton;

    void Start()
    {
        // 1. Comprobamos si la instancia del GameManager existe
        if (ParkController.Instance != null)
        {
            ParkController.Instance.UnlockCursor(); // Aseguramos que el cursor esté libre
            // 2. Leemos los datos guardados del Singleton
            int rounds = ParkController.Instance.FinalTotalRounds;
            float accuracy = ParkController.Instance.FinalAccuracy;
            float avgReaction = ParkController.Instance.FinalAverageReactionTime;
            float duration = ParkController.Instance.FinalTotalDuration;

            // 3. Mostramos los datos en la UI
            // Usamos .ToString("F1") o "F2" para formatear los decimales
            roundsText.text = $"{rounds}";
            accuracyText.text = $"{accuracy:F1}%";
            reactionTimeText.text = $"{avgReaction:F2} s";
            durationText.text = $"{duration:F1} s";

            // (Opcional) Puedes añadir un listener para un botón
            // mainMenuButton.onClick.AddListener(GoToMainMenu);

            // 4. (Opcional pero recomendado)
            // Destruimos la instancia para que si vuelves al menú principal
            // y empiezas un juego nuevo, no haya conflictos.
            Destroy(ParkController.Instance.gameObject);
        }
        else
        {
            // Esto pasa si inicias el juego desde la escena de resultados (para pruebas)
            Debug.LogWarning("No se encontró ParkController.Instance. Mostrando datos de prueba.");
            roundsText.text = "N/A";
            accuracyText.text = "N/A";
            reactionTimeText.text = "N/A";
            durationText.text = "N/A";
        }
    }
}