using UnityEngine;
using MoreMountains.Feedbacks;
using UnityEngine.Events;
using System.Collections.Generic;
using FMODUnity;
using Unity.Audio;
using Unity.VisualScripting;
using MoreMountains.Tools;
using UnityEngine.Rendering.UI;
using System;
using System.Linq;

public class NewPlayerController : MonoBehaviour, ILaunchable
{
    public UIManager uiManager;

    public float moveSpeed = 5f;
    public float bouncyPadGravity = 5f;
    public Transform throwPoint;
    public Transform objectThrowPoint;
    public GameObject seedPrefab;
    public GameObject bouncyPadSeedPrefab;
    public float throwForce = 10f;
    public float arcHeight = 5f;
    private bool isWalking = false;
    public bool onGround = true;
    public bool snapSeedToGrid = false;

    public bool hasUnlockedVineSeed = true;
    public bool hasUnlockedBouncyPadSeed = true;

    public float timeInPoisonGas = 0f;
    public float poisonGasTimeToKill = 1f;
    public bool isInPoisionGas = false;

    private Vector2 movement;
    private Rigidbody rb;
    public static Vector3 facingDirection = Vector3.down;

    private float timeSinceFalling = 0f;
    private float voidOutTime = 0.5f;

    private GameObject currentIntractedObject;
    private ThrowableBaseClass currentItemBeingCarried;

    public Transform spriteQuad;
    public Texture rightTexture;
    public Texture downTexture;
    public Texture upTexture;

    //launching parameters
    public bool isBeingLaunched = false;
    public bool hasBeenLaunched = false;
    private bool canOverrideLaunch = false;

    private float launchGravity;

    [Header("Feedbacks")]
    /// a MMF_Player to play when player walks
    public MMF_Player WalkingFeedback;
    /// a MMF_Player to play when player throws
    public MMF_Player ThrowingFeedback;
    public MMF_Player RecallingFeedback;
    public MMF_Player InteractingFeedback;



    private List<GrowableSproutBaseClass> plantedSeed = new List<GrowableSproutBaseClass>();
    private float vineButtonHeldDownTimer = 0f;
    private float bouncyPadButtonHeldDownTimer = 0f;

    private Material sproutMaterial;

    [Header("FMOD")]
    [EventRef] public string walkingSFXEventPath;
    private FMOD.Studio.EventInstance walkingSFXInstance;
    private FMOD.Studio.EventInstance realingSFX;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        realingSFX = RuntimeManager.CreateInstance("event:/Sound Effects/Player Sounds/Player Planting");

        sproutMaterial = spriteQuad.gameObject.GetComponent<MeshRenderer>().material;

