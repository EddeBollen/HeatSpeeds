using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PointEffector2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float activeTime = 0.5f;
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private float heatDrainAmount = 0.2f;

    [Header("Debug")]
    [SerializeField, Range(0f, 1f)] private float currentHeat;

    private PlayerHeatBoost heatBoost;
    private PointEffector2D pointEffector;
    private CircleCollider2D circleCollider;
    private ShockwaveVisual shockwaveVisual;

    private PlayerCameraController cam;

    private bool isActive;
    private float cooldownTimer;

    private void Awake()
    {
        heatBoost = GetComponentInParent<PlayerHeatBoost>();
        pointEffector = GetComponent<PointEffector2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        cam = FindObjectOfType<PlayerCameraController>();

        shockwaveVisual = GetComponentInChildren<ShockwaveVisual>(true);

        if (shockwaveVisual == null)
            Debug.LogError("ShockwaveVisual NOT FOUND in children!");

        pointEffector.enabled = false;
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (heatBoost != null)
            currentHeat = heatBoost.heat;

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isActive && cooldownTimer <= 0f && heatBoost != null && heatBoost.heat >= 0.4f)
                StartCoroutine(ActivateShockwave());
        }
    }

    private IEnumerator ActivateShockwave()
    {
        isActive = true;
        cooldownTimer = cooldown;

        if (shockwaveVisual != null && heatBoost != null)
            shockwaveVisual.Play(heatBoost.heat);

        pointEffector.enabled = true;

        if (heatBoost != null)
            heatBoost.heat = Mathf.Clamp01(heatBoost.heat - heatDrainAmount);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius);
        List<GameObject> enemies = new List<GameObject>();

        foreach (Collider2D hit in hits)
            if (hit.CompareTag("Enemy"))
                enemies.Add(hit.gameObject);

        if (cam != null)
        {
            yield return new WaitForSeconds(0.01f);
            cam.TriggerShockwaveShake(intensity: 15f, duration: 0.5f);
        }

        // ✅ DAMAGE ISTÄLLET FÖR DESTROY
        foreach (GameObject enemy in enemies)
        {
            EnemyHealth hp = enemy.GetComponent<EnemyHealth>();
            if (hp != null)
                hp.TakeDamage(1);
        }

        yield return new WaitForSeconds(activeTime);

        pointEffector.enabled = false;
        isActive = false;
    }
}
