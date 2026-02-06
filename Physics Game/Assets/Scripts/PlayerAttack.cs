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
    [SerializeField] private float heatDrainAmount = 0.3f;

    [Header("Debug")]
    [SerializeField, Range(0f, 1f)] private float currentHeat;

    private PlayerHeatBoost heatBoost;
    private PointEffector2D pointEffector;
    private CircleCollider2D circleCollider;
    private ShockwaveVisual shockwaveVisual;

    private PlayerCameraController cam; // Shake

    private bool isActive;
    private float cooldownTimer;

    private void Awake()
    {
        heatBoost = GetComponentInParent<PlayerHeatBoost>();
        pointEffector = GetComponent<PointEffector2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        cam = FindObjectOfType<PlayerCameraController>();

        // 🔥 AUTOMATISK KOPPLING – FUNKAR ÄVEN OM OBJEKTET ÄR INAKTIVT
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
            if (!isActive && cooldownTimer <= 0f && heatBoost != null && heatBoost.heat >= 0.6f)
                StartCoroutine(ActivateShockwave());
        }
    }

    private IEnumerator ActivateShockwave()
    {
        isActive = true;
        cooldownTimer = cooldown;

        // 🔥 VISUELL EFFEKT
        if (shockwaveVisual != null && heatBoost != null)
            shockwaveVisual.Play(heatBoost.heat);

        // ⚡ AKTIVERA ATTACK
        pointEffector.enabled = true;

        if (heatBoost != null)
            heatBoost.heat = Mathf.Clamp01(heatBoost.heat - heatDrainAmount);

        // 🎯 HIT DETECTION
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius);
        List<GameObject> enemies = new List<GameObject>();

        foreach (Collider2D hit in hits)
            if (hit.CompareTag("Enemy"))
                enemies.Add(hit.gameObject);

        // ⚡ CAMERA SHAKE
        if (cam != null)
        {
            // Vänta en kort stund så PS startar, men inte tillräckligt för att störa visual
            yield return new WaitForSeconds(0.01f);
            cam.TriggerShockwaveShake(intensity: 15f, duration: 0.5f);
        }

        yield return new WaitForSeconds(activeTime);

        pointEffector.enabled = false;
        isActive = false;

        foreach (GameObject enemy in enemies)
            StartCoroutine(DestroyAfterDelay(enemy, 0.1f));
    }

    private IEnumerator DestroyAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
            Destroy(obj);
    }
}
