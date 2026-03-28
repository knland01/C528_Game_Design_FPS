using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Bullet speed
    public float speed = 20f;
    // How long before deletes itself
    public float lifetime = 5f;
    // Damage delt
    public float damage = 10f;

    // Reference to physics component attached to bullet.
    private Rigidbody rb;

    void Awake() // runs as soon as the object is created
    {
        // Find the Rigidbody on THIS bullet and store it in rb
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // Velocity = direction * speed
        rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Ignore self
        if (other.transform.root == transform.root)
            return;

        Health hp = other.GetComponentInParent<Health>();
        if (hp != null)
        {
            Debug.Log($"PROJECTILE HIT: {hp.name}");
            hp.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
