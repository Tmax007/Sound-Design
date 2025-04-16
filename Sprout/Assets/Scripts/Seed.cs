using UnityEngine;

public class Seed : MonoBehaviour
{
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Stop movement on impact
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
    }
}