using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Rörelseinställningar")]
    public float acceleration = 20f;
    public float maxSpeed = 15f;
    public float brakeDecel = 25f;

    [HideInInspector] public float moveAccelerationMultiplier = 1f;
    [HideInInspector] public float maxSpeedMultiplier = 1f;

    [HideInInspector] public Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        float moveInput = 0f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) moveInput = -1f;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) moveInput = 1f;

        float currentAcceleration = acceleration * moveAccelerationMultiplier;
        float currentMaxSpeed = maxSpeed * maxSpeedMultiplier;

        if (moveInput != 0)
        {
            rb.linearVelocity = new Vector2(
                Mathf.MoveTowards(rb.linearVelocity.x, moveInput * currentMaxSpeed, currentAcceleration * Time.fixedDeltaTime),
                rb.linearVelocity.y
            );
        }
        else
        {
            rb.linearVelocity = new Vector2(
                Mathf.MoveTowards(rb.linearVelocity.x, 0f, brakeDecel * Time.fixedDeltaTime),
                rb.linearVelocity.y
            );
        }

        // Rulla bollen
        float radius = GetComponent<CircleCollider2D>().radius * transform.localScale.x;
        float expectedAngular = -(rb.linearVelocity.x / (2 * Mathf.PI * radius)) * 360f;
        rb.angularVelocity = expectedAngular;
    }
}
