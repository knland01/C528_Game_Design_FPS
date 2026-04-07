using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public bool canDie = true;

    private float currentHealth;
    private bool isDead;
    private GameObject lastAttacker;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount, GameObject attacker)
    {
        if (isDead) // I am dead.
            return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f); // no neg HP vals

        lastAttacker = attacker; // used for killshot + Kill Points

        Debug.Log($"{name} took {amount} damage. HP={currentHealth}");

        // --- SCORE LOGIC (ADD THIS HERE) ---
        RoleHandler myRole = GetComponent<RoleHandler>();
        ScoreSystem attackerScore = attacker.GetComponentInParent<ScoreSystem>();

        if (myRole != null && attackerScore != null)
        {
            // ATTACKER RECEIVES POINTS
            int points = ScoreRules.GetHitScore(myRole.role);
            attackerScore.AddScore(points);

            Debug.Log(
                $"Attacker: {attacker.name} | " +
                $"Role: {myRole?.role} | " +
                $"Points scored: {points}"
            );

            // VICTIM IS PENALIZED
            ScoreSystem myScore = GetComponentInParent<ScoreSystem>();
            RoleHandler attackerRole = attacker.GetComponentInParent<RoleHandler>();

            if (myRole != null && myScore != null)
            {
                int penalty = ScoreRules.GetHitByPenalty(attackerRole.role);
                myScore.AddScore(penalty);

                Debug.Log($"[PENALTY] {name} lost {penalty}");
            }

        }
        // ----------------------------------

        if (currentHealth <= 0f && canDie)
        {
            Die();
        }
    }
    private void Die()
    {
        if (isDead) return; // no dying again!

        isDead = true;
        Debug.Log($"{name} has died.");

        GameObject killer = lastAttacker;

        RoleHandler myRole = GetComponent<RoleHandler>(); // role of the deceased
        ScoreSystem attackerScore = killer?.GetComponentInParent<ScoreSystem>();
        //killer? - only cont if killer is not null.

        if (myRole != null && attackerScore != null)
        {
            int points = ScoreRules.GetKillScore(myRole.role);
            attackerScore.AddScore(points);

            Debug.Log($"[KILL] {killer.name} killed {myRole.role} for {points}");
        }

        gameObject.SetActive(false);

    }
    
}