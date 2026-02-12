using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class ShaderDamageFlash : MonoBehaviour
{
    [Header("Shader Properties")]
    public string blendProperty = "_Blend";    // Property som styr flashens styrka
    public string opacityProperty = "_Opacity"; // Property som styr flashens opacity

    [Header("Flash Settings")]
    [Range(0f, 5f)] public float flashBlendValue = 1f;
    [Range(0f, 5f)] public float flashOpacityValue = 1f;
    public float flashTime = 0.08f; // Hur snabbt flashen försvinner

    private Material mat;
    private Coroutine routine;

    void Awake()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            // Skapa en unik material-instans för detta objekt
            mat = Instantiate(sr.material);
            sr.material = mat;
        }
        else
        {
            Debug.LogError("No SpriteRenderer found on " + gameObject.name);
        }
    }

    public void Flash()
    {
        if (mat == null) return;

        if (routine != null)
            StopCoroutine(routine);
        routine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // Sätt materialet på max flash
        mat.SetFloat(blendProperty, flashBlendValue);
        mat.SetFloat(opacityProperty, flashOpacityValue);

        float t = 0f;
        while (t < flashTime)
        {
            float v = Mathf.Lerp(1f, 0f, t / flashTime);
            mat.SetFloat(blendProperty, v * flashBlendValue);
            mat.SetFloat(opacityProperty, v * flashOpacityValue);
            t += Time.deltaTime;
            yield return null;
        }

        // Avsluta på 0
        mat.SetFloat(blendProperty, 0f);
        mat.SetFloat(opacityProperty, 0f);
    }
}
