using UnityEngine;

public class BouncyPadSeed : GrowableSproutBaseClass
{
    public BouncyPadPlant bouncyPadPlant;
    public GameObject seed;
    public GrowthTimeBar growthTimeBar;
    public ParticleSystem overchargeParticles;

    public float currentGrowthProgress;
    public float timeToGrow = 0.5f;
    public float timeAtFullGrowth = 0f;
    public float fullGrowthTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        growthTimeBar.gameObject.SetActive(false);
        bouncyPadPlant = GetComponentInChildren<BouncyPadPlant>();
        bouncyPadPlant.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(hasBeenGrown)
        {
            currentGrowthProgress += Time.deltaTime / timeToGrow;
            bouncyPadPlant.SetGrowthProgress(currentGrowthProgress);
            fullGrowthTimer -= Time.deltaTime;
            if(fullGrowthTimer < 0f )
            {
                InvokeUngrowthEvent();
                Destroy(gameObject);
            }

            growthTimeBar.SetBarProgress((timeAtFullGrowth-fullGrowthTimer) / timeAtFullGrowth);

        }

        if(bouncyPadPlant.hasUngrown)
        {
            Destroy(gameObject);
        }
    }

    public override void GrowSprout(float duration, Vector2 growDirection)
    {
        //$Seed Growing
        //FMODUnity.RuntimeManager.PlayOneShot("event:/Sound Effects/Seed Growing");
        seed.SetActive(false);

        growthTimeBar.gameObject.SetActive(true);

        bouncyPadPlant.gameObject.SetActive(true);

        if(hasBeenGrown)
        {
            ActivateOverchargeEffect(duration);
        }
        else
        {
            timeAtFullGrowth = duration;
            bouncyPadPlant.launchDirection = growDirection;
            hasBeenGrown = true;
        }

        bouncyPadPlant.SetCurrentState(BouncyPadPlant.CurrentState.currentlyGrowing);

        fullGrowthTimer = timeAtFullGrowth;
    }

    private void ActivateOverchargeEffect(float duration)
    {
        timeAtFullGrowth += duration;
        overchargeParticles.Play();
        bouncyPadPlant.isCurrentlyOvercharged = true;
        bouncyPadPlant.OverCharge();
        hasBeenOvercharged = true;
        growthTimeBar.Overcharge();
    }

    public override float GetGrowthProgress()
    {
        return currentGrowthProgress;
    }
}
