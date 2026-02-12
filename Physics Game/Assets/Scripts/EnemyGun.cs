using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 8f;
    public float fireRate = 1f;
    public float shootDistance = 6f;

    private Transform player;
    private Transform enemy; // parent enemy
    private float nextFireTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        enemy = GetComponentInParent<EnemyMovement>()?.transform;

        if (firePoint == null)
            Debug.LogError("EnemyGun: FirePoint is missing!");
        if (enemy == null)
            Debug.LogError("EnemyGun: Could not find EnemyMovement parent!");
    }

    void Update()
    {
        if (player == null || firePoint == null || enemy == null)
            return;

        // Räkna ut avstånd till spelaren
        float distance = Vector2.Distance(transform.position, player.position);

        // Skjut om spelaren är inom range och cooldown klar
        if (distance <= shootDistance && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        // Rotera FirePoint beroende på enemy facing
        FlipFirePoint(enemy.localScale.x > 0);
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = firePoint.right * bulletSpeed;
        }

        Destroy(bullet, 4f);
    }

    /// <summary>
    /// Rotera firePoint 0° eller 180° på Z beroende på enemy facing
    /// </summary>
    /// <param name="facingRight"></param>
    void FlipFirePoint(bool facingRight)
    {
        if (firePoint != null)
        {
            firePoint.localRotation = facingRight
                ? Quaternion.Euler(0f, 0f, 0f)
                : Quaternion.Euler(0f, 0f, 180f);
        }
    }
}
