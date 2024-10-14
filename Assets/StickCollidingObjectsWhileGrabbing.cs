using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.Interaction.Toolkit;

public class StickObjectsOnGrab : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private bool isObjectGrabbed = false;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Subscribe to grab events
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // Set the flag to true when the object is grabbed
        isObjectGrabbed = true;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // Set the flag to false when the object is released
        isObjectGrabbed = false;

        // Optionally remove the FixedJoints on release, if desired
        FixedJoint[] joints = GetComponentsInChildren<FixedJoint>();
        foreach (var joint in joints)
        {
            Destroy(joint);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isObjectGrabbed && collision.rigidbody != null)
        {
            // Add a FixedJoint to the collided object, making it stick to the grabbed object
            FixedJoint joint = collision.gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = GetComponent<Rigidbody>();
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to avoid memory leaks
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }
}