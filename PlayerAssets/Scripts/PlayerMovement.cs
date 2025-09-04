using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// A script for handling player movement and collision response in a top-down 2D game.
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

    // Private variables to store component references and input values.
    private Rigidbody2D rb;
    private Vector2 movementInput;

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
            // Freeze the rotation of the Rigidbody2D to prevent it from spinning on collision.
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
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
    /// FixedUpdate is called at a fixed interval and is used for physics calculations.
    /// We apply movement here to ensure it's consistent regardless of frame rate.
    /// </summary>
    private void FixedUpdate()
    {
        // Calculate the new velocity based on input and speed.
        // We multiply by Time.fixedDeltaTime to make movement frame-rate independent.
        rb.linearVelocity = movementInput * speed;
    }

    float Val = 0;

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
