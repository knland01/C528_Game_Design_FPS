using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
            return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f);

        Debug.Log($"{name} took {amount} damage. HP={currentHealth}");

        if (currentHealth <= 0f)
            Die();
    }
    private void Die()
        {
            isDead = true;

            Debug.Log($"{name} has died.");

            gameObject.SetActive(false);

        }
    
}