using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class ActivateTeleportationRay : MonoBehaviour
{
    public GameObject rightTeleportationRay;
    public GameObject leftTeleportationRay;

    public InputActionProperty leftActivate;
    public InputActionProperty rightActivate;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        leftTeleportationRay.SetActive(leftActivate.action.ReadValue<float>() > 0.1f);
        rightTeleportationRay.SetActive(rightActivate.action.ReadValue<float>() > 0.1f);
    }
}
