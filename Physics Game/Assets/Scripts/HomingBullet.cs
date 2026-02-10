using UnityEngine;

public class BossHomingBullet2D : MonoBehaviour
{
    public float startSpeed = 2f;       // initial speed
    public float maxSpeed = 12f;        // max speed during homing
    public float acceleration = 20f;    // acceleration during homing
    public float turnSpeed = 200f;      // rotation speed
    public int damage = 1;
    public float homingDuration = 3f;   // seconds to follow player

    Transform player;
    Rigidbody2D rb;

    float currentSpeed;
    bool homingActive = true;          // start with homing
    float homingTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        currentSpeed = startSpeed;

        Destroy(gameObject, 7f); // destroy after some time in case it never hits
    }

    void FixedUpdate()
    {
        if (homingActive)
        {
            homingTimer += Time.fixedDeltaTime;

            // Accelerate towards max speed
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.fixedDeltaTime);

            // Calculate direction to player
            Vector2 dir = (Vector2)player.position - rb.position;
            dir.Normalize();

            // Smooth rotation towards player
            float rotateAmount = Vector3.Cross(dir, transform.right).z;
            rb.angularVelocity = -rotateAmount * turnSpeed;

            // Move forward
            rb.linearVelocity = transform.right * currentSpeed;

            // Stop homing after timer
            if (homingTimer >= homingDuration)
            {
                homingActive = false;
                // rb.angularVelocity = 0f; // optional: stop rotating
            }
        }
        else
        {
            // Continue straight in current direction
            rb.linearVelocity = transform.right * currentSpeed;
            rb.angularVelocity = 0f; // no more rotation
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
