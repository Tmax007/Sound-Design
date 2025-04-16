using UnityEngine;

public class Debri : MonoBehaviour
{
    public float timeToDespawn = 3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("Despawn", timeToDespawn);

        transform.localScale = Vector3.one * Random.Range(0.2f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }
}
