using UnityEngine;

public class DisplayController : MonoBehaviour
{
    public GameObject display;

    public void ShowDisplay()
    {
        Debug.Log("SHOW DISPLAY CLICKED");

        display.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}