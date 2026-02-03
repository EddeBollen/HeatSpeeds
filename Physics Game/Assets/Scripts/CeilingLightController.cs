using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering.Universal;

public class CeilingLightController : MonoBehaviour
{
    public Light2D ceilingLight;
    public Color normalColor = Color.white;
    public Color alertColor = Color.red;
    public float rotationSpeed = 120f;
    public Transform spotlight;

    void Update()
    {
        if (GlobalAlertSystem.Instance == null) return;

        if (GlobalAlertSystem.Instance.isAlert)
        {
            if (ceilingLight != null)
                ceilingLight.color = alertColor;

            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
        else
        {
            if (ceilingLight != null)
                ceilingLight.color = normalColor;
            spotlight.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
