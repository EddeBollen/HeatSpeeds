using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerHeatBoost : MonoBehaviour
{
    [Header("Värme / Boost-inställningar")]
    [SerializeField] private float heatGainSpeed = 10f;     // Fart som krävs för att börja bygga värme
    [SerializeField] private float heatIncreaseRate = 0.4f; // Hur snabbt värmen byggs upp
    [SerializeField] private float heatDecreaseRate = 0.1f; // Hur snabbt den svalnar
    [SerializeField] private float boostForce = 30f;
    [SerializeField] private float boostDuration = 1f;
    [SerializeField] private float boostCooldown = 3f;

    [Header("Jump inställningar")]
    [SerializeField] private float minJump = 6f;
    [SerializeField] private float maxJump = 18f;

    private PlayerMovement movement;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    public float heat = 0f;
    private bool isBoosting = false;
    private float boostTimer = 0f;
    private float cooldownTimer = 0f;
    private int boostDir = 1;
    private bool isTouchingObject = false;
    private bool grounded = false; // <- för markkontroll

    // Kamera
    private PlayerCameraController cam;

    public bool IsBoosting => isBoosting;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        cam = FindObjectOfType<PlayerCameraController>();
    }

    private void FixedUpdate()
    {
        float speed = Mathf.Abs(rb.linearVelocity.x);

        // 🔥 Bygg värme endast om bollen rör ett objekt och går tillräckligt snabbt
        if (isTouchingObject && speed >= heatGainSpeed)
            heat += heatIncreaseRate * Time.fixedDeltaTime;
        else
            heat -= heatDecreaseRate * Time.fixedDeltaTime;

        heat = Mathf.Clamp01(heat);

        // 💨 Boost med Shift
        if (Input.GetKey(KeyCode.LeftShift) && heat >= 0.1f)
        {
            isBoosting = true;
            movement.moveAccelerationMultiplier = 2f;
            movement.maxSpeedMultiplier = 1.5f;
            heat -= 0.5f * Time.fixedDeltaTime;
        }
        else
        {
            isBoosting = false;
            movement.moveAccelerationMultiplier = 1f;
            movement.maxSpeedMultiplier = 1f;
        }

        // Färg efter heat
        Color heatColor = Color.Lerp(Color.white, new Color(1f, 0.3f, 0f), heat);
        sr.color = heatColor;
    }

    private void Update()
    {
        // Hoppa
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            float jumpForce = Mathf.Lerp(minJump, maxJump, heat);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            heat = 0.1f;
            grounded = false; // vi är i luften nu
        }
    }

    // ===== Kollisioner =====
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.isTrigger)
        {
            isTouchingObject = true;

            // Markkontakt
            if (collision.contacts[0].normal.y > 0.5f)
            {
                grounded = true;

                // ---- TRIGGA MARK-SHAKE ----
                float impactForce = collision.relativeVelocity.magnitude;
                if (cam != null && impactForce > 2f)
                    cam.TriggerImpactShake(impactForce);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.collider.isTrigger)
            isTouchingObject = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.collider.isTrigger)
            isTouchingObject = false;

        if (collision.contacts.Length > 0 && collision.contacts[0].normal.y > 0.5f)
            grounded = false;
    }
}
