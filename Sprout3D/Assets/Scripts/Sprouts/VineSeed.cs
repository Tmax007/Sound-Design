using UnityEngine;

public class VineSeed : GrowableSproutBaseClass
{
    private VinePlant vine;

    public Transform seedMesh;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vine = GetComponentInChildren<VinePlant>();
        vine.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(vine.hasUnGrown)
        {
            InvokeUngrowthEvent();

            Destroy(gameObject);
        }
    }

    //Handle vine growth, callable using IGrowable interface
    public override void GrowSprout(float duration, Vector2 growthDirection)
    {
        //$Seed Growing
        //FMODUnity.RuntimeManager.PlayOneShot("event:/Sound Effects/Seed Growing");
        if (!hasBeenGrown)
        {
            //Disable seed mesh and collider
            seedMesh.GetComponent<MeshRenderer>().enabled = false;
            //gameObject.GetComponent<SphereCollider>().enabled = false;
        
            //Activate vine and set duration of vine at full growth
            vine.fullGrowthDuration = duration;
            hasBeenGrown = true;
            vine.gameObject.SetActive(true);  
            vine.SetGrowthDirection(NewPlayerController.facingDirection);
        }
        else if(hasBeenGrown && !hasBeenOvercharged)
        {
            vine.Overcharge(duration);
            hasBeenOvercharged = true;

            Debug.Log("Plant was overcharged");
        }
    }

    public override float GetGrowthProgress()
    {
        return vine.GetGrowthProgress();
    }
}