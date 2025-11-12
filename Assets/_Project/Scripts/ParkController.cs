using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ParkController : MonoBehaviour
{
    public List<GameObject> selectableObjects;
    public FeedbackToast feedbackToast;
    public GameObject instructionsDetailsText;
    public Transform playerTransform;
    public Transform spawnPoint;

    public TextMeshProUGUI timeText;
    public TextMeshProUGUI roundsText;

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

    private bool isCursorLocked = false;

    // Usamos { get; private set; } para que otros scripts puedan LEER pero no MODIFICAR.
    public int FinalTotalRounds { get; private set; }
    public int FinalTotalAttempts { get; private set; }
    public float FinalAccuracy { get; private set; }
    public float FinalAverageReactionTime { get; private set; }
    public float FinalTotalDuration { get; private set; }
    // --- Fin de las nuevas variables ---

    public static ParkController Instance { get; private set; }

    void Awake()
    {
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


        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        
        }
}

    void Start()
    {
        sessionStartTime = Time.time;
        StartNewRound();
        LockCursor(); // <-- AÑADIDO: Bloquea el cursor al inicio
    }

    void Update()
    {
        // --- NUEVA LÓGICA DE GESTIÓN DEL CURSOR ---

        // 1. Si presionamos Escape, liberamos el cursor
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            UnlockCursor();
        }
        // 2. Si el cursor está libre Y hacemos click (y no es sobre UI), lo bloqueamos
        else if (!isCursorLocked && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Evita bloquear el cursor si estamos haciendo clic en un botón de la UI
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                LockCursor();
            }
        }

        // --- LÓGICA DE CLICK EXISTENTE (MODIFICADA) ---

        // Solo procesamos el click del juego SI el cursor ESTÁ BLOQUEADO
        if (isCursorLocked && isWaitingForSelection && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) // <-- MODIFICADO
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            ProcessClickSelection();
        }


        timeText.text = $"{Time.time - sessionStartTime:F1} s";

        Debug.Log($"Total Rounds: {totalRounds}");

        if (totalRounds > 5)
        {
            ShowSessionSummary();

            // 2. Cargamos la escena de resultados
            // ¡¡IMPORTANTE!! Cambia "TuEscenaDeResultados" por el nombre real de tu escena.
            SceneManager.LoadScene("Demo_results");
        }
        else
        {
            roundsText.text = $"{totalRounds}/5";
        }
    }

    // --- MÉTODOS AUXILIARES AÑADIDOS ---

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isCursorLocked = true;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isCursorLocked = false;
    }

    // --- RESTO DE TUS MÉTODOS (SIN CAMBIOS) ---

    public void StartNewRound()
    {
        TeleportPlayerToSpawn();
        totalRounds++;

        if (selectableObjects.Count == 0)
        {
            return;
        }

        correctObject = selectableObjects[Random.Range(0, selectableObjects.Count)];
        InteractableObject interactableObject = correctObject.GetComponent<InteractableObject>();
        string details = $"Busca <b>{interactableObject.objectData.displayName}</b> de color <b>{interactableObject.instanceColor}</b> cerca <b>{interactableObject.instanceLocation}</b>";
        instructionsDetailsText.GetComponentInChildren<TextMeshProUGUI>().text = details;
        isWaitingForSelection = true;
        roundStartTime = Time.time;
    }

    private IEnumerator ShowHelpPanelCoroutine()
    {
        isHelpPanelActive = true;
        yield return new WaitForSeconds(3f);
        isHelpPanelActive = false;
    }

    private void TeleportPlayerToSpawn()
    {
        if (playerTransform == null || spawnPoint == null) return;

        if (playerController != null && playerController.enabled)
        {
            playerController.enabled = false;
            playerTransform.position = spawnPoint.position;
            playerTransform.rotation = spawnPoint.rotation;
            Camera.main.transform.rotation = Quaternion.Euler(0, 0, 0);
            playerController.enabled = true;
        }
        else
        {
            playerTransform.position = spawnPoint.position;
        }
    }

    public void StartNewAttempt()
    {
        isWaitingForSelection = true;
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
                feedbackToast.ShowMessage("¡Correcto! Empieza la siguiente ronda.", MessageType.Success);
                Invoke("StartNewRound", 2f);
            }
            else
            {
                feedbackToast.ShowMessage($"Eso no es lo que buscamos. Sigue intentándolo.", MessageType.Error);
                Invoke("StartNewAttempt", 2f);
            }
        }
    }

    private void ShowSessionSummary()
    {
        // Esta comprobación ahora es más importante
        if (totalRounds <= 1 && totalAttempts == 0) return;

        // Calculamos los valores
        int completedRounds = totalRounds - 1; // Si totalRounds es 6 (porque > 5), las rondas completas son 5
        float totalDuration = Time.time - sessionStartTime;
        float accuracy = 0f;
        if (totalAttempts > 0)
        {
            accuracy = ((float)completedRounds / totalAttempts) * 100f;
        }

        float averageReactionTime = 0f;
        if (completedRounds > 0)
        {
            // Usamos 'completedRounds' (5) en lugar de 'totalRounds' (6) para la media
            averageReactionTime = totalReactionTime / completedRounds;
        }

        // --- ¡NUEVO! Guardamos los datos en las variables públicas ---
        FinalTotalRounds = completedRounds;
        FinalTotalAttempts = totalAttempts - 1;
        FinalAccuracy = accuracy;
        FinalAverageReactionTime = averageReactionTime;
        FinalTotalDuration = totalDuration;

        // (Opcional) Puedes dejar los logs si quieres para depurar
        Debug.Log("--- RESUMEN DE LA SESIÓN (GUARDADO) ---");
        Debug.Log($"Rondas: {FinalTotalRounds}");
        Debug.Log($"Precisión: {FinalAccuracy:F1}%");
        Debug.Log($"Tiempo de Reacción Promedio: {FinalAverageReactionTime:F2} segundos");
        Debug.Log($"Duración Total: {FinalTotalDuration:F1} segundos");
        Debug.Log("-------------------------------------");
    }
}