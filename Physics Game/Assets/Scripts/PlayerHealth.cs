using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log("Player HP: " + currentHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("PLAYER TOOK DAMAGE! HP = " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("PLAYER DIED");
        Destroy(gameObject);
    }
}
