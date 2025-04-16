using FMODUnity;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class MachineTest : MonoBehaviour, IVineInteractable
{
    //objects connected to this machine
    public GameObject[] connectedObjects;

    public Transform vineTransform;
    public Transform machineMesh;
    public Transform gears;

    public bool isHeldByVines = false;

    private float vineGrowthTime = 0.5f;
    private float vineGrowthTimer = 0f;
    private float vineOffsetAmount = 2.5f;

    private Vector3 machinePosition = Vector3.zero;
    private float rotateTimer = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public FMOD.Studio.EventInstance machineWorking;
    public FMOD.Studio.EventInstance machineTrapped;

    void Start()
    {
        //$Machine Working
        //FMODUnity.RuntimeManager.PlayOneShot("event:/Sound Effects/Machine Working");

        machineWorking = RuntimeManager.CreateInstance("event:/Sound Effects/Machine Sounds/Machine_Working 2");
        machineWorking.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        machineWorking.start();


        machineTrapped = RuntimeManager.CreateInstance("event:/Sound Effects/Machine Sounds/Machine_Trapped 2");
        machineTrapped.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        machineTrapped.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);


        //machinePosition = machineMesh.position;

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
            machineWorking.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
        else
        {
            machineTrapped.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            vineGrowthTimer -= Time.deltaTime / vineGrowthTime;
        }
        vineGrowthTimer = Mathf.Clamp01(vineGrowthTimer);

        float yOffset = Mathf.Lerp(0, vineOffsetAmount, vineGrowthTimer);

        vineTransform.position = new Vector3(vineTransform.position.x, yOffset, vineTransform.position.z);


        if(!isHeldByVines)
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
        machineWorking.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        machineTrapped.start();

        foreach (var obj in connectedObjects)
        {
            obj.GetComponent<IActivatable>().Deactivate();
        }
        Debug.Log("test");
        machineMesh.GetComponent<MeshRenderer>().material.color = Color.red;
        isHeldByVines = true;
        //FMODUnity.RuntimeManager.PlayOneShot("event:/Sound Effects/Machine Trapped");
    }

    //IVineInteractable interface implementaion
    //Activate objects connected to this machine on vine stop growing
    public void OnVineStopGrowth()
    {
        //$Machine Working
        machineWorking.start();
        machineTrapped.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        foreach (var obj in connectedObjects)
        {
            obj.GetComponent<IActivatable>().Activate();
        }

        machineMesh.GetComponent<MeshRenderer>().material.color = Color.blue;

        isHeldByVines = false;
    }

    private void ShakeViolently()
    {
        Vector2 randomPos = Random.insideUnitCircle * 0.08f;

        machineMesh.localPosition = Vector3.zero + new Vector3(randomPos.x, transform.position.y, randomPos.y);
    }
}
