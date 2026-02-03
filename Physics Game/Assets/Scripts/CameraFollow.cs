using UnityEngine;
using Unity.Cinemachine;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Cinemachine")]
    public CinemachineVirtualCamera cineCam;
    public Transform target;

    [Header("Zoom-inställningar")]
    [SerializeField] private float baseZoom = 8f;
    [SerializeField] private float maxZoomOut = 13f;
    [SerializeField] private float zoomSpeed = 3f;

    [Header("Boost-effekter")]
    [SerializeField] private float boostZoomOut = 15f;
    [SerializeField] private float shakeIntensity = 1.5f;
    [SerializeField] private float shakeDuration = 0.25f;
    [SerializeField] private float followLag = 0.2f;

    private PlayerMovement movement;
    private PlayerHeatBoost heatBoost;
    private Rigidbody2D rb;

    private float currentZoomVelocity;
    private float shakeTimer = 0f;
    private Vector3 camOffsetVelocity;

    private CinemachineBasicMultiChannelPerlin noise;
    private bool wasBoostingLastFrame = false;

    private void Awake()
    {
        if (target != null)
        {
            movement = target.GetComponent<PlayerMovement>();
            heatBoost = target.GetComponent<PlayerHeatBoost>();
            rb = target.GetComponent<Rigidbody2D>();
        }

        if (cineCam != null)
        {
            // Få noise-komponenten för kamerashake
            var cineComp = cineCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (cineComp != null) noise = cineComp;
        }
    }

    private void LateUpdate()
    {
        if (cineCam == null || target == null || rb == null) return;

        // Smooth follow
        cineCam.Follow = target;

        float speed = Mathf.Abs(rb.linearVelocity.x);
        float speedFactor = Mathf.InverseLerp(0f, movement.maxSpeed * 1.5f, speed);
        float targetZoom = Mathf.Lerp(baseZoom, maxZoomOut, speedFactor);

        // Extra zoom när boost är aktiv
        if (heatBoost != null && heatBoost.IsBoosting)
        {
            targetZoom = Mathf.Lerp(targetZoom, boostZoomOut, 0.8f);

            // Trigger shake EN gång när boost startar
            if (!wasBoostingLastFrame)
            {
                shakeTimer = shakeDuration;
            }
        }

        wasBoostingLastFrame = heatBoost != null && heatBoost.IsBoosting;

        // Smooth zoom
        float currentZoom = cineCam.m_Lens.OrthographicSize;
        cineCam.m_Lens.OrthographicSize = Mathf.SmoothDamp(
            currentZoom, targetZoom, ref currentZoomVelocity, 1f / zoomSpeed
        );

        // Shake
        if (noise != null)
        {
            if (shakeTimer > 0f)
            {
                float shakeAmount = shakeIntensity * (shakeTimer / shakeDuration);
                noise.AmplitudeGain = shakeAmount; // fungerar i Unity 2021+
                shakeTimer -= Time.deltaTime;
            }
            else
            {
                noise.AmplitudeGain = 0f;
            }
        }

        // Smooth camera follow position
        Vector3 desiredPos = target.position;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref camOffsetVelocity, followLag);
    }
}
