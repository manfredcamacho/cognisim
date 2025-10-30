using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CancellationTaskController : MonoBehaviour, IExerciseController, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    // --- IExerciseController Implementation ---
    public event Action OnExerciseComplete;

    // --- Public Fields (for Unity Inspector) ---
    [SerializeField] private GridLayoutGroup gridLayout;
    [SerializeField] private ShapeItem ShapeItemPrefab;
    [SerializeField] private RectTransform gridContainer;
    [SerializeField] private Button startButton;
    [SerializeField] private Image targetShapeImage;
    [SerializeField] private TextMeshProUGUI remainingTimeText;
    [SerializeField] private TextMeshProUGUI remainingTargetsText;

    // --- Private Fields ---
    private string exerciseId;
    private int totalTargets;
    private int targetsFound = 0;
    private bool isTrackingPath = false;
    private List<Vector2> currentPath = new List<Vector2>();
    private float durationSeconds;

    // --- Timer and Initialization ---
    private bool exerciseStarted = false;
    private float exerciseStartTime;


    private void Update()
    {
        // Update the timer display if the exercise has started
        if (exerciseStarted)
        {
            float elapsed = Time.time - exerciseStartTime;
            float remaining = Mathf.Max(0, durationSeconds - elapsed);
            remainingTimeText.text = $"{remaining:F1} s";

            remainingTargetsText.text = $"Objetivos restantes: {totalTargets - targetsFound}";
        }
    }

    public void Initialize(string exerciseId, ExerciseParameters parameters)
    {
        this.exerciseId = exerciseId;
        this.targetShapeImage.sprite = Resources.Load<Sprite>($"Sprites/Shapes/{parameters.targetShape}");
        GenerateGrid(parameters.gridSize, parameters.targetShape, parameters.targetColor, parameters.distractorShapes, parameters.distractorsColor);
        this.durationSeconds = parameters.durationSeconds;
        remainingTargetsText.text = $"Objetivos restantes: {totalTargets}";
        this.remainingTimeText.text = $"{(durationSeconds):F1} s";
        this.startButton.onClick.AddListener(StartExercise);

    }

    public void StartExercise()
    {
        this.gridContainer.gameObject.SetActive(true);
        exerciseStarted = true;
        exerciseStartTime = Time.time;
        MetricsManager.Instance.LogEvent(exerciseId, "ExerciseStart");
        startButton.onClick.RemoveAllListeners();
        startButton.gameObject.SetActive(false);

        StartCoroutine(TimerCoroutine(this.durationSeconds));
    }

    private void GenerateGrid(string gridSize, string targetShapeId, string targetColorStr, List<string> distractorShapes, string distractorsColorStr)
    {
        this.gridContainer.gameObject.SetActive(false);        
        string[] dimensions = gridSize.Split('x');
        int rows = int.Parse(dimensions[0]);
        int cols = int.Parse(dimensions[1]);

        Color targetColor = Color.clear;
        ColorUtility.TryParseHtmlString(targetColorStr, out targetColor);
        this.targetShapeImage.color = targetColor;


        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = cols;
        totalTargets = 0;

        // Simple logic to decide what to spawn
        for (int i = 0; i < rows * cols; i++)
        {
            ShapeItem newItem = Instantiate(ShapeItemPrefab, gridContainer);
            newItem.color = Color.clear;

            bool isTarget = (UnityEngine.Random.value > 0.7f); // 30% chance of being a target

            if (isTarget)
            {
                newItem.isTarget = true;
                newItem.shapeId = targetShapeId;
                newItem.color = targetColor;
                totalTargets++;
            }
            else
            {
                newItem.isTarget = false;
                ColorUtility.TryParseHtmlString(distractorsColorStr, out newItem.color);
                if (distractorShapes != null && distractorShapes.Count > 0)
                {
                    //pick random distractor
                    int randomIndex = UnityEngine.Random.Range(0, distractorShapes.Count);
                    newItem.shapeId = distractorShapes[randomIndex];
                }
                else
                {
                    newItem.shapeId = "shape"; // Fallback if no distractors are defined.
                }
            }

            // Load the sprite from Resources
            newItem.itemImage.sprite = Resources.Load<Sprite>($"Sprites/Shapes/{newItem.shapeId}");
            newItem.itemImage.color = newItem.color;

            // Add a listener to the button click
            newItem.itemButton.onClick.AddListener(() => OnShapeClicked(newItem));
        }
    }

    private void OnShapeClicked(ShapeItem item)
    {
        item.itemButton.interactable = false; // Disable after click

        if (item.isTarget)
        {
            targetsFound++;
            MetricsManager.Instance.LogEvent(exerciseId, "CorrectSelection", item.transform.position, item.shapeId);
            item.itemImage.color = Color.green; // Visual feedback

            if (targetsFound >= totalTargets)
            {
                EndExercise("Completed");
            }
        }
        else
        {
            MetricsManager.Instance.LogEvent(exerciseId, "ErrorCommission", item.transform.position, item.shapeId);
            item.itemImage.color = Color.red; // Visual feedback
        }
    }

    // --- Path Tracking Implementation ---
    public void OnPointerDown(PointerEventData eventData)
    {
        isTrackingPath = true;
        currentPath.Clear();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isTrackingPath)
        {
            currentPath.Add(eventData.position);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isTrackingPath)
        {
            isTrackingPath = false;
            if (currentPath.Count > 1) // Only log meaningful paths
            {
                // Serialize the path to a JSON string to store in 'details'
                string pathJson = JsonUtility.ToJson(new PathData(currentPath));
                MetricsManager.Instance.LogEvent(exerciseId, "PathDrawn", null, pathJson);
            }
        }
    }

    // --- Timer and Cleanup ---
    private IEnumerator TimerCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        EndExercise("Timeout");
    }

    private void EndExercise(string reason)
    {
        StopAllCoroutines();
        MetricsManager.Instance.LogEvent(exerciseId, "ExerciseEnd", null, $"Reason: {reason}");
        OnExerciseComplete?.Invoke();
    }
}

// Helper class to serialize the path list

internal class PathData
{
    public List<Vector2> path;
    public PathData(List<Vector2> path) { this.path = path; }
}