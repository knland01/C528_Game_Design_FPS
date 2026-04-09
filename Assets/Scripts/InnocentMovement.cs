using UnityEngine;

public class InnocentMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float changeDirectionTime = 3f;
    public float idleChance = 0.3f;

    private Vector3 moveDirection;
    private float timer;

    private Animator anim;

    void Start()
    {
        PickNewDirection();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            PickNewDirection();
        }

        // Move
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        // Face direction
        if (moveDirection != Vector3.zero)
        {
            transform.forward = moveDirection;
        }

        // Animation
        if (anim != null)
        {
            bool isWalking = moveDirection != Vector3.zero;
            anim.SetBool("isWalking", isWalking);
        }
    }

    void PickNewDirection()
    {
        timer = changeDirectionTime;

        // Random idle
        if (Random.value < idleChance)
        {
            moveDirection = Vector3.zero;
            return;
        }

        float x = Random.Range(-1f, 1f);
        float z = Random.Range(-1f, 1f);

        moveDirection = new Vector3(x, 0f, z).normalized;
    }
}