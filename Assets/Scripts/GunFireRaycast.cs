using UnityEngine;

public class GunHitscan : MonoBehaviour
{
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
    public LayerMask hitMask = ~0;

    // Tracks when you're allowed to shoot again
    [SerializeField] private float nextFireTime;


    void Start()
    {
        // Make sure the player doesn't hit self
        hitMask = ~LayerMask.GetMask("Player");
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

        // Did the ray hit something?
        if (Physics.Raycast(
                ray,
                out RaycastHit hit,
                range,
                hitMask,
                QueryTriggerInteraction.Ignore))
        {
            Debug.Log($"HIT: {hit.collider.name}"); // What it hit?

            // Ignore the player if shot fired inside own collider
            if (hit.collider.transform.root == transform.root) // hit.collider = the thing you hit
                return;

            Health hp = hit.collider.GetComponentInParent<Health>();
            if (hp != null)
                hp.TakeDamage(damage);

            Debug.DrawLine(muzzle.position, muzzle.position + muzzle.forward * range, Color.red, 1f);
        }
        else
        {
            Debug.DrawLine(
                muzzle.position,
                muzzle.position + muzzle.forward * range,
                Color.red,
                0.1f
            );
        }
    }
}