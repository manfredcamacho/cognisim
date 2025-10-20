using UnityEngine;
using System.IO;

/// <summary>
/// Manages the session data for a specific patient.
/// </summary>
public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance { get; private set; }
    public SessionData CurrentSession { get; private set; }

    private void Awake()
    {
        if (Instance!= null && Instance!= this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Loads the session data for a specific patient from a JSON file.
    /// </summary>
    /// <param name="patientId">The identifier of the patient, used to find the configuration file.</param>
    /// <returns>True if the session was loaded correctly, False otherwise.</returns>
    public bool LoadSession(string patientId)
    {
        // TODO: In the future, this section will be replaced by a call to a web API.
        // The API logic will fill the 'CurrentSession' object in the same way.

        // Build the relative path within the Resources folder.
        // We don't include the .json extension, because Resources.Load doesn't need it.
        string filePath = Path.Combine("SessionConfigs", patientId);

        TextAsset textAsset = Resources.Load<TextAsset>(filePath);

        if (textAsset == null)
        {
            Debug.LogError($" Error: Session configuration file not found in 'Resources/{filePath}.json'. Make sure the file exists and the patientId is correct.");

            return false;
        }

        try
        {
            // Use JsonUtility to convert the text of the file to our data classes.
            CurrentSession = JsonUtility.FromJson<SessionData>(textAsset.text);
            
            if (CurrentSession!= null && CurrentSession.exercises!= null)
            {
                Debug.Log($" Session loaded successfully for patient '{CurrentSession.patientId}'. Number of exercises: {CurrentSession.exercises.Count}");
                return true;
            }
            else
            {
                Debug.LogError($" The JSON file in '{filePath}' is empty or malformed.");
                return false;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($" Failed to parse the JSON of the session in '{filePath}'. Error: {e.Message}");
            CurrentSession = null;
            return false;
        }
    }
}
