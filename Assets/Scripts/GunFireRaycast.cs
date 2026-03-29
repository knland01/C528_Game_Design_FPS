using UnityEngine;

public class GunHitscan : MonoBehaviour
{
    [Header("Player")]
    public Transform owner;

    [Header("Refs")]
    public Transform muzzle;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;

    [Header("Input")]
    public KeyCode fireKey = KeyCode.Space;

    [Header("Fire")]
    public float range = 50f;
    public float fireCooldown = 0.15f;
    public float damage = 10f;

    [Header("Hit Filtering")]
    // Controls what the ray is allowed to hit
    //public LayerMask hitMask = ~0;

    // Tracks when you're allowed to shoot again
    [SerializeField] private float nextFireTime;


    void Start()
    {
        // Make sure the player doesn't hit self
        //hitMask = ~LayerMask.GetMask("Player");
    }

    void Update()
    {
        if (Input.GetKeyDown(fireKey))
            Debug.Log($"{name}: fire pressed");

        if (muzzle == null)
            return;

        // Cooldown check
        if (Time.time < nextFireTime)
            return;

        if (!Input.GetKeyDown(fireKey))
            return;

        Shoot();
        nextFireTime = Time.time + fireCooldown;
    }

    private void Shoot()
    {
        // Play Muzzle Flash
        if (muzzleFlash != null)
            muzzleFlash.Play();

        Ray ray = new Ray(muzzle.position, muzzle.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, range);

        // Sort hits by distance (important!)
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        bool hitSomething = false;

        foreach (RaycastHit hit in hits)
        {
            Debug.Log(
                $"HIT: {hit.collider.name} | " +
                $"ROOT: {hit.collider.transform.root.name} | " +
                $"MY ROOT: {transform.root.name}"
            );

            // Skip self (your own player or attachments)
            if (hit.collider.transform.IsChildOf(owner))
                continue;

            

            // Try to damage
            Health hp = hit.collider.GetComponentInParent<Health>();
            if (hp != null)
            {
                hitSomething = true;
                hp.TakeDamage(damage);
                break; // stop after first valid hit
            }
        }

        // Draw debug ray
        Debug.DrawLine(
            muzzle.position,
            muzzle.position + muzzle.forward * range,
            Color.red,
            hitSomething ? 1f : 0.1f
        );
    }
}