using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button actionButton;
    [SerializeField] private TextMeshProUGUI buttonTextComponent;

    [Header("Custom Params")]
    [SerializeField] private string title;
    [SerializeField] private string buttonText;
    [SerializeField] private UnityEvent onActionCallback;

    private void Awake()
    {
        // Set title text
        if (titleText != null)
        {
            titleText.text = title;
        }
        // Set button text
        if (buttonTextComponent != null)
        {
            buttonTextComponent.text = buttonText;
        }
        // Set up button callback
        if (actionButton != null)
        {
            actionButton.onClick.AddListener(() =>
            {
                onActionCallback?.Invoke();
                gameObject.SetActive(false);
            });
        }
    }
}