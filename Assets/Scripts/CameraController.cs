using UnityEngine;

// Makes the camera smoothly follow a target
public class CameraController : MonoBehaviour
{
    // Object the camera follows (e.g. player)
    public Transform target;

    // Distance from the target
    public Vector3 offset = new Vector3(0, 3, -6);

    // How fast the camera follows
    public float followSpeed = 8f;

    void LateUpdate()
    {
        // Stop if no target is set
        if (!target) return;

        // Calculate target camera position
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move the camera
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );
    }
}