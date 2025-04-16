using UnityEngine;
using FMODUnity;
public class ThrowableBaseClass : MonoBehaviour, ILaunchable
{
    public delegate void OnPickUp();
    public event OnPickUp onPickUp;

    public delegate void OnThrow();
    public event OnThrow onThrow;

    public delegate void OnDrop();
    public event OnDrop onDrop;

    public delegate void OnHitAfterThrown();
    public event OnHitAfterThrown onHitAfterThrown;

    public delegate void OnHitAfterDropped();
    public event OnHitAfterDropped onHitAfterDropped;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private SphereCollider sphereCollider;
    [SerializeField] private BoxCollider boxTrigger;

    public float distance = 6f;
    public float gravity = 18f;

    public float launchGravity;
    public bool wasLaunched = false;

    private bool wasThrown;
    private bool wasDropped;

    private bool currentlyBeingCarried;
    private Transform objectToFollow;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(rb == null)
        {
            if(gameObject.GetComponent<Rigidbody>() != null)
            {
                rb = gameObject.GetComponent<Rigidbody>();
            }
            else
            {
                Debug.LogWarning(gameObject.name + " has no rigidbody attached");
            }
        }

        if(sphereCollider == null)
        {
            if(gameObject.GetComponent<SphereCollider>() != null)
            {
                sphereCollider = gameObject.GetComponent<SphereCollider>();
            }
            else
            {
                Debug.LogWarning(gameObject.name + " has no Sphere Collider attached");
            }
        }

        if(boxTrigger == null)
        {
            if(gameObject.GetComponent<BoxCollider>() != null)
            {
                boxTrigger = gameObject.GetComponent<BoxCollider>();
            }
            else
            {
                Debug.LogWarning(gameObject.name + " has no Box Trigger attached");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(currentlyBeingCarried)
        {
            transform.position = objectToFollow.position;
        }
    }

    private void FixedUpdate()
    {
        if(wasThrown && !wasLaunched)
        {
            rb.linearVelocity -= new Vector3(0, gravity, 0) * Time.fixedDeltaTime;
        }

        HandleLaunchGravity();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(wasThrown)
        {
            onHitAfterThrown?.Invoke();
        }
        else if (wasDropped)
        {
            onHitAfterDropped?.Invoke();
        }

        rb.useGravity = true;
        wasLaunched = false;
        wasThrown = false;
        wasDropped = false;
        rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Test");
    }

    public void Throw(Vector2 direction)
    {
        currentlyBeingCarried = false;
        rb.linearVelocity = new Vector3(direction.x * distance, 1f, direction.y * distance);

        sphereCollider.enabled = true;

        onThrow?.Invoke();

        wasThrown = true;
    }


    public void Drop()
    {
        rb.useGravity = true; 
        currentlyBeingCarried = false;

        onDrop?.Invoke();

        sphereCollider.enabled = true;

        sphereCollider.excludeLayers = LayerMask.GetMask("Player");

        wasDropped = true;
    }

    public virtual void PickUp(Transform objectToFollow)
    {
        rb.useGravity = false;
        wasLaunched = false;
        this.objectToFollow = objectToFollow;
        currentlyBeingCarried = true;
        sphereCollider.enabled = false;

        onPickUp?.Invoke();
    }

    public void Launch(Vector2 direction, float height, float distance, float timeToDestination, Vector3 launchPosition)
    {
        //$Bomb Bouncing
        FMODUnity.RuntimeManager.PlayOneShot("event:/Sound Effects/Bomb Sounds/Bomb_Bouncing",transform.position);
        height *= 2;
        rb.useGravity = false;
        launchGravity = 0f;
        rb.linearVelocity = Vector3.zero;
        transform.position = launchPosition;
        transform.position += Vector3.up * 0.5f;
        wasLaunched = true;
        StartCoroutine(LaunchDelay(0.1f, direction, height, distance, timeToDestination, launchPosition));
    }

    private void HandleLaunchGravity()
    {
        if(wasLaunched)
        {
            rb.linearVelocity -= new Vector3(0, launchGravity, 0) * Time.fixedDeltaTime;
        }
    }

    System.Collections.IEnumerator LaunchDelay(float duration, Vector2 direction, float height, float distance, float timeToDestination, Vector3 launchPosition)
    {
        yield return new WaitForSeconds(duration);

        rb.linearVelocity = new Vector3(direction.x * distance, height, direction.y * distance);
        launchGravity = height * 2;
    }
}
