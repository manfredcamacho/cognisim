using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI instructionsText;
    [SerializeField] private TextMeshProUGUI textButton;
    [SerializeField] private Button actionButton;

    [Header("Custom Params")]
    [SerializeField] private string title;
    [SerializeField] private string instructions;
    [SerializeField] private string buttonText;
    [SerializeField] private UnityEvent onActionCallback;

    private void Awake()
    {
        // Set title text
        if (titleText != null)
        {
            titleText.text = title;
        }

        // Set title text
        if (instructionsText != null)
        {
            instructionsText.text = instructions;
        }

        // Set button text
        if (textButton != null)
        {
            textButton.text = buttonText;
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


    public void Show(string title, Sprite icon, Action callback)
    {
        this.titleText.text = title;
        this.iconImage.sprite = icon;    
        this.onActionCallback.AddListener(() => callback());
    }
}