using UnityEngine;
#if UNITY_2021_2_OR_NEWER
using UnityEngine.Rendering.Universal;
#endif
using System.Linq; // för Any()

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;
    public float stopDistance = 1.2f;

    [Header("Detection")]
    public float detectRadius = 5f;
    public LayerMask playerLayer;

    [Header("Spotlight")]
    public Light unityLight;
#if UNITY_2021_2_OR_NEWER
    public Light2D light2D;
#endif
    public Color normalColor = Color.white;
    public Color alertColor = Color.red;

    [Header("Alert Settings")]
    public float alertLightDuration = 10f;

    Transform player;
    Rigidbody2D rb;
    Vector3 startScale;

    public bool isAlert = false;
    float alertTimer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        startScale = transform.localScale;

        SetLight(normalColor);
    }

    void FixedUpdate()
    {
        DetectPlayer();
        HandleAlertTimer();

        if (isAlert)
            FollowPlayer();
        else
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    void DetectPlayer()
    {
        if (isAlert) return;

        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectRadius, playerLayer);

        if (hit != null)
        {
            isAlert = true;
            alertTimer = alertLightDuration;
            SetLight(alertColor);

            // Trigger global alert
            if (GlobalAlertSystem.Instance != null)
                GlobalAlertSystem.Instance.TriggerAlert();
        }
    }

    void HandleAlertTimer()
    {
        if (!isAlert) return;

        alertTimer -= Time.deltaTime;

        if (alertTimer <= 0)
        {
            isAlert = false;
            SetLight(normalColor);

            // Om ingen annan fiende är alert, stäng av global alert
            if (GlobalAlertSystem.Instance != null)
            {
                bool anyAlert = FindObjectsOfType<EnemyMovement>().Any(e => e.isAlert);
                if (!anyAlert)
                    GlobalAlertSystem.Instance.StopAlert();
            }
        }
    }

    void FollowPlayer()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(dir.x * speed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        // Flip baserat på rörelseriktning
        if (rb.linearVelocity.x > 0.05f)
            transform.localScale = new Vector3(Mathf.Abs(startScale.x), startScale.y, startScale.z);
        else if (rb.linearVelocity.x < -0.05f)
            transform.localScale = new Vector3(-Mathf.Abs(startScale.x), startScale.y, startScale.z);
    }

    void SetLight(Color c)
    {
        if (unityLight != null) unityLight.color = c;
#if UNITY_2021_2_OR_NEWER
        if (light2D != null) light2D.color = c;
#endif
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
