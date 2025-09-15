using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<GameObject> selectableObjects;
    public GameObject instructionsHelpPanel;
    public GameObject feedbackPanel;
    public GameObject instructionsDetailsText;
    public Transform playerTransform;
    public Transform spawnPoint;

    private CharacterController playerController;
    private GameObject correctObject;
    private bool isWaitingForSelection = false;

    private float sessionStartTime;
    private float roundStartTime;
    private int totalRounds = 0;
    private int totalAttempts = 0;
    private int correctRounds = 0;
    private float totalReactionTime = 0f;

    void Awake(){
        selectableObjects = new List<GameObject>();
        foreach (Transform child in GameObject.FindObjectsOfType<Transform>())
        {
            if (child.GetComponent<InteractableObject>() != null)
            {
                selectableObjects.Add(child.gameObject);
            }
        }

        if (playerTransform != null)
        {
            playerController = playerTransform.GetComponent<CharacterController>();
        }
    }

    void Start()
    {
        feedbackPanel.gameObject.SetActive(false);
        instructionsHelpPanel.gameObject.SetActive(false);
        sessionStartTime = Time.time;
        StartNewRound();
    }

    void Update()
    {
        if (isWaitingForSelection && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            ProcessClickSelection();
        }
    }

    void OnDestroy()
    {
        ShowSessionSummary();
    }

    public void StartNewRound()
    {
        TeleportPlayerToSpawn();
        feedbackPanel.gameObject.SetActive(false);
        totalRounds++;

        if (selectableObjects.Count == 0)
        {
            instructionsHelpPanel.GetComponentInChildren<TextMeshProUGUI>().text = "Error: No hay objetos asignados.";
            return;
        }

        correctObject = selectableObjects[Random.Range(0, selectableObjects.Count)];
        InteractableObject interactableObject = correctObject.GetComponent<InteractableObject>();
        string details = $"Nombre: {interactableObject.objectData.displayName }\nColores: {interactableObject.instanceColor}\nUbicación: {interactableObject.instanceLocation}";
        instructionsHelpPanel.GetComponentInChildren<TextMeshProUGUI>().text = $"Encuentra: {details.Replace("\n", ", ")}"; 
        instructionsDetailsText.GetComponentInChildren<TextMeshProUGUI>().text = details;
        isWaitingForSelection = true;
        roundStartTime = Time.time;
    }

     private void TeleportPlayerToSpawn()
    {
        if (playerTransform == null || spawnPoint == null) return;

        if (playerController != null && playerController.enabled)
        {
            // El método profesional: deshabilitar el controlador, mover y rehabilitar.
            playerController.enabled = false;
            playerTransform.position = spawnPoint.position;
            playerController.enabled = true;
        }
        else
        {
            // Si no hay CharacterController, el movimiento directo es seguro.
            playerTransform.position = spawnPoint.position;
        }
    }

    public void StartNewAttempt()
    {
        isWaitingForSelection = true;
        feedbackPanel.gameObject.SetActive(false);
    }

    private void ProcessClickSelection()
    {
        if (Camera.main == null)
        {
            Debug.LogError("CRITICAL ERROR! Main Camera not found. Ensure your camera is tagged as 'MainCamera'.");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            isWaitingForSelection = false;

            float currentRoundReactionTime = Time.time - roundStartTime;
            totalReactionTime += currentRoundReactionTime;
            totalAttempts++;

            if (hit.collider.gameObject == correctObject)
            {
                correctRounds++;
                ShowFeedback("¡Correcto! Empieza la siguiente ronda.", Color.green);
                Invoke("StartNewRound", 2f);
            }
            else
            {
                ShowFeedback($"Eso no es lo que buscamos. Sigue intentándolo.", Color.red);
                Invoke("StartNewAttempt", 2f);
            }
        }
    }

    private void ShowFeedback(string message, Color color)
    {
        if (color == Color.green)
        {
            feedbackPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 50); 
            feedbackPanel.GetComponent<Image>().color = new Color(0.294f, 0.706f, 0.263f, 1f);
        }
        else
        {
            feedbackPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 50); 
            feedbackPanel.GetComponent<Image>().color = new Color(0.961f, 0.243f, 0.208f, 1f);
        }
        feedbackPanel.GetComponentInChildren<TextMeshProUGUI>().text = message;
        feedbackPanel.gameObject.SetActive(true);
    }

    private void ShowSessionSummary()
    {
        if (totalRounds == 0) return;

        float totalDuration = Time.time - sessionStartTime;
        float accuracy = ((float)(totalRounds - 1) / totalAttempts) * 100f;
        float averageReactionTime = totalReactionTime / totalRounds;


        Debug.Log("--- RESUMEN DE LA SESIÓN ---");
        Debug.Log($"Rondas: {totalRounds - 1}");
        Debug.Log($"Intentos: {totalAttempts}");
        Debug.Log($"Precisión: {accuracy:F1}% ({totalAttempts} de {totalRounds - 1})");
        Debug.Log($"Tiempo de Reacción Promedio: {averageReactionTime:F2} segundos");
        Debug.Log("---------------------------");
    }
}