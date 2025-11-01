using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SequencingTaskController : MonoBehaviour, IExerciseController
{
    public event Action OnExerciseComplete;

    // --- Inspector Fields ---
    [SerializeField] private Transform availableCardsContainer;
    [SerializeField] private Transform solutionSlotsContainer;
    [SerializeField] private FeedbackToast feedbackContainer;
    [SerializeField] private Button checkButton;
    [SerializeField] private SequenceCard cardPrefab;
    [SerializeField] private DroppableSlot slotPrefab;
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private TextMeshProUGUI remainingStepsText;

    // --- Private Fields ---
    private string exerciseId;
    private Dictionary<string, Dictionary<int, string>> scenarios = new Dictionary<string, Dictionary<int, string>>();
    private List<DroppableSlot> solutionSlots = new List<DroppableSlot>();
    private Dictionary<int, string> steps;
    private int remainingStepsCounter = 0;
    private bool exerciseStarted = false;
    private float exerciseStartTime;

    void Awake()
    {
        // In a real application, this data would come from a config file or server.
        scenarios.Add("hand_washing", new Dictionary<int, string>
        {
            { 1, "Abrir canilla" },
            { 2, "Mojarse las manos con agua" },
            { 3, "Aplicar jabón" },
            { 4, "Frotar las manos durante unos segundos" },
            { 5, "Enjuagar y secar" },
            { 6, "Cerrar canilla" }
        });
        scenarios.Add("cook_pasta", new Dictionary<int, string>
        {
            {1, "Llenar una olla con agua" },
            {2, "Calentar agua hasta que hierva"},
            {3, "Agregar la pasta"},
            {4, "Cocinar el tiempo indicado"},
            {5, "Escurrir la pasta en un colador"},
        });
    }

    private void Update()
    {
        if (exerciseStarted)
        {
            timer.text = $"{(Time.time - exerciseStartTime):F1} s";
        }
        else
        {
            timer.text = "0.0 s";
        }

        remainingStepsText.text = $"Pasos restantes: {remainingStepsCounter}/{steps.Count}";

        if (remainingStepsCounter == steps.Count)
        {
            checkButton.interactable = true;
        }
        else
        {
            checkButton.interactable = false;
        }

    }

    public void Initialize(string exerciseId, ExerciseParameters parameters)
    {
        this.exerciseId = exerciseId;
        MetricsManager.Instance.LogEvent(this.exerciseId, "ExerciseStart", null, $"Scenario: {parameters.scenario}");

        if (!scenarios.ContainsKey(parameters.scenario))
        {
            Debug.LogError($"Scenario '{parameters.scenario}' not found!");
            EndExercise("Error_ScenarioNotFound");
            return;
        }

        steps = scenarios[parameters.scenario];

        availableCardsContainer.gameObject.SetActive(false);
        solutionSlotsContainer.gameObject.SetActive(false);
        checkButton.gameObject.SetActive(false);

        GenerateSlots(steps.Count);
        GenerateCards(steps);

        checkButton.onClick.AddListener(OnCheckButtonPressed);
        checkButton.interactable = false;
        exerciseStarted = false;
    }

    public void StartExercise()
    {
        availableCardsContainer.gameObject.SetActive(true);
        solutionSlotsContainer.gameObject.SetActive(true);
        checkButton.gameObject.SetActive(true);

        exerciseStarted = true;
        exerciseStartTime = Time.time;
    }

    private void GenerateSlots(int count)
    {
        for (int i = 0; i < count; i++)
        {
            DroppableSlot newSlot = Instantiate(slotPrefab, solutionSlotsContainer);
            newSlot.canReplaceItem = true;
            solutionSlots.Add(newSlot);
        }
    }

    private void GenerateCards(Dictionary<int, string> steps)
    {
        var shuffledSteps = steps.OrderBy(a => Guid.NewGuid()).ToList();

        for (int i = 0; i < shuffledSteps.Count; i++)
        {
            SequenceCard newCard = Instantiate(cardPrefab, availableCardsContainer);
            newCard.stepText.text = shuffledSteps[i].Value;
            newCard.correctIndex = shuffledSteps[i].Key;
            newCard.OnCardDragged += HandleCardDragged;
        }
    }

    private void HandleCardDragged(SequenceCard card, PointerEventData eventData)
    {
        if (eventData.pointerEnter.GetComponent<DroppableSlot>() != null)
        {
            string details = $"Card '{card.stepText.text}' dropped.";
            MetricsManager.Instance.LogEvent(exerciseId, "ItemDrop", eventData.position, details);

            // Update remaining steps counter
            int filledSlots = 0;
            for (int i = 0; i < solutionSlots.Count; i++)
            {
                if (solutionSlots[i].transform.childCount > 0)
                {
                    filledSlots++;
                }
            }

            remainingStepsCounter = filledSlots;
        }
    }

    private void OnCheckButtonPressed()
    {
        bool isCorrect = true;
        string submittedSequence = "";

        for (int i = 0; i < solutionSlots.Count; i++)
        {
            SequenceCard cardInSlot = solutionSlots[i].transform.GetChild(0).GetComponent<SequenceCard>();
            submittedSequence += $"{cardInSlot.correctIndex};";
            if (cardInSlot.correctIndex != i + 1)
            {
                isCorrect = false;
                feedbackContainer.ShowMessage("Secuencia incorrecta. Inténtalo de nuevo.", MessageType.Error);
            }
        }

        MetricsManager.Instance.LogEvent(exerciseId, "SequenceSubmitted", null, $"Correct: {isCorrect}, Sequence: {submittedSequence}");

        if (isCorrect)
        {
            EndExercise("Completed");
        }
        else
        {
            // Provide feedback to the user (e.g., shake the cards, show a red X)
            Debug.Log("Sequence is incorrect. Try again.");
        }
    }

    private void EndExercise(string reason)
    {
        MetricsManager.Instance.LogEvent(exerciseId, "ExerciseEnd", null, $"Reason: {reason}");
        OnExerciseComplete?.Invoke();
    }
}