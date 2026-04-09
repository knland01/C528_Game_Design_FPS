using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public bool canDie = true;
    public Image healthBarFill;
    public float healRate = 5f;
    public float healDelay = 5f;
    public bool canRegenHealth = false; // Players and Sheriff

    private float lastDamageTime;
    private float currentHealth;
    private bool isDead;
    private GameObject lastAttacker;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead) return;
        // Only heal if enough time has passed since last damage
        if (!canRegenHealth) return;
        if (Time.time - lastDamageTime >= healDelay)
        {
            if (currentHealth < maxHealth)
            {
                currentHealth += healRate * Time.deltaTime;
                currentHealth = Mathf.Min(currentHealth, maxHealth);

                UpdateHealthBar();
            }
        }
    }

    public void TakeDamage(float amount, GameObject attacker)
    {
        if (isDead) // I am dead.
            return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f); // no neg HP vals
        UpdateHealthBar();

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

                //Debug.Log($"[PENALTY] {name} lost {penalty}");
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
    void UpdateHealthBar()
    {
        if (healthBarFill == null)
            return;

        float percent = currentHealth / maxHealth;

        healthBarFill.fillAmount = percent;

        if (percent < 0.3f)
        {
            healthBarFill.color = Color.red;
        }
        else if (percent < 0.6f)
        {
            healthBarFill.color = Color.yellow;
        }
        else
        {
            healthBarFill.color = Color.green;
        }
    }

}