//using UnityEngine;
//using UnityEngine.Rendering.Universal;

//public class EnemySpotlight : MonoBehaviour
//{
//    public Light2D spotlight;
//    public float alertDuration = 10f;

//    private bool isAlert = false;
//    private float alertTimer = 0f;

//    private void Start()
//    {
//        if (spotlight == null)
//            spotlight = GetComponent<Light2D>();

//        GlobalAlertSystem.OnAlertActivated += BecomeAlert;
//        GlobalAlertSystem.OnAlertStopped += StopAlert;
//    }

//    private void OnDestroy()
//    {
//        GlobalAlertSystem.OnAlertActivated -= BecomeAlert;
//        GlobalAlertSystem.OnAlertStopped -= StopAlert;
//    }

//    private void Update()
//    {
//        if (isAlert)
//        {
//            alertTimer += Time.deltaTime;
//            if (alertTimer >= alertDuration)
//            {
//                alertTimer = 0f;
//                StopAlert();
//            }
//        }
//    }

//    private void BecomeAlert()
//    {
//        isAlert = true;
//        alertTimer = 0f;
//        if (spotlight != null)
//            spotlight.color = Color.red;
//    }

//    private void StopAlert()
//    {
//        isAlert = false;
//        if (spotlight != null)
//            spotlight.color = Color.white;
//    }
//}
