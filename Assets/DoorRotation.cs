using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DoorInteraction : MonoBehaviour
{
    public float rotationSpeed = 50f; // Speed of rotation
    private XRBaseInteractor currentInteractor;
    private Vector3 lastInteractorPosition;

    private void Start()
    {
        lastInteractorPosition = Vector3.zero;
    }

    // Called when interaction starts
    public void StartInteraction(XRBaseInteractor interactor)
    {
        Debug.Log("Interaction started with " + interactor.name);
        currentInteractor = interactor;
        lastInteractorPosition = interactor.transform.position;
    }

    public void EndInteraction()
    {
        Debug.Log("Interaction ended.");
        currentInteractor = null;
    }

    private void Update()
    {
        if (currentInteractor != null)
        {
            Debug.Log("Interactor Position: " + currentInteractor.transform.position);
            Vector3 interactorMovement = currentInteractor.transform.position - lastInteractorPosition;
            Debug.Log("Interactor Movement: " + interactorMovement);

            Vector3 doorForward = transform.forward; // Direction the door faces
            Vector3 projectedMovement = Vector3.ProjectOnPlane(interactorMovement, Vector3.up);
            Debug.Log("Projected Movement: " + projectedMovement);

            float rotationDirection = Vector3.Dot(projectedMovement, doorForward);
            Debug.Log("Rotation Direction: " + rotationDirection);

            transform.Rotate(0, rotationDirection * rotationSpeed * Time.deltaTime, 0);

            lastInteractorPosition = currentInteractor.transform.position;
        }
    }
}

