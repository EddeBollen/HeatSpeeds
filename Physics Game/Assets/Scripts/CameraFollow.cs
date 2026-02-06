using UnityEngine;
using Unity.Cinemachine;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Zoom Settings")]
    public float baseZoom = 8f;
    public float maxZoomOut = 13f;
    public float zoomSpeed = 3f;

    [Header("Shake Settings")]
    public float boostShakeIntensity = 1.5f;
    public float boostShakeDuration = 0.25f;

    public float impactShakeIntensity = 0.05f;
    public float impactShakeMultiplier = 0.12f; // 🔥 skalar med fallhastighet
    public float maxImpactShake = 20f;           // 🔥 max quake

    public float followLag = 0.2f;

    private Rigidbody2D rb;
    private PlayerMovement movement;

    private CinemachineCamera cineCam;
    private CinemachineBasicMultiChannelPerlin noise;

    private float currentZoomVelocity;
    private float shakeTimer;
    private Vector3 camOffsetVelocity;

    private void Awake()
    {
        if (target != null)
        {
            rb = target.GetComponent<Rigidbody2D>();
            movement = target.GetComponent<PlayerMovement>();
        }

        cineCam = FindObjectOfType<CinemachineCamera>();
        if (cineCam == null)
            Debug.LogError("No CinemachineCamera found!");

        if (cineCam != null)
            noise = cineCam.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
    }

    private void LateUpdate()
    {
        if (cineCam == null || target == null || rb == null) return;

        // Follow
        Vector3 desiredPos = target.position;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref camOffsetVelocity, followLag);

        // Zoom baserat på speed
        float speed = Mathf.Abs(rb.linearVelocity.x);
        float speedFactor = Mathf.InverseLerp(0f, movement.maxSpeed * 1.5f, speed);
        float targetZoom = Mathf.Lerp(baseZoom, maxZoomOut, speedFactor);

        cineCam.Lens.OrthographicSize = Mathf.SmoothDamp(
            cineCam.Lens.OrthographicSize,
            targetZoom,
            ref currentZoomVelocity,
            1f / zoomSpeed
        );

        // Fade shake
        if (noise != null)
        {
            if (shakeTimer > 0f)
            {
                shakeTimer -= Time.deltaTime;
            }
            else
            {
                noise.AmplitudeGain = 0f;
            }
        }
    }

    // Boost shake
    public void TriggerBoostShake()
    {
        shakeTimer = boostShakeDuration;
        if (noise != null)
            noise.AmplitudeGain = boostShakeIntensity;
    }

    // Impact shake (ju hårdare fall → mer shake)
    public void TriggerImpactShake(float impactForce)
    {
        float shake = impactShakeIntensity + impactForce * impactShakeMultiplier;
        shake = Mathf.Clamp(shake, 0f, maxImpactShake);

        shakeTimer = 0.25f;
        if (noise != null)
            noise.AmplitudeGain = shake;
    }

    // Shockwave attack shake
    public void TriggerShockwaveShake(float intensity = 0.5f, float duration = 0.1f)
    {
        shakeTimer = duration;
        if (noise != null)
            noise.AmplitudeGain = intensity;
    }
}
