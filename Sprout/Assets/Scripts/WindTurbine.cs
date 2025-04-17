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

    private FMOD.Studio.EventInstance fanSound;

    void Start()
    {
        blowDirection = transform.forward;
        windArea = GetComponent<BoxCollider>();

        // Create fan loop sound
        fanSound = RuntimeManager.CreateInstance("event:/Sound Effects/Wind_Fan_Working");
        fanSound.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));

        if (isActivated)
        {
            Activate(); // Properly start if already activated
        }
        else
        {
            Deactivate();
        }
    }

    void Update()
    {
        blowDirection = transform.forward;

        if (isActivated)
        {
            SpinTurbine();

            if (windVFX.isStopped)
            {
                windVFX.Play();
            }

            // Continuously update position in case turbine moves
            fanSound.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        }
        else
        {
            if (windVFX.isPlaying)
            {
                windVFX.Stop();
            }
        }
    }

    public void Activate()
    {
        isActivated = true;
        fanSound.start();

        if (!windVFX.isPlaying)
        {
            windVFX.Play();
        }
    }

    public void Deactivate()
    {
        isActivated = false;

        fanSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        if (windVFX.isPlaying)
        {
            windVFX.Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PoisonGas gas = other.GetComponent<PoisonGas>();
        if (gas != null)
        {
            gas.StartBlowing(blowDirection);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PoisonGas gas = other.GetComponent<PoisonGas>();
        if (gas != null)
        {
            gas.StopBlowing();
        }
    }

    private void SpinTurbine()
    {
        float currentZ = turbineMesh.localRotation.eulerAngles.z;
        float newZ = currentZ + spinSpeed * Time.deltaTime;
        turbineMesh.localRotation = Quaternion.Euler(0, 0, newZ);
    }

    private void OnDestroy()
    {
        // Clean up FMOD instance
        fanSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        fanSound.release();
    }
}