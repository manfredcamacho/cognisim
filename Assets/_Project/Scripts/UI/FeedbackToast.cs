using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackToast : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Image panelBackground;
    [SerializeField] private float displayDuration = 2.0f;

    private float timer = 0.0f;
    private bool isDisplaying = false;

    void Update()
    {
        if (isDisplaying)
        {
            timer += Time.deltaTime;
            if (timer >= displayDuration)
            {
                HideMessage();
            }
        }
    }

    public void ShowMessage(string message, MessageType type)
    {
        messageText.text = message;
        panelBackground.color = GetColorForMessageType(type);

        isDisplaying = true;
        timer = 0.0f;
        gameObject.SetActive(true);
    }

    private void HideMessage()
    {
        isDisplaying = false;
        gameObject.SetActive(false);
    }

    private Color GetColorForMessageType(MessageType type)
    {
        switch (type)
        {
            case MessageType.Success:
                return new Color(70, 214, 99);
            case MessageType.Error:
                return Color.red;
            case MessageType.Warning:
                return new Color(255, 165, 0);
            case MessageType.Info:
            default:
                return Color.blue;
        }
    }

}

public enum MessageType
{
    Success,
    Error,
    Warning,
    Info
}
