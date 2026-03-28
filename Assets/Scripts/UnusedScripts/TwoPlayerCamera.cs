using UnityEngine; // imports Unity's core types: Transform, Vector3, Camera, Time, Mathf

// THIS Script attaches to: Main Camera for gameplay view
// ... Two player camera adjusts FOV as players move apart and closer together

public class TwoPlayerCamera : MonoBehaviour 
    // MonoBehaviour: Script inherits from this base class -- allows you to attach script to GameObjects
    // ... allows Unity to implicitly call event functions for your script: Start(), Update(), LateUpdate(), Awake()
{
    [Header("Targets")] // Adds a label in the Unity Inspector window for the public field
    public Transform player1; // Transform: Every GameObject in Unity has a transform component:
                              // ... with attributes: Position | Rotation | Scale
                              // ... ex: transform.position
    public Transform player2;
    // When you drag a player object into this field, you're storing a reference to the object's Transform component - not the GameObject.

    [Header("Follow")]
    public Vector3 offset = new Vector3(0f, 8f, -12f); // Vector3 = struct (custom data type) defined by Unity
                                                       // ... represents 3 FP #s: x, y, z (3D Vector) = any 3D quantity
                                                       // ... such as: Position, Direction, Velocity Force, Scale
                                                       // Offset = camera's positional offset from the midpoint (between P1 & P2)
                                                       // f = float literal
                                                       // 0f (x | horizontal Position) = don't shift left/right relative to midpoint
                                                       // 8f (y | vertical Position) = move camera 8 units up
                                                       // -12f (z | depth Position) = move camera 12 units back
    public float followSmoothTime = 0.15f; // Smoothing parameter - how long it takes to approach target position
                                           // ... Used inside Vector3.SmoothDamp(..)

    [Header("Zoom (FOV)")]
    public float minFOV = 50f; // Camera's viewing angle in degrees 
    public float maxFOV = 75f;
    public float zoomSmoothSpeed = 5f;

    [Tooltip("Distance between players that map to maxFOV,")]
    public float maxDistanceForZoom = 25f;

    private Vector3 velocity; // internal helper variable managed by Vector3.SmoothDamp(..)
                              // ... to keep track of how fast camera is moving so it can simulate spring-like motion
    private Camera cam;       // Stores reference to Camera component attached to THIS GameObject (THIS: GameObject this script is attached to)

    void Awake() // Unity lifecycle method - user defined here 
                 // ... Not called explicitly in the script, just defined here 
                 // ... When the Unity engine sees the MonoBehaviour script contains a method by this name...
                 // ... ... it is called automatically during the scene initialization phase.  
    {
        cam = GetComponent<Camera>(); // This is a Generic method: 
                                      // ... <Camera>: tells Unity Engine - Search for component of type Camera
                                      // ... All together: this is saying get the Camera component of the GameObject this script is attached to
                                      // ... ... and assign it to cam
        if (cam == null)
        {
            Debug.LogError("TwoPlayerCamera requires a Camera component.");
        }
    }

    void LateUpdate() // Unity calls this once per frame, after all Update() calls
                      // Players move in Update() <-- Unity doesn't gaurantee order without explicit ordering when in Update()
                      // Camera reacts afterward -- prevents jjjitter by ensuring player moves then camera moves for each frame
    
    // BELOW: Per Frame camera logic
    {
        if (player1 == null || player2 == null ||  cam == null)
        {
            return;
        }

        // Calculate midpoint between players
        Vector3 mid = (player1.position + player2.position) * 0.5f;

        // Smooth follow
        Vector3 targetPos = mid + offset; // calculate new camera targer position
        transform.position = Vector3.SmoothDamp(
            transform.position, // Vector3 current: current camera position
                                // ... accesses the transform.position attribute of the GameObject this script is attached to
            targetPos,          // Vector3 target
            ref velocity,       // ref Vector3 currentVelocity -- reference so that it can update this value out of scope
                                // ... or so that the change to velocity can persist between frames
            followSmoothTime    // float smoothTime
        );

        // Keep camera looking at midpoint
        transform.LookAt(mid);

        // Zoom based on distance between players
        float dist = Vector3.Distance(player1.position, player2.position);
        float t = Mathf.Clamp01(dist / maxDistanceForZoom);

        float targetFOV = Mathf.Lerp(minFOV, maxFOV, t);
        cam.fieldOfView = Mathf.Lerp(
            cam.fieldOfView,
            targetFOV,
            Time.deltaTime * zoomSmoothSpeed
        );
    }
}