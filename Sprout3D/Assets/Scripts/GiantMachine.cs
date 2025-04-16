using FMODUnity;
using System.Collections.Generic;
using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GiantMachine : MonoBehaviour, IVineInteractable
{
    //objects connected to this machine
    public GameObject[] connectedObjects;

    public Transform vineTransform;
    public Transform machineMesh;
    public Transform gears;

    public List<ParticleSystem> firstSmokeParticles = new List<ParticleSystem>();
    public List<ParticleSystem> secondSmokeParticles = new List<ParticleSystem>();
    public List<GameObject> explosions = new List<GameObject>();
    public ParticleSystem explosionParticle;

    public bool isHeldByVines = false;
    public bool hasBeenDestroyed = false;
    public bool canBeGrownOn = true;

    private float vineGrowthTime = 0.5f;
    private float vineGrowthTimer = 0f;
    public float vineOffsetAmount = 2.5f;
    public float elapsedTime = 0f;

    private Vector3 machinePosition = Vector3.zero;
    private float rotateTimer = 0f;

    [Header("Timings")]
    public float firstSmokeEffect = 1f;
    public bool firstSmokeEffectHasBeenActivated = false;

    public float secondSmokeEffect = 2f;
    public bool secondSmokeEffectHasBeenActivated = false;
    
    public float explosionEffect = 4f;
    public bool explosionEffectHasBeenActivated = false;

    public float explosionEffectDeactivate = 4.5f;
    public bool explosionEffectHasBeenDeactivated = false;

    public FMOD.Studio.EventInstance machineWorking;
    public FMOD.Studio.EventInstance machineTrapped;
    public FMOD.Studio.EventInstance explostion;

    private static int destroyedMachinesCount = 0;

    public bool isFinalMachine = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //$Machine Working
        machineWorking = RuntimeManager.CreateInstance("event:/Sound Effects/Machine Working");
        machineWorking.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        machineWorking.start();


        machineTrapped = RuntimeManager.CreateInstance("event:/Sound Effects/Machine Trapped");
        machineTrapped.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        machineTrapped.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        foreach (GameObject explosion in explosions)
        {
            if (explosion != null)
            {
                explosion.SetActive(false);
            }
        }

        machinePosition = machineMesh.position;

        foreach (var obj in connectedObjects)
        {
            obj.GetComponent<IActivatable>().Activate();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isHeldByVines)
        {
            vineGrowthTimer += Time.deltaTime / vineGrowthTime;
        }
        else
        {
            vineGrowthTimer -= Time.deltaTime / vineGrowthTime;
        }
        vineGrowthTimer = Mathf.Clamp01(vineGrowthTimer);

        float yOffset = Mathf.Lerp(0, vineOffsetAmount, vineGrowthTimer);

        vineTransform.position = new Vector3(vineTransform.position.x, yOffset, vineTransform.position.z);


        if(!isHeldByVines && !hasBeenDestroyed)
        {
            rotateTimer += Time.deltaTime;
            if(rotateTimer > 0.075f)
            {
                for (int i = 0; i < gears.childCount; i++)
                {
                    if(i == 1)
                    {
                        gears.GetChild(i).transform.localEulerAngles += new Vector3(0, 0, 10f);
                    }
                    else
                    {
                        gears.GetChild(i).transform.localEulerAngles += new Vector3(0, 0, -10f);
                    }
                }
                rotateTimer = 0f;
            }
        }

        if (isHeldByVines)
        {
            elapsedTime += Time.deltaTime;
        }

        if(elapsedTime > firstSmokeEffect && !firstSmokeEffectHasBeenActivated)
        {
            firstSmokeEffectHasBeenActivated = true;
            foreach(ParticleSystem smoke in firstSmokeParticles)
            {
                if(smoke != null)
                {
                    smoke.Play();
                }
            }
        }
         
        if(elapsedTime > secondSmokeEffect && !secondSmokeEffectHasBeenActivated)
        {
            secondSmokeEffectHasBeenActivated = true;
            foreach(ParticleSystem smoke in secondSmokeParticles)
            {
                if(smoke != null)
                {
                    smoke.Play();
                }
            }
        }

        if(elapsedTime > explosionEffect && !explosionEffectHasBeenActivated)
        {
            explosionEffectHasBeenActivated = true;
            Debug.Log("Activate Explosion");
            explosionParticle.Play();
            foreach (GameObject explosion in explosions)
            {
                explosion.SetActive(true);
            }
        }

        if (elapsedTime > explosionEffectDeactivate && !explosionEffectHasBeenDeactivated)
        {
            explosionEffectHasBeenDeactivated = true;

            foreach (GameObject explosion in explosions)
            {
                if(explosion != null)
                {
                    explosion.SetActive(false);
                }
            }

            foreach (ParticleSystem smoke in firstSmokeParticles)
            {
                if (smoke != null)
                {
                    smoke.Stop();
                }
            }
            hasBeenDestroyed = true;
            isHeldByVines = false;

            machineTrapped.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            FMODUnity.RuntimeManager.PlayOneShot("event:/Sound Effects/Bomb Door Exploding");
            Invoke("DeactivateConnectedObjects", 2f);

            //$Machine is destroyed
        }
    }

    private void FixedUpdate()
    {
        if(isHeldByVines)
        {
            ShakeViolently();
        }
    }

    //IVineInteractable interface implementaion
    //Deactivate objects connected to this machine on vine growth 
    public void OnVineGrowth()
    {
        //$Machine Trapped
        machineWorking.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        machineTrapped.start();
        if (canBeGrownOn)
        {
            Debug.Log("test");
            machineMesh.GetComponent<MeshRenderer>().material.color = Color.red;
            isHeldByVines = true;

            canBeGrownOn = false;
        }
    }

    //IVineInteractable interface implementaion
    //Activate objects connected to this machine on vine stop growing
    public void OnVineStopGrowth()
    {
        ////$Machine Working

        //foreach (var obj in connectedObjects)
        //{
        //    obj.GetComponent<IActivatable>().Activate();
        //}

        //machineMesh.GetComponent<MeshRenderer>().material.color = Color.blue;

        //isHeldByVines = false;
    }

    private void ShakeViolently()
    {
        Vector2 randomPos = Random.insideUnitCircle * 0.08f;

        machineMesh.position = machinePosition + new Vector3(randomPos.x, transform.position.y, randomPos.y);
    }

    // In DeactivateConnectedObjects():
    private void DeactivateConnectedObjects()
    {
        foreach (var obj in connectedObjects)
        {
            obj.GetComponent<IActivatable>().Deactivate();
        }

        destroyedMachinesCount++;

        if (isFinalMachine && destroyedMachinesCount >= 2)
        {
            SceneManager.LoadScene("Credits");
        }
    }
}