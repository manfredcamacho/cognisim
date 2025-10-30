using UnityEngine;
using System.IO;
using System;

/// <summary>
/// A persistent Singleton responsible for collecting, storing, and saving all
/// metric events that occur during a therapy session.
/// </summary>
public class MetricsManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    public static MetricsManager Instance { get; private set; }

    private SessionResults currentSessionResults;
    private float sessionStartTime;

    private void Awake()
    {
        // Singleton implementation
        if (Instance!= null && Instance!= this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Starts a new metrics recording session. Clears any previous data.
    /// </summary>
    /// <param name="sessionId">The unique ID for this session.</param>
    /// <param name="patientId">The ID of the patient.</param>
    public void StartNewSession(string sessionId, string patientId)
    {
        currentSessionResults = new SessionResults(sessionId, patientId);
        sessionStartTime = Time.time;
        currentSessionResults.sessionStartTime = DateTime.UtcNow.ToString("o"); // ISO 8601 format
        Debug.Log($"MetricsManager: New session started for patient '{patientId}' with session ID '{sessionId}'.");
    }

    /// <summary>
    /// Logs a specific event. This method is called by exercise controllers.
    /// </summary>
    /// <param name="exerciseId">The ID of the exercise logging the event.</param>
    /// <param name="eventType">A string describing the event (e.g., "CorrectSelection").</param>
    /// <param name="position">The screen position of the interaction (optional).</param>
    /// <param name="details">Any additional string data (optional).</param>
    public void LogEvent(string exerciseId, string eventType, Vector2? position = null, string details = "")
    {
        if (currentSessionResults == null)
        {
            Debug.LogWarning("MetricsManager: Tried to log an event, but no session is active.");
            return;
        }

        float timestamp = Time.time - sessionStartTime;
        MetricEvent newEvent = new MetricEvent(timestamp, exerciseId, eventType, position, details);
        currentSessionResults.events.Add(newEvent);
    }

    /// <summary>
    /// Finalizes the session and saves all collected metrics to a local JSON file.
    /// </summary>
    public void EndSessionAndSave()
    {
        if (currentSessionResults == null)
        {
            Debug.LogWarning("MetricsManager: Tried to end session, but no session was active.");
            return;
        }

        currentSessionResults.sessionEndTime = DateTime.UtcNow.ToString("o");

        string json = JsonUtility.ToJson(currentSessionResults, true); // 'true' for pretty printing
        string fileName = $"Results_{currentSessionResults.patientId}_{currentSessionResults.sessionId}_{DateTime.Now.ToString("yyyyMMdd_hhmmss")}.json";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        try
        {
            File.WriteAllText(filePath, json);
            Debug.Log($"MetricsManager: Session results saved successfully to: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"MetricsManager: Failed to save session results. Error: {e.Message}");
        }

        // Clear the current session data after saving
        currentSessionResults = null;
    }
}