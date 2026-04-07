using UnityEngine;

public class EnemyRaycast : MonoBehaviour
{
    public Transform firePoint;
    public float range = 20f;
    public float damage = 20f;
    //public ParticleSystem muzzleFlash;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    public void Fire()
    {
        //if (muzzleFlash != null)
        //    muzzleFlash.Play();

        Vector3 targetPoint = player.position + Vector3.up * 1.0f; // aim at player chest

        Vector3 direction = (targetPoint - firePoint.position).normalized;

        Ray ray = new Ray(firePoint.position, direction);

        // Collect hit objects
        RaycastHit[] hits = Physics.RaycastAll(ray, range);
        // sort by distance
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            Transform root = hit.collider.transform.root;

            Debug.Log(
                $"ENEMY HIT: {hit.collider.name} | " +
                $"ROOT: {hit.collider.transform.root.name} | " +
                $"MY ROOT: {transform.root.name}"
            );

            // skip self
            if (root == transform.root)
                continue;

            Health target = root.GetComponent<Health>();
            if (target != null)
            {
                Debug.Log($"Applying damage to {root.name}");
                target.TakeDamage(damage, gameObject);
                break; // stop after first valid hit
            }

        }
    }
}
