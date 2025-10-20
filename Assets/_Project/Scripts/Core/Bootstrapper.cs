using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This script runs only once at the start of the application.
/// It initializes all persistent managers and then loads the main menu.
/// </summary>
public class Bootstrapper : MonoBehaviour
{
    // In the Unity Inspector, drag your Manager prefabs here.
    [SerializeField] private GameObject sessionManagerPrefab;
    [SerializeField] private GameObject metricsManagerPrefab;

    private void Awake()
    {
        // Instantiate the managers so they exist in the scene.
        // Their own Awake() methods will handle the Singleton logic.
        if (SessionManager.Instance == null && sessionManagerPrefab!= null)
        {
            Instantiate(sessionManagerPrefab);
        }

        if (MetricsManager.Instance == null && metricsManagerPrefab!= null)
        {
            Instantiate(metricsManagerPrefab);
        }

        // Once managers are ready, load the main menu scene.
        SceneManager.LoadScene("01_MainMenu");
    }
}