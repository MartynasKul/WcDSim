using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Transform[] waypoints; // Waypoints for the car to follow
    public float speed = 10f;     // Car's movement speed
    public float turnSpeed = 5f;  // Speed of turning along the path
    public float wheelRotationSpeed = 300f; // Rotation speed for the wheels

    // Assign wheel objects in the Inspector
    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public Transform backLeftWheel;
    public Transform backRightWheel;

    private int currentWaypointIndex = 0;

    void Update()
    {
        MoveCarAlongPath();
        RotateWheels();
    }

    private void MoveCarAlongPath()
    {
        if (waypoints.Length == 0) return;

        // Get the current target waypoint
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;

        // Move the car towards the waypoint
        transform.position += direction * speed * Time.deltaTime;

        // Smoothly rotate the car towards the waypoint direction with -90 degree offset on Y-axis
        if (direction != Vector3.zero)
        {
            Quaternion pathRotation = Quaternion.LookRotation(direction);
            Quaternion targetRotation = pathRotation * Quaternion.Euler(-90, 0, 0); // Apply -90 degrees offset on Y
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        // Check if we reached the waypoint
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.5f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // Loop back if needed
        }
    }

    private void RotateWheels()
    {
        // Rotate each wheel based on the car's movement speed
        float rotationAmount = wheelRotationSpeed * Time.deltaTime;

        frontLeftWheel.Rotate(Vector3.right, rotationAmount);
        frontRightWheel.Rotate(Vector3.right, rotationAmount);
        backLeftWheel.Rotate(Vector3.right, rotationAmount);
        backRightWheel.Rotate(Vector3.right, rotationAmount);
    }
}
