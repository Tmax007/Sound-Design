using UnityEngine;

public class GrowEventHandler : MonoBehaviour 
{
    public delegate void OnSproutUngrowth();
    public event OnSproutUngrowth onSproutUngrowth;

    void InvokeUngrowthEvent()
    {
        onSproutUngrowth.Invoke();
    }
}
