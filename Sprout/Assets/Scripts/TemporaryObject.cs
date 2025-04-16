using UnityEngine;

public class TemporaryObject : MonoBehaviour
{

    [SerializeField] float duration = 0.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("DestroySelf", duration);
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
