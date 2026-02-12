using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    private int currentHealth;

    private ShaderDamageFlash flash;

    void Awake()
    {
        currentHealth = maxHealth;
        flash = GetComponent<ShaderDamageFlash>();
        if (flash == null)
            Debug.LogWarning("ShaderDamageFlash missing on Player");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("PLAYER TOOK DAMAGE! HP = " + currentHealth);

        // Trigger flash
        if (flash != null)
            flash.Flash();

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("PLAYER DIED");
        Destroy(gameObject);
    }
}
