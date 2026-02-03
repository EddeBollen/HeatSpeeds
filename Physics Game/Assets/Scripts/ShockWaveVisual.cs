using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ShockwaveVisual : MonoBehaviour
{
    private ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();

        // Tvinga AV vid start
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        gameObject.SetActive(false);
    }

    public void Play(float heat)
    {
        Debug.Log("ShockwaveVisual.Play() CALLED");

        gameObject.SetActive(true);

        var main = ps.main;
        main.startSizeMultiplier = Mathf.Lerp(0.6f, 1.6f, heat);
        main.startColor = Color.Lerp(Color.yellow, Color.red, heat);

        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.Play();

        float disableTime = main.duration + main.startLifetime.constantMax;
        Invoke(nameof(Disable), disableTime);
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }
}
