using FMODUnity;
using UnityEngine;

public class WindTurbine : MonoBehaviour, IActivatable
{
    public bool isActivated = false;
    public bool reverseResponse = false;
    public ParticleSystem windVFX;
    public Transform turbineMesh;
    public BoxCollider windArea;
    private Vector3 blowDirection;

    public float spinSpeed = 360f;

    public FMOD.Studio.EventInstance fanSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fanSound = RuntimeManager.CreateInstance("event:/Sound Effects/Wind Fan Working");
        fanSound.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));

        blowDirection = transform.forward;

        windArea = GetComponent<BoxCollider>();   

        if(isActivated)
        {
            //windArea.enabled = true;
            SpinTurbine();
            windVFX.Play();
            fanSound.start();
        }
        else
        {
            //windArea.enabled = false;
            windVFX.Stop();
            fanSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    // Update is called once per frame
    void Update()
    {
        blowDirection = transform.forward;

        if (isActivated)
        {
            //windArea.enabled = true;
            SpinTurbine();

            if(windVFX.isStopped)
            {
                //$Wind Fan Activate
                windVFX.Play();
                //FMODUnity.RuntimeManager.PlayOneShot("event:/Sound Effects/Wind Fan Working");
            }
        }
        else
        {
            //$Wind Fan Deactivate

            //windArea.enabled = false;
            windVFX.Stop();
        }
    }

    public void Activate()
    {
        isActivated = true;
        fanSound.start();
    }

    public void Deactivate()
    {
        isActivated = false;
        fanSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    private void OnTriggerEnter(Collider other)
    {
        PoisonGas gas = other.GetComponent<PoisonGas>();
        if (gas != null)
        {
            Vector3 blowDirection = transform.forward;   // or whatever direction you need
            gas.StartBlowing(blowDirection);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PoisonGas>() != null)
        {
            other.gameObject.GetComponent<PoisonGas>().StopBlowing();
        }
    }

    private void SpinTurbine()
    {
        float currentZ = turbineMesh.localRotation.eulerAngles.z;
        float newZ = currentZ + spinSpeed * Time.deltaTime;
        turbineMesh.localRotation = Quaternion.Euler(0, 0, newZ);
    }
}
