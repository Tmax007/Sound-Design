using FMODUnity;
using UnityEngine;

public class PoisonGas : MonoBehaviour
{
    public float timeToSubside = 0.5f;
    public float subsidePrograss = 0f;
    public Vector3 subsideDirection = Vector3.down;
    public float subsideDistance = 3f;
    private Vector3 startingPosition;
    private Vector3 subsidePosition;
    public Transform poisonGasses;
    public BoxCollider poisonArea;
    public bool currentlyBlown = false;
    public bool gasIsActve = true;

    private FMOD.Studio.EventInstance gasActiveSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gasActiveSound = RuntimeManager.CreateInstance("event:/Sound Effects/Poison Gas");
        gasActiveSound.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        gasActiveSound.start();

        //$Poison Gas
        poisonArea = GetComponent<BoxCollider>();
        startingPosition = poisonGasses.position;
        subsidePosition = poisonGasses.position + subsideDirection * subsideDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentlyBlown)
        {
            subsidePrograss += Time.deltaTime/timeToSubside;
        }
        else
        {
            subsidePrograss -= Time.deltaTime/timeToSubside;

            if(!poisonGasses.gameObject.activeSelf)
            {
                //$Poison Gas
                poisonGasses.gameObject.SetActive(true);
            }
        }

        subsidePrograss = Mathf.Clamp01(subsidePrograss);

        if (subsidePrograss >= 1)
        {
            poisonGasses.gameObject.SetActive(false);
            gasIsActve = false;
        }
        if (subsidePrograss <= 0.1)
        {
            gasIsActve = true;
        }

        poisonGasses.position = Vector3.Lerp(startingPosition, subsidePosition, subsidePrograss/timeToSubside);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if(other.gameObject.GetComponent<WindTurbine>() != null)
        //{
        //    currentlyBlown = true;
        //}
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<WindTurbine>() != null)
        {
            if(other.gameObject.GetComponent<WindTurbine>().isActivated)
            {

                //$Poison Gas Dissapearing
                if (!currentlyBlown)
                {
                    gasActiveSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Sound Effects/Poison Gas Disappearing");
                }

                currentlyBlown = true;
            }
            else
            {
                if(currentlyBlown)
                {
                    gasActiveSound.start();
                }
                currentlyBlown = false;  
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Extit");
        //if (other.gameObject.GetComponent<WindTurbine>() != null)
        //{
        //    currentlyBlown = false;
        //}
    }

    public void StartBlowing(Vector3 direction)
    {
        currentlyBlown = true;

        direction = Vector3.Normalize(direction);
    }

    public void StopBlowing()
    {
        currentlyBlown = false;
    }

    public void ChangeSubsideDirection(Vector3 direction)
    {
        subsideDirection = direction;
        subsidePosition = poisonGasses.position + subsideDirection * subsideDistance;
    }
}
