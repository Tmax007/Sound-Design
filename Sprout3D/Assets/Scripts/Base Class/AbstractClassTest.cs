using UnityEngine;

public class AbstractClassTest : MonoBehaviour
{
    public delegate void OnSproutUngrowth();
    public event OnSproutUngrowth onSproutUngrowth;

    protected void InvokeUngrowthEvent()
    {
        onSproutUngrowth.Invoke();
    }
}
