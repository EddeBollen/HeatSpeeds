using UnityEngine;
#if UNITY_2021_2_OR_NEWER
using UnityEngine.Rendering.Universal;
#endif
using System.Linq;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;
    public float stopDistance = 1.2f;

    [Header("Patrol")]
    public Transform patrolPointA;
    public Transform patrolPointB;
    public float patrolWaitTime = 1f; // hur länge den stannar vid varje punkt

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

    public bool isAlert = false;
    float alertTimer = 0f;

    // Patrol state
    private bool goingToB = true;
    private bool waiting = false;
    private float waitTimer = 0f;

    private Vector3 startScale;

    [Header("Firepoint")]
    public Transform firePoint; // Lägg till här

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
            Patrol();
    }

    // ================= DETECT PLAYER =================
    void DetectPlayer()
    {
        if (isAlert) return;

        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectRadius, playerLayer);

        if (hit != null)
        {
            isAlert = true;
            alertTimer = alertLightDuration;
            SetLight(alertColor);

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

            if (GlobalAlertSystem.Instance != null)
            {
                bool anyAlert = FindObjectsOfType<EnemyMovement>().Any(e => e.isAlert);
                if (!anyAlert)
                    GlobalAlertSystem.Instance.StopAlert();
            }
        }
    }

    // ================= PATROL =================
    void Patrol()
    {
        if (patrolPointA == null || patrolPointB == null)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // Bestäm nuvarande mål
        Vector2 target = goingToB ? (Vector2)patrolPointB.position : (Vector2)patrolPointA.position;

        // Vänta om fienden väntar
        if (waiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                waiting = false;
                goingToB = !goingToB; // byt mål
            }
            return;
        }

        // Beräkna avstånd till mål
        float distance = Vector2.Distance(rb.position, target);

        if (distance < 0.05f)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            waiting = true;
            waitTimer = patrolWaitTime;
            return;
        }

        // Flytta mot mål
        Vector2 dir = (target - rb.position).normalized;
        rb.linearVelocity = new Vector2(dir.x * speed, rb.linearVelocity.y);

        Flip(dir.x);
    }

    // ================= FOLLOW PLAYER =================
    void FollowPlayer()
    {
        float distance = Vector2.Distance(rb.position, player.position);

        if (distance > stopDistance)
        {
            Vector2 dir = ((Vector2)player.position - rb.position).normalized;
            rb.linearVelocity = new Vector2(dir.x * speed, rb.linearVelocity.y);
            Flip(dir.x);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    // ================= FLIP SPRITE =================
    void Flip(float x)
    {
        if (x > 0.05f)
            transform.localScale = new Vector3(Mathf.Abs(startScale.x), startScale.y, startScale.z);
        else if (x < -0.05f)
            transform.localScale = new Vector3(-Mathf.Abs(startScale.x), startScale.y, startScale.z);

        // ENDA ÄNDRINGEN: rotera firePoint 180 grader vid flip
        if (firePoint != null)
        {
            firePoint.localRotation = Quaternion.Euler(0f, 0f, transform.localScale.x > 0 ? 0f : 180f);
        }

    }

    // ================= LIGHT =================
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
        if (patrolPointA != null) Gizmos.DrawSphere(patrolPointA.position, 0.1f);
        if (patrolPointB != null) Gizmos.DrawSphere(patrolPointB.position, 0.1f);
    }
}
