
using UnityEngine;
using UnityEngine.Events;

public class LaserReceiver : MonoBehaviour
{
    [Header("Receiver")]
    public Renderer targetRenderer;
    public Color inactiveColor = Color.gray;
    public Color activeColor = Color.green;

    public UnityEvent onReceiverActivated;

    private bool activated;

    private void Start()
    {
        SetColor(inactiveColor);
    }

    public void ActivateReceiver()
    {
        if (activated)
            return;

        activated = true;
        SetColor(activeColor);

        Debug.Log("LASER PUZZLE SOLVED!");

        onReceiverActivated.Invoke();
    }

    private void SetColor(Color color)
    {
        if (targetRenderer != null)
            targetRenderer.material.color = color;
    }
}