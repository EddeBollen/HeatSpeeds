using UnityEngine;

public class GlobalAlertSystem : MonoBehaviour
{
    public static GlobalAlertSystem Instance;
    public bool isAlert = false;

    private void Awake()
    {
        Instance = this;
    }

    public void TriggerAlert()
    {
        isAlert = true;
    }
    public void StopAlert()
    {
        isAlert = false;
    }
}
