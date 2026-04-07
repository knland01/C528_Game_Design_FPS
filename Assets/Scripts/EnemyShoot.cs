using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    public float attackRange = 10f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireForce = 20f;
    public float fireCooldown = 1.5f;
    public ParticleSystem muzzleFlash;
    public float damage = 5f;
    public float aimNPC = 0.2f; 

    private float lastFireTime;
    private Transform player;
    private Transform owner;
    private Animator animator;
    private EnemyRaycast weaponRay;

    void Start()
    {
        owner = transform.root;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        weaponRay = GetComponent<EnemyRaycast>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        player = GetClosestPlayer();

        if (player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < attackRange)
        {
            animator.SetBool("isShooting", true);
            Attack();
        }
        else
        {
            animator.SetBool("isShooting", false);
        }
    }

    Transform GetClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject p in players)
        {
            float dist = Vector3.Distance(transform.position, p.transform.position);

            if (dist < minDistance)
            {
                minDistance = dist;
                closest = p.transform;
            }
        }

        return closest;
    }

    void Attack()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;                    // flatten first
        direction = direction.normalized;   // THEN normalize

        // Smooth rotation instead of snapping
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * 5f
        );

        // makes sure enemy doesn't shoot every frame
        if (Time.time > lastFireTime + fireCooldown) 
        {
            Shoot();
            lastFireTime = Time.time; // record time for cooldown
        }
    }

    void Shoot()
    {
        // Get center of player
        Collider playerCollider = player.GetComponent<Collider>(); 
        Vector3 targetPoint = playerCollider.bounds.center;

        // Get Perfect Shot direction
        Vector3 perfectShot = (targetPoint - firePoint.position).normalized;

        // Get Distance to player (drives accuracy)
        float distance = Vector3.Distance(firePoint.position, player.position);


        // ____ [ DISTANCE BASED AIM ] __________________________
        // Scale spread based on distance
        float accuracyFactor = distance / attackRange;
        // close --> accFact near 0 | far --> accFact near 1

        // Clamp -- keeps value btwn (0, 1)
        accuracyFactor = Mathf.Clamp01(accuracyFactor);

        // Final spread shrinks when close
        float horizontalSpread = aimNPC * accuracyFactor;
        float verticalSpread = (aimNPC * 0.5f) * accuracyFactor; // smaller vertical spread

        // Randomized innacuracy -- based on distance & aimNPC
        Vector3 spread = new Vector3(
            Random.Range(-horizontalSpread, horizontalSpread),
            Random.Range(-verticalSpread, verticalSpread),
            0f
        );

        // -------------------------------------------------------

        Vector3 shotDirection = (perfectShot + spread).normalized;

        if (muzzleFlash != null)
            muzzleFlash.Play();

        // --- RAYCAST (damage) ---
        Ray ray = new Ray(firePoint.position, shotDirection);

        // Hits everything in range
        RaycastHit[] hits = Physics.RaycastAll(ray, attackRange);
        // Sort by distance
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            // skips hitting self
            if (hit.collider.transform.IsChildOf(transform.root))
                continue;

            // Looks for health script in any hit objects and applies damage
            Health target = hit.collider.GetComponentInParent<Health>();
            if (target != null)
            {
                target.TakeDamage(damage, gameObject);
                break;
            }
        }

        // --- BULLET (visual only) ---
        // spawns bullet & rotates to shooting direction
        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.LookRotation(shotDirection)
        );
        // gives bullet movement for visual and follows raycast
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = shotDirection * fireForce;
    }
}
