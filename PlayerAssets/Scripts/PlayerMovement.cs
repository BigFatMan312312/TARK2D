using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// A script for handling player movement, collision response, and a simple shooting mechanic in a top-down 2D game.
/// This script uses Unity's new Input System for player input.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    // Public variables for tweaking movement in the Unity Inspector.
    [Tooltip("The speed at which the player moves.")]
    public float speed = 5f;

    [Tooltip("The force applied when the player collides with an object.")]
    public float pushForce = 10f;

    [Tooltip("A Transform representing the position from which projectiles are fired.")]
    public Transform gunTransform;

    [Tooltip("The maximum distance the raycast can travel.")]
    public float shootingDistance = 20f;

    // Private variables to store component references and input values.
    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Vector2 lookInput;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Used to get the Rigidbody2D component from the GameObject.
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on the GameObject. Please add one.");
        }
        
        // Ensure gravity is disabled for top-down movement.
        if (rb != null)
        {
            rb.gravityScale = 0f;
            // The constraints were preventing manual rotation. We will now handle rotation directly on the Rigidbody2D.
        }
    }

    /// <summary>
    /// This method is automatically called by the new Input System when the move action is triggered.
    /// The action should be a Vector2 type.
    /// </summary>
    /// <param name="value">The input value from the Input System.</param>
    public void OnMove(InputValue value)
    {
        // Read the 2D vector input (e.g., from WASD or a joystick)
        movementInput = value.Get<Vector2>();
    }
    
    /// <summary>
    /// This method is called by the new Input System when the look action is triggered.
    /// It reads the mouse position from the input system.
    /// </summary>
    /// <param name="value">The input value from the Input System.</param>

    /// <summary>
    /// This method is called by the new Input System when the shoot action is triggered.
    /// It performs a Raycast2D from the gun's position.
    /// </summary>
    /// <param name="value">The input value from the Input System.</param>
    public void OnFire(InputValue value)
    {
        // Get the starting position for the raycast. Use the gunTransform if it's assigned.
        Vector2 raycastOrigin = (gunTransform != null) ? (Vector2)gunTransform.position : (Vector2)transform.position;

        // The direction for the raycast is the player's current "up" direction.
        Vector2 raycastDirection = transform.up;

        // Perform the raycast.
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, raycastDirection, shootingDistance);

        // Check if the raycast hit anything.
        if (hit.collider != null)
        {
            // If it hit something, draw a red line to the point of collision.
            Debug.DrawLine(raycastOrigin, hit.point, Color.red, 2f);
            Debug.Log("Raycast hit: " + hit.collider.name);
        }
        else
        {
            // If it didn't hit anything, draw a blue line to the end of the raycast distance.
            Debug.DrawLine(raycastOrigin, raycastOrigin + raycastDirection * shootingDistance, Color.blue, 2f);
            Debug.Log("Raycast did not hit anything.");
        }
    }

    /// <summary>
    /// FixedUpdate is called at a fixed interval and is used for physics calculations.
    /// We apply movement and rotation here to ensure it's consistent regardless of frame rate.
    /// </summary>
    private void FixedUpdate()
    {
        // Calculate the new velocity based on input and speed.
        rb.linearVelocity = movementInput * speed;

        // Rotate the player to face the mouse position.
        Vector3 mousePositionInWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3 directionToMouse = mousePositionInWorld - transform.position;
        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg - 90f;
        
        // Use the Rigidbody2D's rotation property for smooth, physics-aware rotation.
        rb.rotation = angle;
    }

    /// <summary>
    /// This method is called when this object's collider starts touching another collider.
    /// It's used to apply a physics-based push force upon collision.
    /// </summary>
    /// <param name="collision">Information about the collision.</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if there is at least one contact point.
        if (collision.contacts.Length > 0)
        {
            // Get the normal of the contact point. The normal is a vector pointing away from the collision surface.
            Vector2 normal = collision.contacts[0].normal;

            // Apply a force to the Rigidbody2D in the direction of the normal.
            // ForceMode2D.Impulse applies an instant force, which is good for a "push" or "bounce" effect.
            rb.AddForce(normal * pushForce, ForceMode2D.Impulse);
        }
    }
}
