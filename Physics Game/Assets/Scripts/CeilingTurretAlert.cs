using UnityEngine;
#if UNITY_2021_2_OR_NEWER
using UnityEngine.Rendering.Universal;
#endif

public class CeilingTurret : MonoBehaviour
{
    [Header("Turret Parts")]
    public Transform turretPivot;   // Skaftet som roterar
    public Transform firePoint;     // Kulans spawnpunkt
    public GameObject bulletPrefab;

    [Header("Shooting")]
    public float shootDistance = 6f;
    public float fireRate = 1f;       // Skott per sekund
    public float bulletSpeed = 8f;

    [Header("Detection")]
    public float detectRadius = 6f;
    public LayerMask playerLayer;

    [Header("Light")]
    public Light unityLight;
#if UNITY_2021_2_OR_NEWER
    public Light2D light2D;
#endif
    public Color normalColor = Color.white;
    public Color alertColor = Color.red;

    [Header("Alert")]
    public float alertLightDuration = 10f;

    Transform player;
    float nextFireTime;
    bool isAlert;
    float alertTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        SetLight(normalColor);
    }

    void Update()
    {
        if (player == null) return;

        DetectPlayer();
        HandleAlertTimer();

        if (isAlert)
        {
            AimAtPlayer();
            TryShoot();
        }
    }

    // ================= DETECTION =================
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
                GlobalAlertSystem.Instance.StopAlert();
        }
    }

    // ================= AIM =================
    void AimAtPlayer()
    {
        Vector2 dir = player.position - turretPivot.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        turretPivot.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    // ================= SHOOT =================
    void TryShoot()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > shootDistance) return;

        if (Time.time > nextFireTime)
        {
            Shoot();

            // Högre fireRate = snabbare skjutning
            nextFireTime = Time.time + (1f / Mathf.Max(fireRate, 0.0001f));
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Rotate 90 degrees om sprite är liggande
        bullet.transform.Rotate(0, 0, 90f);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        // Flyger längs FirePoint upp
        rb.linearVelocity = firePoint.up * bulletSpeed;

        Destroy(bullet, 4f);
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
