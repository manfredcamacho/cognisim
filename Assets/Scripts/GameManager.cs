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
    public GameObject instructionPanel;
    public GameObject feedbackPanel;

    private GameObject correctObject;
    private bool isWaitingForSelection = false;

    private float sessionStartTime;
    private float roundStartTime;
    private int totalRounds = 0;
    private int correctRounds = 0;
    private float totalReactionTime = 0f;

    void Start()
    {
        feedbackPanel.gameObject.SetActive(false);
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
        feedbackPanel.gameObject.SetActive(false);

        if (selectableObjects.Count == 0)
        {
            instructionPanel.GetComponentInChildren<TextMeshProUGUI>().text = "Error: No hay objetos asignados.";
            return;
        }

        correctObject = selectableObjects[Random.Range(0, selectableObjects.Count)];
        instructionPanel.GetComponentInChildren<TextMeshProUGUI>().text = $"Encuentra: {correctObject.name}";
        isWaitingForSelection = true;
        roundStartTime = Time.time;
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
            totalRounds++;

            if (hit.collider.gameObject == correctObject)
            {
                correctRounds++;
                ShowFeedback("¡Correcto!", Color.green);
            }
            else
            {
                ShowFeedback($"Eso no es lo que buscamos. Sigue intentándolo.", Color.red);
            }

            Invoke("StartNewRound", 2f);
        }
    }

    private void ShowFeedback(string message, Color color)
    {
        if (color == Color.green)
        {
            feedbackPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 50); 
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
        float accuracy = ((float)correctRounds / totalRounds) * 100f;
        float averageReactionTime = totalReactionTime / totalRounds;

        Debug.Log("--- RESUMEN DE LA SESIÓN ---");
        Debug.Log($"Duración Total: {totalDuration:F2} segundos");
        Debug.Log($"Rondas Jugadas: {totalRounds}");
        Debug.Log($"Precisión: {accuracy:F1}% ({correctRounds} de {totalRounds})");
        Debug.Log($"Tiempo de Reacción Promedio: {averageReactionTime:F2} segundos");
        Debug.Log("---------------------------");
    }
}