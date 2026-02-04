using UnityEngine;
using Unity.Cinemachine;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Zoom Settings")]
    public float baseZoom = 8f;
    public float maxZoomOut = 13f;
    public float zoomSpeed = 4f;

    [Header("Shake Settings")]
    public float boostShakeIntensity = 1.5f;
    public float boostShakeDuration = 0.25f;

    public float impactShakeIntensity = 1f;
    public float impactShakeMultiplier = 0.3f;
    public float maxImpactShake = 5f;

    public float followLag = 0.2f;

    // Interna
    private Rigidbody2D rb;
    private PlayerMovement movement;

    private CinemachineCamera cineCam; // CM3
    private CinemachineBasicMultiChannelPerlin noise;

    private float currentZoomVelocity;
    private float shakeTimer = 0f;
    private Vector3 camOffsetVelocity;

    private void Awake()
    {
        if (target != null)
        {
            rb = target.GetComponent<Rigidbody2D>();
            movement = target.GetComponent<PlayerMovement>();
        }

        // Hitta CM3 kamera
        cineCam = FindObjectOfType<CinemachineCamera>();
        if (cineCam == null)
        {
            Debug.LogError("No CinemachineCamera found in scene!");
            return;
        }

        // Hämta noise extension
        noise = cineCam.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
        if (noise == null)
        {
            Debug.LogWarning("Add Basic Multi Channel Perlin to the CinemachineCamera!");
        }

        // Viktigt: se till att noise inte skakar hela tiden
        if (noise != null)
            noise.AmplitudeGain = 0f;
    }

    private void LateUpdate()
    {
        if (cineCam == null || target == null || rb == null) return;

        // Smooth follow
        Vector3 desiredPos = target.position;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref camOffsetVelocity, followLag);

        // Zoom baserat på hastighet
        float speed = Mathf.Abs(rb.linearVelocity.x);
        float speedFactor = Mathf.InverseLerp(0f, movement.maxSpeed * 1.5f, speed);
        float targetZoom = Mathf.Lerp(baseZoom, maxZoomOut, speedFactor);

        cineCam.Lens.OrthographicSize = Mathf.SmoothDamp(
            cineCam.Lens.OrthographicSize,
            targetZoom,
            ref currentZoomVelocity,
            1f / zoomSpeed
        );

        // Shake hantering
        if (noise != null)
        {
            if (shakeTimer > 0f)
            {
                shakeTimer -= Time.deltaTime;
                if (shakeTimer <= 0f)
                    noise.AmplitudeGain = 0f; // stop shake
            }
        }
    }

    // ===== Shake triggers =====

    // Boost shake
    public void TriggerBoostShake()
    {
        if (noise == null) return;

        shakeTimer = boostShakeDuration;
        noise.AmplitudeGain = boostShakeIntensity;
    }

    // Ground impact shake
    public void TriggerImpactShake(float impactForce)
    {
        if (noise == null) return;

        float shake = impactShakeIntensity + impactForce * impactShakeMultiplier;
        shake = Mathf.Clamp(shake, 0f, maxImpactShake);
        shakeTimer = 0.2f;
        noise.AmplitudeGain = shake;
    }

    // Shockwave shake
    public void TriggerShockwaveShake(float intensity = 2.5f, float duration = 0.3f)
    {
        if (noise == null) return;

        shakeTimer = duration;
        noise.AmplitudeGain = intensity;
    }
}
