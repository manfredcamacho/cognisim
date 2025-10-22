using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopPanelController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image patientPicture;
    [SerializeField] private TextMeshProUGUI patientName;

    public void SetUserInfo(string name, Sprite picture = null)
    {
        if (picture != null)
        {
            patientPicture.sprite = picture;
        }

        if (name != null)
        {
            patientName.text = "Bienvenido, " + name;
        }
    }
}