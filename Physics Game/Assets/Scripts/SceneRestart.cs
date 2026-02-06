using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartIfPlayerGone : MonoBehaviour
{
    bool restarting = false;

    void Update()
    {
        if (!restarting && GameObject.FindWithTag("Player") == null)
        {
            restarting = true;
            StartCoroutine(Restart());
        }
    }

    IEnumerator Restart()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
