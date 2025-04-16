using UnityEditor;
using UnityEngine;

public class MechanicalDoor : MonoBehaviour, IActivatable
{
    private int numOfSpiritFlowers = 0;
    private int numOfGrownFlowers = 0;

    private float lerpAlpha = 0f;
    public float timeToOpenDoor = 0.5f;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private bool isActivated = false;
    public bool reverseResponse = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(!reverseResponse)
        {
            startPosition = transform.position;

            endPosition = transform.position - new Vector3(0, 3f, 0);
        }
        else
        {
            startPosition = transform.position - new Vector3(0, 3f, 0);

            endPosition = transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isActivated)
        {
            //$Machine Door Open
            //FMODUnity.RuntimeManager.PlayOneShot("event:/Sound Effects/Machine Door Open");
            lerpAlpha += Time.deltaTime / timeToOpenDoor;
        }
        else
        {
            //$Machine Door Close
            //FMODUnity.RuntimeManager.PlayOneShot("event:/Sound Effects/Machine Door Open");
            lerpAlpha -= Time.deltaTime / timeToOpenDoor;
        }

        HandleDoorMovement();
    }

    void HandleDoorMovement()
    {
        lerpAlpha = Mathf.Clamp01(lerpAlpha);

        transform.position = Vector3.Lerp(startPosition, endPosition, lerpAlpha);
    }

    public void Activate()
    {
        isActivated = true;
    }

    public void Deactivate()
    {
        isActivated = false;
    }
}
