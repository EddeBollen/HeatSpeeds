using UnityEngine;
using UnityEngine.Rendering.Universal; // Rätt namespace för 2D Lights

public class HeatTrail : MonoBehaviour
{
    [Header("Particle System")]
    public ParticleSystem trailParticles;
    public float maxEmissionRate = 60f;

    [Header("Freeform 2D Light Settings")]
    public Light2D freeformLight;          // Din Freeform Light 2D
    public float maxIntensity = 1.5f;
    public Color minColor = Color.blue;
    public Color maxColor = Color.red;
    public float lightSmoothSpeed = 3f;

    [Header("Heat Reference")]
    public PlayerHeatBoost playerHeatBoost;
    public float activationThreshold = 0.95f;

    private ParticleSystem.EmissionModule emission;

    void Start()
    {
        // Init Particle System
        emission = trailParticles.emission;
        emission.enabled = false;

        // Init Light
        if (freeformLight != null)
        {
            freeformLight.intensity = 0f;
            freeformLight.color = minColor;
        }
    }

    void Update()
    {
        float heat = playerHeatBoost.heat;

        // -------- PARTICLE SYSTEM --------
        emission.enabled = heat >= activationThreshold;
        if (emission.enabled)
        {
            emission.rateOverTime = heat * maxEmissionRate;
        }

        // -------- FREEFORM LIGHT --------
        if (freeformLight != null)
        {
            float targetIntensity = heat * maxIntensity;
            freeformLight.intensity = Mathf.Lerp(freeformLight.intensity, targetIntensity, Time.deltaTime * lightSmoothSpeed);

            // Gradvis färgändring
            freeformLight.color = Color.Lerp(freeformLight.color, Color.Lerp(minColor, maxColor, heat), Time.deltaTime * lightSmoothSpeed);
        }
    }
}
