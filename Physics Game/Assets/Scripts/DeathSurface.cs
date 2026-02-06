using UnityEngine;

public class DeathSurface : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Destroy(collision.gameObject); // Döda spelaren
        }
    }
}
