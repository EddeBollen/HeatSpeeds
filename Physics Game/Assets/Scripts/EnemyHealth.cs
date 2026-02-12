using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHP = 1;
    private int currentHP;

    private ShaderDamageFlash flash;

    void Awake()
    {
        currentHP = maxHP;
        flash = GetComponent<ShaderDamageFlash>();
        if (flash == null)
            Debug.LogWarning("ShaderDamageFlash missing on " + gameObject.name);
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;

        // Trigger flash
        if (flash != null)
            flash.Flash();

        if (currentHP <= 0)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
