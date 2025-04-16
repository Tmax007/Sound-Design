using UnityEngine;

public class GrowableSproutBaseClass : MonoBehaviour
{
    public delegate void OnSproutGrowth();
    public event OnSproutGrowth onSproutGrowth;

    public delegate void OnSproutUngrowth();
    public event OnSproutUngrowth onSproutUngrowth;

    public bool hasBeenGrown = false;
    public bool hasBeenOvercharged = false;

    public float growthProgress { get; private set; } = 0f;

    protected void InvokeUngrowthEvent()
    {
        onSproutUngrowth?.Invoke();
    }

    protected void InvokeGrowthEvent()
    {
        onSproutGrowth?.Invoke();
    }

    public virtual void GrowSprout(float duration, Vector2 growthDirection)
    {

    }

    public virtual float GetGrowthProgress()
    {
        return 0f;
    }
}
