using UnityEngine;

public interface ILaunchable
{
    public void Launch(Vector2 direction, float height, float distance, float time, Vector3 launchPosition);
}
