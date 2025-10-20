using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public List<GameObject> selectableObjects;
    public GameObject instructionsHelpPanel;
    public GameObject feedbackPanel;
    public GameObject instructionsDetailsText;
    public Transform playerTransform;
    public Transform spawnPoint;

    [Tooltip("Selecciona la capa en la que se encuentran los objetos interactuables.")]
    public LayerMask interactableLayer;

    private CharacterController playerController;
    private GameObject correctObject;
    private bool isWaitingForSelection = false;
    private bool isHelpPanelActive = false;

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

        // Comprueba si se ha pulsado la tecla 'E'
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            // Solo inicia la corutina si no está ya activa
            if (!isHelpPanelActive)
            {
                StartCoroutine(ShowHelpPanelCoroutine());
            }
        }

        if(totalRounds == 5)
        {
            SceneManager.LoadScene(2);
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
        instructionsHelpPanel.GetComponentInChildren<TextMeshProUGUI>().text = $"Encuentra:\n{details}"; 
        instructionsDetailsText.GetComponentInChildren<TextMeshProUGUI>().text = details;
        isWaitingForSelection = true;
        roundStartTime = Time.time;
    }

    private IEnumerator ShowHelpPanelCoroutine()
    {
        // 1. Marcar como activo y mostrar el panel
        isHelpPanelActive = true;
        instructionsHelpPanel.gameObject.SetActive(true);

        // 2. Pausar la ejecución de ESTA función durante 3 segundos
        yield return new WaitForSeconds(3f);

        // 3. Ocultar el panel y marcar como inactivo
        instructionsHelpPanel.gameObject.SetActive(false);
        isHelpPanelActive = false;
    }

     private void TeleportPlayerToSpawn()
    {
        if (playerTransform == null || spawnPoint == null) return;

        if (playerController != null && playerController.enabled)
        {
            // El método profesional: deshabilitar el controlador, mover y rehabilitar.
            playerController.enabled = false;
            playerTransform.position = spawnPoint.position;
            playerTransform.rotation = spawnPoint.rotation;
            Camera.main.transform.rotation = Quaternion.Euler(0, 0, 0);
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

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, interactableLayer))
        {
            InteractableObject clickedObject = hit.collider.GetComponent<InteractableObject>();
            if (clickedObject == null) return;
            
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