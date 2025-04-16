using UnityEngine;

public class ResettableObject : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start()
    {
        // Store the initial position & rotation
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    public void ResetState()
    {
        // Reset position & rotation
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        // Add any other reset logic
    }
}