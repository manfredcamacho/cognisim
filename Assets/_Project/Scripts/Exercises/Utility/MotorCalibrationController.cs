using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;

/// <summary>
/// Controls the Motor Calibration exercise.
/// This task measures the user's baseline motor reaction and movement speed
/// by having them click/tap on a series of targets.
/// </summary>
public class MotorCalibrationController : MonoBehaviour, IExerciseController
{
    // --- IExerciseController Implementation ---
    public event Action OnExerciseComplete;

    // --- Public Fields (for Unity Inspector assignment) ---
    [SerializeField] private RectTransform spawnArea; // The canvas area where targets can appear.
    [SerializeField] private Button targetPrefab; // The prefab for the target button.
    [SerializeField] private Button startButton; // The button to start the exercise.
    [SerializeField] private TextMeshProUGUI roundCounter;
    [SerializeField] private TextMeshProUGUI timer;

    // --- Private Fields ---
    private ExerciseParameters currentParameters;
    private string exerciseId;
    private int targetsToSpawn;
    private int targetsCompleted = 0;
    private float delayBetweenTargets = 0.25f; // Delay between target spawns
    
    private bool exerciseStarted = false;    
    private float exerciseStartTime;
    private Button currentTargetInstance;
    private float targetSpawnTime;

    private void Update()
    {
        // Update the timer display if the exercise has started
        if (exerciseStarted)
        {
            timer.text = $"{(Time.time - exerciseStartTime):F1} s";
        }
        else
        {
            timer.text = "0.0 s";
        }

            updateTargetsCopleted();

    }

    public void Initialize(string exerciseId, ExerciseParameters parameters)
    {
        currentParameters = parameters;
        targetsToSpawn = currentParameters.numTargets;
        this.exerciseId = exerciseId;
        startButton.onClick.AddListener(StartExercise);
    }

    public void StartExercise()
    {
        // Start the exercise by spawning the first target
        exerciseStarted = true;
        exerciseStartTime = Time.time;
        SpawnNextTarget();
        MetricsManager.Instance.LogEvent(exerciseId, "ExerciseStart");
        startButton.onClick.RemoveAllListeners();
        startButton.gameObject.SetActive(false);
    }

    private void SpawnNextTarget()
    {
        // Calculate a random position within the spawn area
        float halfWidth = spawnArea.rect.width / 2;
        float halfHeight = spawnArea.rect.height / 2;
        
        // Subtract a buffer to ensure the entire target is visible
        float buffer = 100f; 

        float randomX = UnityEngine.Random.Range(-halfWidth + buffer, halfWidth - buffer);
        float randomY = UnityEngine.Random.Range(-halfHeight + buffer, halfHeight - buffer);

        Vector2 spawnPosition = new Vector2(randomX, randomY);

        // Instantiate the target and set its position
        currentTargetInstance = Instantiate(targetPrefab, spawnArea);
        currentTargetInstance.GetComponent<RectTransform>().anchoredPosition = spawnPosition;

        // Add a listener to the button's click event
        currentTargetInstance.onClick.AddListener(OnTargetClicked);

        // Record the spawn time to calculate reaction time later
        targetSpawnTime = Time.time;
        MetricsManager.Instance.LogEvent(exerciseId, "TargetSpawned", spawnPosition, $"Target_{targetsCompleted + 1}");
    }

    private void OnTargetClicked()
    {
        float reactionTime = Time.time - targetSpawnTime;
        targetsCompleted++;

        // Log the successful interaction
        Vector2 clickPosition = currentTargetInstance.GetComponent<RectTransform>().anchoredPosition;
        Debug.Log($"Target clicked at position: {clickPosition}");
        MetricsManager.Instance.LogEvent(exerciseId, "TargetHit", clickPosition, $"Target_{targetsCompleted}, ReactionTime: {reactionTime:F3}s");

        // Clean up the clicked target
        Destroy(currentTargetInstance.gameObject);

        // Check if the exercise is complete
        if (targetsCompleted >= targetsToSpawn)
        {
            EndExercise();
        }
        else
        {
            // Spawn the next one after a short delay
            StartCoroutine(SpawnWithDelay(delayBetweenTargets));
        }
    }

    private IEnumerator SpawnWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnNextTarget();
    }

    private void EndExercise()
    {
        exerciseStarted = false;
        MetricsManager.Instance.LogEvent(exerciseId, "ExerciseEnd");
        Debug.Log("Motor Calibration exercise finished.");
        
        // Notify the SessionController that we are done
        OnExerciseComplete?.Invoke();
    }

    // Helper function to find our own exerciseId
    private int FindMyExerciseIndex()
    {
        for (int i = 0; i < SessionManager.Instance.CurrentSession.exercises.Count; i++)
        {
            // This is a simple check. A more robust system might pass the index during Initialize.
            if (SessionManager.Instance.CurrentSession.exercises[i].parameters.numTargets == currentParameters.numTargets)
            {
                return i;
            }
        }
        return -1;
    }

    private void updateTargetsCopleted()
    {
        roundCounter.text = $"Objetivos completados {targetsCompleted}/{targetsToSpawn}";
    }
}