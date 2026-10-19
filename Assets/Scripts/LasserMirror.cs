
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class LaserMirror : MonoBehaviour
{
    public float rotationStep = 15f;
    public Vector3 localRotationAxis = Vector3.up;

    private XRSimpleInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();

        if (interactable == null)
            interactable = gameObject.AddComponent<XRSimpleInteractable>();

        interactable.selectEntered.AddListener(OnSelected);
    }

    private void OnDestroy()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnSelected);
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        transform.Rotate(
            localRotationAxis.normalized,
            rotationStep,
            Space.Self
        );

        Debug.Log("MIRROR ROTATED: " + gameObject.name);
    }
}