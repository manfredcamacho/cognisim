using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Orchestrates the flow of a therapy session.
/// It is responsible for instantiating, managing and destroying the prefabs of the exercises
/// in the order specified by the SessionManager.
/// </summary>
public class SessionController : MonoBehaviour
{
   [SerializeField] private Transform exerciseContainer;

    private int currentExerciseIndex = 0;
    private GameObject currentExerciseInstance = null;

    private void Start()
    {
        if (SessionManager.Instance == null || SessionManager.Instance.CurrentSession == null)
        {
            Debug.LogError("SessionController: No session loaded. Returning to the main menu.");
            
            SceneManager.LoadScene("01_MainMenu");
        }

        StartNextExercise();
    }

    private void StartNextExercise()
    {
        if (currentExerciseIndex < SessionManager.Instance.CurrentSession.exercises.Count)
        {
            ExerciseData exerciseData = SessionManager.Instance.CurrentSession.exercises[currentExerciseIndex];
            string exerciseId = exerciseData.exerciseId;

            // Build the path to the prefab within the Resources folder.
            string prefabPath = $"Prefabs/Exercises/{exerciseId}";
            GameObject exercisePrefab = Resources.Load<GameObject>(prefabPath);

            if (exercisePrefab == null)
            {
                Debug.LogError($"Failed to find the exercise prefab at the path: '{prefabPath}'. Please check that the prefab exists and that the 'exerciseId' in the JSON is correct.");
                currentExerciseIndex++;
                StartNextExercise();
                return;
            }

            currentExerciseInstance = Instantiate(exercisePrefab, exerciseContainer);

            IExerciseController exerciseController = currentExerciseInstance.GetComponent<IExerciseController>();

            if (exerciseController == null)
            {
                Debug.LogError($"The prefab '{exerciseId}' does not have a script that implements the IExerciseController interface.");
                return;
            }

            // Subscribe to the completion event and initialize it.
            exerciseController.OnExerciseComplete += HandleExerciseCompleted;
            exerciseController.Initialize(exerciseData.parameters);
        }
        else
        {
            // If there are no more exercises, end the session.
            EndSession();
        }
    }

    private void HandleExerciseCompleted()
    {
        Debug.Log($"Exercise '{SessionManager.Instance.CurrentSession.exercises[currentExerciseIndex].exerciseId}' completed.");

        // Clean up: unsubscribe from the event and destroy the exercise object.
        if (currentExerciseInstance!= null)
        {
            IExerciseController exerciseController = currentExerciseInstance.GetComponent<IExerciseController>();
            if (exerciseController!= null)
            {
                exerciseController.OnExerciseComplete -= HandleExerciseCompleted;
            }
            Destroy(currentExerciseInstance);
        }

        // Go to the next exercise.
        currentExerciseIndex++;
        StartNextExercise();
    }

    private void EndSession()
    {
        Debug.Log("Session completed! All exercises have finished.");
        
        // We notify the MetricsManager to finalize and save all the collected data.
        if (MetricsManager.Instance!= null)
        {
            MetricsManager.Instance.EndSessionAndSave();
        }

        // In the future, we would also show a "Session Complete" screen to the user here,
        // and then perhaps navigate back to the main menu.
    }
}