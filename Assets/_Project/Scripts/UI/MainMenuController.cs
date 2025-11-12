using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private TMP_InputField sessionIdField;
    [SerializeField] private Button startSessionButton;
    [SerializeField] private Button startAsGuestButton;
    [SerializeField] private TextMeshProUGUI feedbackText;

    private void Start()
    {
        startSessionButton.onClick.AddListener(OnStartSessionClicked);
        startAsGuestButton.onClick.AddListener(OnStartAsGuestClicked);
        feedbackText.text = "";
    }

    private void OnStartAsGuestClicked()
    {
        // Start a guest session without loading any specific session data.
        SceneManager.LoadScene("01_ExerciseList");
    }

    private void OnStartSessionClicked()
    {
        string sessionId = sessionIdField.text;
        if (string.IsNullOrEmpty(sessionId))
        {
            feedbackText.text = "Ingrese un ID de Sesión";
            return;
        }

        bool sessionLoaded = SessionManager.Instance.LoadSession(sessionId);

        if (sessionLoaded)
        {
            // If the session is loaded, start the metrics and load the player scene.
            MetricsManager.Instance.StartNewSession(
                SessionManager.Instance.CurrentSession.sessionId,
                SessionManager.Instance.CurrentSession.patientId
            );
            SceneManager.LoadScene("02_SessionPlayer");
        }
        else
        {
            feedbackText.text = $"Failed to load session for ID: {sessionId}. Check console for errors.";
        }
    }
}