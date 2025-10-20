using UnityEngine;

public class ResetMouse : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //desbloquear mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

   
}
