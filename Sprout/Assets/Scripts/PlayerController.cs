using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform throwPoint; 
    public GameObject seedPrefab; 
    public float throwForce = 10f; 
    public float arcHeight = 5f;  

    private Vector2 movement; 
    private Rigidbody2D rb; 
    private Vector2 facingDirection = Vector2.down; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        MoveInput();
        UpdateThrowPoint();
        ThrowSeed();
        Interact();
    }

    void FixedUpdate()
    {
        MoveCharacter();
    }

    // Get movement input from player
    void MoveInput()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Normalize input to prevent faster diagonal movement
        if (movement != Vector2.zero)
        {
            facingDirection = movement.normalized;
        }
    }

    // Move the player
    void MoveCharacter()
    {
        if (movement == Vector2.zero)
        {
            rb.linearVelocity = Vector2.zero; 
        }
        else
        {
            rb.linearVelocity = movement.normalized * moveSpeed; 
        }
    }

    // Update throw point position based on facing direction
    void UpdateThrowPoint()
    {
        Vector3 offset = Vector3.zero;

        if (facingDirection == Vector2.up)
            offset = new Vector3(0, 0.5f, 0); // Above
        else if (facingDirection == Vector2.down)
            offset = new Vector3(0, -0.5f, 0); // Below
        else if (facingDirection == Vector2.left)
            offset = new Vector3(-0.5f, 0, 0); // Left
        else if (facingDirection == Vector2.right)
            offset = new Vector3(0.5f, 0, 0); // Right

        // Update the throw point's local position
        throwPoint.localPosition = offset;
    }

    // Throw a seed in the direction the player is facing
    void ThrowSeed()
{
    if (Input.GetMouseButtonDown(0)) 
    {
        // Instantiate seed at throw point
        GameObject seed = Instantiate(seedPrefab, throwPoint.position, Quaternion.identity);

        // Set seed's layer to "Seed"
        seed.layer = LayerMask.NameToLayer("Seed");

        Rigidbody2D seedRb = seed.GetComponent<Rigidbody2D>();

        // Calculate throw direction and apply velocity
        Vector2 throwDirection = facingDirection.normalized * throwForce;
        float upwardForce = arcHeight; // Add upward arc. Not necessary maybe
        seedRb.linearVelocity = new Vector2(throwDirection.x, throwDirection.y + upwardForce);
    }
}

    // Handle player interaction (e.g., picking up items, activating objects)
    void Interact()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Interaction logic (will work on it)
            Debug.Log("Interacted!");
        }
    }
}