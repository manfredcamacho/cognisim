using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackToast : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Image panelBackground;
    [SerializeField] private float displayDuration = 2.0f;
    [SerializeField] private Image iconImage;

    private float timer = 0.0f;
    private bool isDisplaying = false;

    private void Start()
    {
        iconImage.sprite = Resources.Load<Sprite>($"Icons/Info");
    }

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
        Color color = GetColorForMessageType(type);
        panelBackground.color = color;
        iconImage.sprite = Resources.Load<Sprite>($"Icons/{type.ToString()}");
        iconImage.color = color;

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
                return new Color(70f / 255f, 214f / 255f, 99f / 255f);
            case MessageType.Error:
                return new Color(247f / 255f, 72f / 255f, 80f / 255f);
            case MessageType.Warning:
                return new Color(255f / 255f, 165f / 255f, 0f / 255f);
            case MessageType.Info:
            default:
                return new Color(0f, 122f / 255f, 255f / 255f);
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
