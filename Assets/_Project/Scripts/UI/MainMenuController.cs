using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private TMP_InputField patientIdField;
    [SerializeField] private Button startSessionButton;
    [SerializeField] private TextMeshProUGUI feedbackText;

    private void Start()
    {
        startSessionButton.onClick.AddListener(OnStartSessionClicked);
        feedbackText.text = "";
    }

    private void OnStartSessionClicked()
    {
        string patientId = patientIdField.text;
        if (string.IsNullOrEmpty(patientId))
        {
            feedbackText.text = "Please enter a Patient ID.";
            return;
        }

        bool sessionLoaded = SessionManager.Instance.LoadSession(patientId);

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
            feedbackText.text = $"Failed to load session for ID: {patientId}. Check console for errors.";
        }
    }
}