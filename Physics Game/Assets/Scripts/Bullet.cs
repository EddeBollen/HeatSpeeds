using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 1;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * speed; // Bullet flyger framåt
        Destroy(gameObject, 5f); // dör efter 5 sek om den inte träffar player
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Kollar bara om det är Player
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            Destroy(gameObject); // Bullet försvinner direkt
        }
        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject); // Bullet försvinner direkt
        }

    }

}