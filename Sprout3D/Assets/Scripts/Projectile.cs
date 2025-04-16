using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 5f;
    private Rigidbody rb;

    public void Initialize(float projectileSpeed)
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.right * projectileSpeed;
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Respawn respawnScript = collision.gameObject.GetComponent<Respawn>();
            if (respawnScript != null)
            {
                respawnScript.RespawnPlayer();
            }
        }

        // Destroy the projectile when it collides with anything
        Destroy(gameObject);
    }
}