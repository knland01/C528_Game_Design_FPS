using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float wanderRadius = 5f; // random location it picks to walk towards within radius from self
    public float obstacleRange = 2f;
    public float stopDistance = 0.5f;

    private EnemyShoot shooter;
    private Transform player;
    private Vector3 targetPosition;
    private Animator animator; // reference to animator component attached to enemy object
    private Vector3 moveDirection;
    private float avoidUntilTime = 0f;



    void Start()
    {
        shooter = GetComponent<EnemyShoot>();
        // Initial movement direction = forward
        moveDirection = transform.forward;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>(); // get the animator component - store here for use in script
        PickNewTarget(); // enemy picks random point around self
    }

    void Update()
    {
        MoveToTarget();
    }

    void PickNewTarget() // modifies class variable directly / internally
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized * wanderRadius;
        // normalized pushes the randomly selected point to the edge of the circle

        targetPosition = new Vector3(
            transform.position.x + randomCircle.x,  // LEFT / RIGHT
            transform.position.y,                   // HEIGHT remains unchanged
            transform.position.z + randomCircle.y   // FORWARD / BACKWARD
        );
    }

    private void MoveToTarget()
    {
        MoveToTarget(moveSpeed);
    }

    void MoveToTarget(float moveSpeed)
    {
        //Debug.Log("isWalking: " + animator.GetBool("isWalking"));
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // CHASE + ATTACK BEHAVIOR
        if (distanceToPlayer < shooter.attackRange)
        {
            //animator.SetBool("isWalking", true);
            Vector3 direction = player.position - transform.position;
            direction.y = 0;

            //// OBSTACLE CHECK WHILE CHASING
            //Ray chaseRay = new Ray(transform.position, direction.normalized);
            //RaycastHit chaseHit;

            //if (Physics.SphereCast(chaseRay, 0.1f, out chaseHit, obstacleRange))
            //{
            //    animator.SetBool("isWalking", false);

            //    float angle = Random.Range(90f, 160f);
            //    float sign = Random.value < 0.5f ? -1f : 1f;

            //    transform.Rotate(0, angle * sign, 0);

            //    // Smooth transition away from obstacle and back towards player
            //    moveDirection = transform.forward;
            //    avoidUntilTime = Time.time + 0.5f;
            //    return;
            //}

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * 5f
            );

            if (distanceToPlayer > stopDistance)
            {
                //Debug.Log("Distance: " + distanceToPlayer);
                animator.SetBool("isWalking", true);
                //animator.Play("Walk");
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            }
            else
            {
                animator.SetBool("isWalking", false);
            }

            return; // IMPORTANT: prevents wandering logic from running
        }

        // === 1. OBSTACLE DETECTION ===
        Ray ray = new Ray(transform.position, moveDirection);
        RaycastHit hit;

        if (Physics.SphereCast(ray, 0.1f, out hit, obstacleRange))
        {
            //animator.SetBool("isWalking", false);

            // Pick a NEW direction (but not crazy random)
            float angle = Random.Range(90f, 160f);
            float sign = Random.value < 0.5f ? -1f : 1f;

            transform.Rotate(0, angle * sign, 0);

            moveDirection = transform.forward;

            return;
        }

        // === 2. MOVE FORWARD ===
        animator.SetBool("isWalking", true);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(moveDirection),
            Time.deltaTime * 3f
        );

        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }
}