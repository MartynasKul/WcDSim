using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FixedRotationOnGrab : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private Transform interactorTransform;  // To store the interactor's transform
    private Quaternion initialRotation;  // To store the initial object rotation
    private float yRotationOffset = -90f;  // The offset to apply to the Y axis
    private bool isGrabbed = false;  // To track if the object is grabbed

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.onSelectEntered.AddListener(OnGrab);
        grabInteractable.onSelectExited.AddListener(OnRelease);
    }

    private void OnGrab(XRBaseInteractor interactor)
    {
        // Store the initial rotation when the object is first grabbed
        interactorTransform = interactor.transform;
        initialRotation = transform.rotation;

        isGrabbed = true;
    }

    private void OnRelease(XRBaseInteractor interactor)
    {
        // Stop updating the rotation when released
        isGrabbed = false;
    }

    void Update()
    {
        if (isGrabbed)
        {
            // Only adjust rotation when the object is grabbed
            AdjustRotation();
        }
    }

    private void AdjustRotation()
    {
        // Get the player's current rotation
        Vector3 interactorRotation = interactorTransform.eulerAngles;

        // Apply the offset to the Y-axis and keep X and Z the same as the initial rotation
        float targetYRotation = interactorRotation.y + yRotationOffset;

        // Set the object's rotation with the new Y rotation, while keeping X and Z as they were
        transform.rotation = Quaternion.Euler(interactorRotation.x, interactorRotation.y-90, interactorRotation.z);
    }
}
