using UnityEngine;

public class EmptySproutGrowth : GrowableSproutBaseClass
{
    public float fullGrowthDuration = 0f;
    public float timeToGrow = 0.5f;

    private float growTimer = 0f;
    private float fullGrowthTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
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
    }

    public override void GrowSprout(float duration, Vector2 growthDirection)
    {
        fullGrowthDuration = duration;
    }

    private void UnGrow()
    {
        InvokeUngrowthEvent();
        Destroy(gameObject);
    }
}
