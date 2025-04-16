using UnityEngine;
using MoreMountains.Feedbacks;
using UnityEngine.Events;

public class NewPlayerController1 : MonoBehaviour
{
    public UIManager uiManager;

    public float moveSpeed = 5f;
    public Transform throwPoint;
    public GameObject seedPrefab;
    public float throwForce = 10f;
    public float arcHeight = 5f;

    private Vector2 movement;
    private Rigidbody rb;
    public static Vector3 facingDirection = Vector3.down;
    private bool isWalking = false;

    private float timeSinceFalling = 0f;
    private float voidOutTime = 0.5f;

    private GameObject currentIntractedObject;

    [Header("Feedbacks")]
    /// a MMF_Player to play when player walks
    public MMF_Player WalkingFeedback;
    /// a MMF_Player to play when player
    public MMF_Player ThrowingFeedback;
    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    void Update()
    {
        MoveInput();
        UpdateThrowPoint();
        ThrowSeed();
        Interact();
        CheckFalling();
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
            facingDirection = new Vector3(movement.x, 0f, movement.y).normalized;

            // Start feedback only if it's not already playing
            if (!WalkingFeedback.IsPlaying)
            {
                WalkingFeedback.PlayFeedbacks();
            }

            isWalking = true;
        }
        else if (isWalking) 
        {
            // Stop feedback when player stops moving
            WalkingFeedback?.StopFeedbacks();
            isWalking = false;
        }
    }


    // Move the player
    void MoveCharacter()
    {
        if (movement == Vector2.zero)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
        else
        {
            Vector2 normalizedMovement = movement.normalized;
            rb.linearVelocity = new Vector3(normalizedMovement.x * moveSpeed, rb.linearVelocity.y, normalizedMovement.y * moveSpeed);
            
            // Play walking feedback only when movement input is detected
            if (WalkingFeedback != null && !WalkingFeedback.IsPlaying)
            {
                WalkingFeedback.PlayFeedbacks();
            }
        }
    }


    // Update throw point position based on facing direction
    void UpdateThrowPoint()
    {
        Vector3 offset = Vector3.zero;

        if (facingDirection == Vector3.forward)
            offset = new Vector3(0, 0, 0.5f); // Above
        else if (facingDirection == Vector3.back)
            offset = new Vector3(0, 0, -0.5f); // Below
        else if (facingDirection == Vector3.left)
            offset = new Vector3(-0.5f, 0, 0); // Left
        else if (facingDirection == Vector3.right)
            offset = new Vector3(0.5f, 0, 0); // Right

        // Update the throw point's local position
        throwPoint.localPosition = offset;
    }

    // Throw a seed in the direction the player is facing
    void ThrowSeed()
    {
        if (uiManager.currentSeeds > 0)
{
    if (Input.GetMouseButtonDown(0))
    {
        // Instantiate seed at throw point
        GameObject seed = Instantiate(seedPrefab, throwPoint.position, Quaternion.identity);

        // Set seed's layer to "Seed"
        seed.layer = LayerMask.NameToLayer("Seed");

        Rigidbody seedRb = seed.GetComponent<Rigidbody>();

        // Calculate throw direction and apply velocity
        Vector2 throwDirection = facingDirection.normalized * throwForce;
        float upwardForce = arcHeight;
        seedRb.linearVelocity = new Vector2(throwDirection.x, throwDirection.y + upwardForce);

        // Play throwing feedback only when a seed is actually thrown
        if (ThrowingFeedback != null)
        {
            ThrowingFeedback?.PlayFeedbacks();
        }

        //uiManager.UseSeed();
    }
}

    }

    // Handle player interaction (e.g., picking up items, activating objects)
    void Interact()
    {
        if (Input.GetKey(KeyCode.E))
        {
            //TODO: make raycast every 0.1 seconds instead of every frame
            RaycastHit hit;

            //check if ray hits
            bool didHit = Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), facingDirection, out hit, 1f);
            
            //shoot raycast towards the facing direction, getting IInteractable interface from the object hit
            if (didHit && hit.transform.gameObject.GetComponent<IInteractable>() != null)
            {
                //store object currently interacted wtih
                currentIntractedObject = hit.transform.gameObject;

                //call OnStartInteract function from IInteractable interface
                hit.transform.gameObject.GetComponent<IInteractable>().OnStartInteract();
            }
            else
            {
                //if raycast didn't hit anything
                if(currentIntractedObject != null)
                {
                    //call OnStopInteraction function from IInteractable interface
                    currentIntractedObject.GetComponent<IInteractable>().OnStopInteract();
                    currentIntractedObject = null;
                }
            }
        }

        //if the interact key was let go
        if(Input.GetKeyUp(KeyCode.E))
        {
            if (currentIntractedObject != null)
            {
                currentIntractedObject.GetComponent<IInteractable>().OnStopInteract();
                currentIntractedObject = null; 
            }
        }
    }

    // Handle player falling and voiding out
    void CheckFalling()
    {
        RaycastHit hit;

        //if there is no ground and player is falling
        if (!Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity) && rb.linearVelocity.y < -1f)
        {
            timeSinceFalling += Time.deltaTime;

            if(timeSinceFalling > voidOutTime)
            {
                //TODO: Add void out
            }
        }
        else
        {
            timeSinceFalling = 0f;
        }
    }
}