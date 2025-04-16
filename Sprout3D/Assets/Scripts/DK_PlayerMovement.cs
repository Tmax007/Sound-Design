using UnityEngine;

public class DK_PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 2f;

    Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();  
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.W))
            {
                rb.linearVelocityY = movementSpeed;
            }
            if (Input.GetKey(KeyCode.S))
            {
                rb.linearVelocityY = -movementSpeed;
            }
        }
        else
        {
            rb.linearVelocityY = 0;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
        {
            if (Input.GetKey(KeyCode.D))
            {
                rb.linearVelocityX = movementSpeed;
            }
            if (Input.GetKey(KeyCode.A))
            {
                rb.linearVelocityX = -movementSpeed;
            }
        }
        else
        {
            rb.linearVelocityX = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("PlayerHit");
    }
}