        // Create the FMOD footstep sound instance
        //walkingSFXInstance = RuntimeManager.CreateInstance(walkingSFXEventPath);
    }

    void Update()
    {
        if (!isBeingLaunched && CheckGround() == true)
        {
            MoveInput();
            UpdateThrowPoint();
            Interact();
            CheckFalling();
            UpdateSpriteDirection();
            
            if(currentItemBeingCarried == null)
            {
                ThrowSeed();
            }
        }
        else
        {
            if(isBeingLaunched)
            {
                HandleBouncyPadGravity();
            }
            else
            {
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            }
        }

        HandlePoisionGas();

        if(Input.GetKeyDown(KeyCode.M))
        {
            uiManager.DisplayControls();
        }
        if(Input.GetKeyUp(KeyCode.M))
        {
            uiManager.StopDisplayingControls();
        }
    }

    void FixedUpdate()
    {
        if (!isBeingLaunched)
        {
            if(CheckGround() == true)
            {
                MoveCharacter();
            }
            else
            {
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            }
        }
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
                //FMODUnity.RuntimeManager.PlayOneShot("event:/Sound Effects/Player Sounds/Player Walking");
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
        //$Player Walking

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
                //FMODUnity.RuntimeManager.PlayOneShot("event:/Sound Effects/Player Sounds/Player Walking");
            }
        }
    }

    // Update throw point position based on facing direction
    void UpdateThrowPoint()
    {
        Vector3 offset = Vector3.right;

        if (facingDirection == Vector3.forward)
            offset = new Vector3(0, 0.25f, 0.5f); // Above
        else if (facingDirection == Vector3.back)
            offset = new Vector3(0, 0.25f, -0.5f); // Below
        else if (facingDirection == Vector3.left)
            offset = new Vector3(-0.5f, 0.25f, 0); // Left
        else if (facingDirection == Vector3.right)
            offset = new Vector3(0.5f, 0.25f, 0); // Right

        // Update the throw point's local position
        throwPoint.localPosition = offset;
    }

    // Throw a seed in the direction the player is facing
    void ThrowSeed()
    {
        if(uiManager.CanPlantSeed())
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.K))
            {
                RaycastHit hit;

                bool didHit = Physics.Raycast(throwPoint.transform.position, Vector3.down, out hit, 2f);

                if (didHit && hit.transform.CompareTag("GrowableGround") && !IsSeedOnPlantLocation(new Vector2(hit.transform.position.x, hit.transform.position.z)))
                {
                    GameObject seed;
                    
                    if((Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.J)) && hasUnlockedVineSeed)
                    {
                        //$Player Planting for vine seed
                        FMODUnity.RuntimeManager.PlayOneShot("event:/Sound Effects/Player Sounds/Player Planting");

                        seed = Instantiate(seedPrefab, throwPoint.position, Quaternion.identity) as GameObject;

                        // Instantiate seed at throw point

                        uiManager.AssignSeedIcon(seed.GetComponent<GrowableSproutBaseClass>());

                        // Set seed's layer to "Seed"
                        seed.layer = LayerMask.NameToLayer("Seed");

                        Rigidbody seedRb = seed.GetComponent<Rigidbody>();

                        // Calculate throw direction and apply velocity
                        Vector2 throwDirection = facingDirection.normalized * throwForce;
                        float upwardForce = arcHeight; // Add upward arc. Not necessary maybe
                                                       //seedRb.linearVelocity = new Vector2(throwDirection.x, throwDirection.y + upwardForce);

                        plantedSeed.Add(seed.GetComponent<GrowableSproutBaseClass>());

                        if(snapSeedToGrid)
                        {
                            seed.transform.position = new Vector3(hit.transform.position.x, seed.transform.position.y, hit.transform.position.z);
                        }
                    }
                    else if(hasUnlockedBouncyPadSeed)
                    {
                        //$Player Planting for bouncy seed
                        FMODUnity.RuntimeManager.PlayOneShot("event:/Sound Effects/Player Sounds/Player Planting");
                        //RecallingFeedback.PlayFeedbacks();
                        seed = Instantiate(bouncyPadSeedPrefab, throwPoint.position, Quaternion.identity) as GameObject;

                        // Instantiate seed at throw point

                        uiManager.AssignSeedIcon(seed.GetComponent<GrowableSproutBaseClass>());

                        // Set seed's layer to "Seed"
                        seed.layer = LayerMask.NameToLayer("Seed");

                        Rigidbody seedRb = seed.GetComponent<Rigidbody>();

                        // Calculate throw direction and apply velocity
                        Vector2 throwDirection = facingDirection.normalized * throwForce;
                        float upwardForce = arcHeight; // Add upward arc. Not necessary maybe
                                                       //seedRb.linearVelocity = new Vector2(throwDirection.x, throwDirection.y + upwardForce);

                        plantedSeed.Add(seed.GetComponent<GrowableSproutBaseClass>());

                        if (snapSeedToGrid)
                        {
                            seed.transform.position = new Vector3(hit.transform.position.x, seed.transform.position.y, hit.transform.position.z);
                        }
                    }
                }
            }
        }

        if(Input.GetMouseButton(0) || Input.GetKey(KeyCode.J))
        {
            vineButtonHeldDownTimer += Time.deltaTime;

            if(vineButtonHeldDownTimer > 1f)
            {
                //$Player Realling Vine Seed
                if (plantedSeed.Count > 0)
                {
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Sound Effects/Player Sounds/Player Recalling");
                    RecallingFeedback.PlayFeedbacks();
                }
                //realingSFX.start();
                foreach (var seed in plantedSeed.ToList())
                {
                    if(seed != null)
                    {
                        if(seed.transform.GetComponent<VineSeed>() != null)
                        {
                            seed.GrowSprout(5f, new Vector2(facingDirection.x, facingDirection.z));
                            seed.transform.GetComponentInChildren<VinePlant>().UnGrow();
                            plantedSeed.Remove(seed);
                        }
                        
                    }
                }
            }
        }

        if(Input.GetMouseButton(1) || Input.GetKey(KeyCode.K))
        {
            bouncyPadButtonHeldDownTimer += Time.deltaTime;

            if (bouncyPadButtonHeldDownTimer > 1f)
            {
                if (plantedSeed.Count > 0)
                {
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Sound Effects/Player Sounds/Player Recalling");
                    RecallingFeedback.PlayFeedbacks();
                }
                //$Player Realling Bouncy Seed
                //FMODUnity.RuntimeManager.PlayOneShot("event:/Sound Effects/Player Sounds/Player Recalling");
                //realingSFX.start();
                //RecallingFeedback.PlayFeedbacks();

                foreach (var seed in plantedSeed.ToList())
                {
                    if (seed != null)
                    {
                        if (seed.transform.GetComponent<BouncyPadSeed>() != null)
                        {
                            seed.GrowSprout(5f, new Vector2(facingDirection.x, facingDirection.z));
                            seed.transform.GetComponentInChildren<BouncyPadPlant>().Ungrow();
                            plantedSeed.Remove(seed);
                        }

                    }
                }
            }
        }

        if(Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.J))
        {
            vineButtonHeldDownTimer = 0f;
        }

        if (Input.GetMouseButtonUp(1) || Input.GetKeyUp(KeyCode.K))
        {
            bouncyPadButtonHeldDownTimer = 0f;
        }

    }

    // Handle player interaction (e.g., picking up items, activating objects)
    void Interact()
    {
        if(currentItemBeingCarried == null)
        {
            if (Input.GetKey(KeyCode.E))
            {
                //TODO: make raycast every 0.1 seconds instead of every frame
                RaycastHit hit;

                GameObject hitObject;

                //check if ray hits
                bool didHit = Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), facingDirection, out hit, 1f);

                if(didHit && (hit.transform.gameObject.GetComponent<IInteractable>() != null || hit.transform.gameObject.GetComponent<ThrowableBaseClass>()))
                {
                    hitObject = hit.transform.gameObject;
                }
                else
                {
                    didHit = IsInteractableOverlap(out hitObject);
                }
            
                //shoot raycast towards the facing direction, getting IInteractable interface from the object hit
                if (hitObject != null && hitObject.GetComponent<IInteractable>() != null)
                {
                    //$Player Interact
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Sound Effects/Player Sounds/Player Interacting");
                    
                    //store object currently interacted wtih
                    currentIntractedObject = hitObject;

                    //call OnStartInteract function from IInteractable interface
                    hitObject.GetComponent<IInteractable>().OnStartInteract();
                    //InteractingFeedback.PlayFeedbacks();
                }
                else if(hitObject != null && hitObject.GetComponent<ThrowableBaseClass>() != null)
                {
                    //$Picking Up
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Sound Effects/Player Sounds/Player Picking Up");
                    currentItemBeingCarried = hitObject.GetComponent<ThrowableBaseClass>();

                    currentItemBeingCarried.PickUp(objectThrowPoint);
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
        else if(currentItemBeingCarried != null)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                //$Player Throwing
                FMODUnity.RuntimeManager.PlayOneShot("event:/Sound Effects/Player Sounds/Player Throwing");
                currentItemBeingCarried.Throw(new Vector2(facingDirection.x, facingDirection.z));
                currentItemBeingCarried = null;

                //if(movement.magnitude > 0)
                //{
                //    currentItemBeingCarried.Throw(new Vector2(facingDirection.x, facingDirection.z));
                //    currentItemBeingCarried = null;
                //}
                //else
                //{
                //    currentItemBeingCarried.Drop();
                //    currentItemBeingCarried = null;
                //}
            }
        }

    }

    private bool IsInteractableOverlap(out GameObject interactable)
    {
        //box overlap everything
        Collider[] hitColliders = Physics.OverlapBox(transform.position + new Vector3(0, 0.5f, 0) + facingDirection * 0.5f, Vector3.one * 0.5f + facingDirection * 0.5f, Quaternion.identity);
        GameObject closestInteractable = null;
        //get the closest plant
        if (hitColliders.Length > 0)
        {
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].gameObject.GetComponent<IInteractable>() == null)
                {
                    continue;
                }
                else
                {
                    if (closestInteractable == null)
                    {
                        closestInteractable = hitColliders[i].gameObject;
                    }
                    else
                    {
                        float playerToCurrentClosest = Vector3.Distance(transform.position, closestInteractable.transform.position);
                        float playerToNextSprout = Vector3.Distance(transform.position, hitColliders[i].transform.position);

                        if (playerToNextSprout < playerToCurrentClosest)
                        {
                            closestInteractable = hitColliders[i].gameObject;
                        }
                    }
                }
            }

            interactable = closestInteractable;
            return true;
        }
        else
        {
            interactable = null;
            return false;
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

    void UpdateSpriteDirection()
    {
        if (facingDirection == Vector3.forward)
        {
            sproutMaterial.mainTexture = upTexture; // Above
            spriteQuad.transform.localScale = new Vector3(1, 1, 1);
            spriteQuad.transform.localPosition = new Vector3(0.25f, spriteQuad.transform.localPosition.y, 0f);
        }
        else if (facingDirection == Vector3.back)
        {
            sproutMaterial.mainTexture = downTexture; // Below
            spriteQuad.transform.localScale = new Vector3(1, 1, 1);
            spriteQuad.transform.localPosition = new Vector3(-0.1f, spriteQuad.transform.localPosition.y, 0f);
        }
        else if (facingDirection == Vector3.left)
        {
            sproutMaterial.mainTexture = rightTexture; // Left
            spriteQuad.transform.localScale = new Vector3(-1, 1, 1);
            spriteQuad.transform.localPosition = new Vector3(0, spriteQuad.transform.localPosition.y, 0f);
        }
        else if (facingDirection == Vector3.right)
        {
            sproutMaterial.mainTexture = rightTexture; // Right
            spriteQuad.transform.localScale = new Vector3(1, 1, 1);
            spriteQuad.transform.localPosition = new Vector3(0, spriteQuad.transform.localPosition.y, 0f);
        }
    }

    bool CheckGround()
    {
        RaycastHit hitUp;
        RaycastHit hitDown;
        RaycastHit hitLeft;
        RaycastHit hitRight;

        bool up = Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0.5f), Vector3.down, out hitUp, 2f);
        bool down = Physics.Raycast(transform.position + new Vector3(0, 0.5f, -0.5f), Vector3.down, out hitDown, 2f);
        bool left = Physics.Raycast(transform.position + new Vector3(-0.5f, 0.5f, 0f), Vector3.down, out hitLeft, 2f);
        bool rigth = Physics.Raycast(transform.position + new Vector3(0.5f, 0.5f, 0f), Vector3.down, out hitRight, 2f);

        if(up || down || left || rigth)
        {
            return true;
        }

        else
        {
            return false;
        }

        //return true;  //Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), Vector3.down, out hit, 2f);
    } 

    private void OnCollisionEnter(Collision collision)
    { 
        if(isBeingLaunched && hasBeenLaunched)
        {
            isBeingLaunched = false;
            rb.useGravity = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PoisonGas>())
        {
            if(other.GetComponent<PoisonGas>().gasIsActve == true)
            {
                isInPoisionGas = true;
            }
            else
            {
                isInPoisionGas = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PoisonGas>())
        {
            isInPoisionGas = false;
        }
    }

    public void Launch(Vector2 direction, float height, float distance, float timeToDestination, Vector3 launchPosition)
    {

        height *= 2;
        rb.useGravity = false;
        isBeingLaunched = true;
        launchGravity = 0f;
        rb.linearVelocity = Vector3.zero;
        transform.position = launchPosition;
        transform.position += Vector3.up * 0.1f;
        hasBeenLaunched = false;
        gameObject.GetComponent<CapsuleCollider>().isTrigger = true;
        StartCoroutine(LaunchDelay(0.1f, direction, height, distance, timeToDestination, launchPosition));
    }

    System.Collections.IEnumerator LaunchDelay(float duration, Vector2 direction, float height, float distance, float timeToDestination, Vector3 launchPosition)
    {
        yield return new WaitForSeconds(duration);

        //$Player Bounce
        FMODUnity.RuntimeManager.PlayOneShot("event:/Sound Effects/Player Sounds/Player Bouncing");
        Debug.Log("Delay");
        hasBeenLaunched = true;
        rb.linearVelocity = new Vector3(direction.x * distance, height, direction.y * distance);
        canOverrideLaunch = false;
        launchGravity = height * 2;
        gameObject.GetComponent<CapsuleCollider>().isTrigger = false;
        Debug.Log(launchGravity);
    }

    private void HandleBouncyPadGravity()
    {
        rb.linearVelocity -= new Vector3(0f, launchGravity, 0f) * Time.deltaTime;
    }

    private void HandlePoisionGas()
    {
        if(isInPoisionGas)
        {
            timeInPoisonGas += Time.deltaTime;
        }
        else
        {
            timeInPoisonGas -= Time.deltaTime;
        }

        timeInPoisonGas = Mathf.Clamp(timeInPoisonGas, 0f, poisonGasTimeToKill);

        if(timeInPoisonGas >= poisonGasTimeToKill)
        {
            Respawn respawnComponent = GetComponent<Respawn>();
            respawnComponent.RespawnPlayer();
            timeInPoisonGas = 0f;
        }
    }

    public void UnlockVineSeeds()
    {
        hasUnlockedVineSeed = true;
    }

    public void UnlockBouncyPadSeed()
    {
        hasUnlockedBouncyPadSeed = true;
    }

    private bool IsSeedOnPlantLocation(Vector2 tileLocation)
    {
        foreach(var seed in plantedSeed)
        {
            if(seed != null)
            {
                Vector2 plantedSeedLocation = new Vector2(seed.transform.position.x, seed.transform.position.z);

                Debug.Log(Vector2.Distance(tileLocation, plantedSeedLocation));

                if(Vector2.Distance(tileLocation, plantedSeedLocation) < 0.01f)
                {
                    Debug.Log("There is seed");

                    return true;
                }
            }
        }

        Debug.Log("There is no seed");

        return false;
    }
}