using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // The target to follow (player character)
    public float distance = 5.0f; // Default distance from the target
   

    public float rotationSpeed = 5.0f; // Speed of camera rotation
    public float verticalAngleMin = -20f; // Minimum vertical angle
    public float verticalAngleMax = 60f; // Maximum vertical angle

    private float currentDistance; // Current distance from the target
    private float currentYaw; // Current yaw (horizontal rotation)
    private float currentPitch; // Current pitch (vertical rotation)

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("Camera target is not assigned.");
            return;
        }

        currentDistance = distance;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        HandleInput();
        UpdateCameraPosition();
    }

    private void HandleInput()
    {
        // Get mouse input for rotation
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Rotate camera based on mouse input
        currentYaw += mouseX * rotationSpeed;
        currentPitch -= mouseY * rotationSpeed;

        // Clamp vertical rotation
        currentPitch = Mathf.Clamp(currentPitch, verticalAngleMin, verticalAngleMax);
    }
    private void UpdateCameraPosition()
    {
        // Calculate camera rotation
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);

        // Calculate the desired position of the camera
        Vector3 desiredPosition = target.position - (rotation * Vector3.forward * currentDistance);

        // Set camera position and rotation
        transform.position = desiredPosition;
        transform.LookAt(target);
    }
}
