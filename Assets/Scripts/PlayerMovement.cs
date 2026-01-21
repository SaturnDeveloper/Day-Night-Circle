using UnityEngine;
using UnityEngine.InputSystem;

// Simple player movement using Unity's Input System
public class PlayerMovement : MonoBehaviour
{
    // Movement speed
    public float speed = 5f;

    // current movement input
    private Vector2 moveInput;

    // Called by the Input System when movement input changes
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        // Convert 2D input into 3D movement (x = horizontal, z = vertical)
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);

        // Move the player
        transform.Translate(move * speed * Time.deltaTime);
    }
}
