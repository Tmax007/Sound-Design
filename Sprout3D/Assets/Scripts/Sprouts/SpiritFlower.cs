using System.Data.Common;
using UnityEngine;

public class SpiritFlower : GrowableSproutBaseClass
{
    public GameObject indicator; 

    private float fullGrowthDuration = 5f;
    private float timeToGrow = 0.5f;

    private float growTimer = 0f;
    private float fullGrowthTimer = 0f;

    public bool hasGrown { get; private set; } = false;

    public ParticleSystem noGrowthParticles;
    public ParticleSystem fullGrowthParticles;
    public ParticleSystem overchargeParticles;
    public SurroundingGrowthEffect surroundingGrowth;
    public SpriteRenderer sprite;
    public Sprite startingFlowerSprite;
    public Sprite grownFlowerSprite;
    public GrowthTimeBar growthTimeBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        growthTimeBar.gameObject.SetActive(false);
        noGrowthParticles.Play();
        fullGrowthParticles.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasGrown)
        {
            //grow vine according to the player's facing direciton
            if (growTimer < timeToGrow)
            {
                growTimer += Time.deltaTime;
            }

            //if vine is at full growth, hold at full growth for a certain amount of duration
            else if (growTimer >= timeToGrow && fullGrowthTimer < fullGrowthDuration)
            {
                fullGrowthTimer += Time.deltaTime;
            }

            //Ungrow vine after full growth timer has reached the threshold
            else if (fullGrowthTimer >= fullGrowthDuration)
            {
                UnGrow();
            }

            growthTimeBar.SetBarProgress(fullGrowthTimer / fullGrowthDuration);
        }
    }

    public override void GrowSprout(float duration, Vector2 growthDirection)
    {
        if(!hasGrown)
        {
            growthTimeBar.gameObject.SetActive(true);
            fullGrowthDuration = duration;
            hasGrown = true;
            growTimer = 0f;
            fullGrowthTimer = 0;

            InvokeGrowthEvent();

            if (indicator != null)
            {
               indicator.GetComponent<IActivatable>().Activate();
            }

            fullGrowthParticles.Play();
            noGrowthParticles.Stop();

            surroundingGrowth.StartGrowth();

            sprite.sprite = grownFlowerSprite;
            Debug.Log("Flower Grow");
        }
        else if(hasGrown && !hasBeenOvercharged)
        {
            fullGrowthDuration += duration;
            growthTimeBar.Overcharge();
            fullGrowthTimer = 0f;
            hasBeenOvercharged = true;
            ActivateOverchargeEffect();
            Debug.Log("Has Been Overcharged");
        }
    }

    private void UnGrow()
    {
        InvokeUngrowthEvent();
        hasGrown = false;
        hasBeenOvercharged = false;

        if(indicator != null)
        {
            indicator.GetComponent<IActivatable>().Deactivate();
        }

        growthTimeBar.RevertOvercharge();

        surroundingGrowth.StartUngrowth();

        sprite.sprite = startingFlowerSprite;

        fullGrowthParticles.Stop();
        noGrowthParticles.Play();

        DeactivateOverchargeEffect();

        growthTimeBar.gameObject.SetActive(false);
    }
    
    private void ActivateOverchargeEffect()
    {
        sprite.color = Color.yellow;
        overchargeParticles.Play();
    }

    private void DeactivateOverchargeEffect()
    {
        sprite.color = Color.white;
        overchargeParticles.Stop();
    }
}