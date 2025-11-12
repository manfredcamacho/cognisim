using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoMenuManager : MonoBehaviour
{
    public void StartGame() { 
        SceneManager.LoadScene("Demo");
    }

    public void GoBackMenu()
    {
        SceneManager.LoadScene("Demo_Menu");
    }
}
