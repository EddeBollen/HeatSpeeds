using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootDistance = 6f;
    public float fireRate = 1f;
    public float bulletSpeed = 8f;

    Transform player;
    float nextFireTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < shootDistance && Time.time > nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        float dir = transform.root.localScale.x > 0 ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * bulletSpeed, 0f);

        Destroy(bullet, 4f);
    }
}
