using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerHeatBoost : MonoBehaviour
{
    [Header("Värme / Boost-inställningar")]
    [SerializeField] private float heatGainSpeed = 10f;
    [SerializeField] private float heatIncreaseRate = 0.4f;
    [SerializeField] private float heatDecreaseRate = 0.1f;

    [Header("Boost Settings (Hold Shift)")]
    [SerializeField] private float boostAccelerationMultiplier = 2f;
    [SerializeField] private float boostMaxSpeedMultiplier = 2f;
    [SerializeField] private float heatDrainWhileBoosting = 0.3f;

    [Header("Jump Settings")]
    [SerializeField] private float minJumpForce = 5f;
    [SerializeField] private float maxJumpForce = 18f;

    private PlayerMovement movement;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    public float heat = 0f;

    private bool isBoosting = false;
    private bool isTouchingObject = false;
    private bool isGrounded = false;

    public bool IsBoosting => isBoosting;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // Boost (håll Shift)
        isBoosting = Input.GetKey(KeyCode.LeftShift) && heat > 0.05f;
    }

    private void FixedUpdate()
    {
        float speed = Mathf.Abs(rb.linearVelocity.x);

        // Bygg heat när bollen rör något och inte boostar
        if (isTouchingObject && speed >= heatGainSpeed && !isBoosting)
            heat += heatIncreaseRate * Time.fixedDeltaTime;
        else if (!isBoosting)
            heat -= heatDecreaseRate * Time.fixedDeltaTime;

        heat = Mathf.Clamp01(heat);

        // BOOST
        if (isBoosting)
        {
            heat -= heatDrainWhileBoosting * Time.fixedDeltaTime;
            heat = Mathf.Clamp01(heat);

            movement.moveAccelerationMultiplier = boostAccelerationMultiplier;
            movement.maxSpeedMultiplier = boostMaxSpeedMultiplier;
        }
        else
        {
            movement.moveAccelerationMultiplier = 1f;
            movement.maxSpeedMultiplier = 1f;
        }

        // Färg baserat på heat
        Color heatColor = Color.Lerp(Color.white, new Color(1f, 0.3f, 0f), heat);
        sr.color = heatColor;
    }

    private void Jump()
    {
        float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, heat);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        heat *= 0.3f; // dränera lite heat
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.isTrigger)
            isTouchingObject = true;
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.collider.isTrigger)
            isTouchingObject = true;
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.collider.isTrigger)
            isTouchingObject = false;
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}
