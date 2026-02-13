using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    private int currentHealth;
    private bool isInvincible = false;

    private ShaderDamageFlash flash;

    void Awake()
    {
        currentHealth = maxHealth;
        flash = GetComponent<ShaderDamageFlash>();
        if (flash == null)
            Debug.LogWarning("ShaderDamageFlash missing on Player");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            isInvincible = !isInvincible; // Växla true/false
            Debug.Log("Invincible: " + isInvincible);
        }
    }

    public void TakeDamage(int damage)
    {

        if (isInvincible)
        {
            Debug.Log("Player is invincible!");
            return; // Stoppar funktionen här
        }

        currentHealth -= damage;
        Debug.Log("PLAYER TOOK DAMAGE! HP = " + currentHealth);

        if (flash != null)
            flash.Flash();

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("PLAYER DIED");
        Destroy(gameObject, 0.3f);
    }
}
