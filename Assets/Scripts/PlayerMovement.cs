using UnityEngine;

[RequireComponent(typeof(CharacterController))]
/*
[ ] above a class = attribute
... Attributes are metadata - modify behavior of thing below them

RequireComponent: Unity-native attribute - that takes constructor arguments
... Tells Unity that attached script depends on another component
... It's actually a class that inherits from Attribute
... B/C it inherits from Attribut, the compiler lets you use it in square brackets

typeof(..): C# operator
... Returns Type Object of thing inside (..)
 
CharacterController component: Unity-native component designed for moving player characters
... HANDLES:
... -- Collision Detection, Sliding along walls, stepping up ledges, detecting ground, prevents passing thru geom.
... -- Does NOT use Unity's physics engine for movement.
... -- For shooters / character games - CharacterController preferred

[RequireComponent(typeof(CharacterController))]: 
-- ENGLISH - UNDER THE HOOD: "Create an instance of the RequireComponent attribute class,
                                ... passing in this Type."
-- ENGLISH - GOAL: The attached GameObject must have a CharacterController for this script to work.
*/
public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    private bool hasAnimator;

    [Header("Movement Keys")] // public in Inspector so works for both P1 and P2 -- just use diff keys
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;

    [Header("Rotation Keys")]
    public KeyCode lturnKey = KeyCode.Q;
    public KeyCode rturnKey = KeyCode.E;

    [Header("Jump Key")]
    public KeyCode jumpKey = KeyCode.LeftShift;

    [Header("Rotation")]
    public float turnSpeed = 120f;

    [Header("Settings")]
    public float moveSpeed = 6f;
    public float gravity = -9.8f;
    public float jumpHeight = 1.5f;
    public float maxJumpHeight = 5f;

    private CharacterController controller;
    private Vector3 velocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        hasAnimator = animator != null;
    }

    void Update() // Runs every frame
    {
        bool isMoving = false; // check for animation trigger
        Vector3 move = Vector3.zero;

        // WASD
        if (Input.GetKey(forwardKey))
            move += transform.forward;

        if (Input.GetKey(backKey))
            move -= transform.forward;

        if (Input.GetKey(leftKey))
            move -= transform.right;

        if (Input.GetKey(rightKey))
            move += transform.right;

        // ROTATION
        if (Input.GetKey(lturnKey))
            transform.Rotate(Vector3.up * -turnSpeed * Time.deltaTime);
        // Time.deltaTime: System based time between last frame in current frame 
        // ... Scales movement basedon how long the last frame took
        // MATH BREAKDOWN: 60 FPS -> 0.0167 SPF --> 3M/sec --> 3 * SPF = distance per frame
        if (Input.GetKey(rturnKey))
            transform.Rotate(Vector3.up * turnSpeed * Time.deltaTime);
        // ROTATION - (x, y, z) = (pitch, yaw, roll)
        // - xPITCH = Look up / down
        // - yYAW = turn L/R
        // - zROLL = tilt L/R
        // Vector3.up = (0f, 1f, 0f) <-- it's a complete vector defined like this


        move = move.normalized; // removes scaling with stacked movement keys - keeping speed consistent

        isMoving = move.magnitude > 0f;
        if (hasAnimator)
        {
            animator.SetBool("isWalking", isMoving);
        }

        controller.Move(move * moveSpeed * Time.deltaTime); // Where character is physically moved

        // Grounded reset
        //if (controller.isGrounded && velocity.y < 0f)
        //    velocity.y = -2f;
        //print(controller.isGrounded);

        // Jump
        //if (controller.isGrounded && Input.GetKeyDown(jumpKey))
        if (Input.GetKeyDown(jumpKey) && transform.position.y < maxJumpHeight)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        // gravity
        //if (controller.isGrounded && velocity.y < 0)
        //    velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime); // Also where character is moved (vertically)
    }
}