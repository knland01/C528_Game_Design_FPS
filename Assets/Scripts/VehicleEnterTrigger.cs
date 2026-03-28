using UnityEngine; // imports Unity's core API

public class VehicleEnterTrigger : MonoBehaviour // Unity component -- attaches to a GameObject in the scene
{
    [Header("Refs")] // Attribute label listed in Inspector window
    public Transform frontSeatPoint;
    public Transform backSeatPoint;
    public Transform exitPoint;
    public Transform canopyBackHandle; // the thing that slides along the fuselage
    public Transform canopyClosedPoint; // empty transform = closed position
    public Transform canopyOpenPoint; // empty transform = open position
    public FlightControl flightControl;
    public Camera playerCamera;
    public Camera planeCamera;

    [Header("Settings")]
    public KeyCode interactKey = KeyCode.P;
    public float canopySlideSpeed = 4f; // units for second

    private PlayerMovement currentPlayerMove;   // the PlayerMovement script (or controller)
    private Transform player; // Will move the player to the seated position.
    private Transform currentSeat;
    private bool playerInRange;
    private bool inVehicle;



    void Update() // Unity callback - runs once per frame - while component enabled
    {
        if (Input.GetKeyDown(interactKey))
        {
            if (!inVehicle)
            {
                if (playerInRange)
                    EnterVehicle();
            }
            else if (inVehicle)
            {
                ExitVehicle();
            }
        }

        // Canopy logic
        if (canopyBackHandle != null)
        {
            if (playerInRange || inVehicle)
                OpenCanopy();
            else
                CloseCanopy();
        }
    }

    void OnTriggerEnter(Collider other) // Unity physics callback
    {
        if (!other.CompareTag("Player")) return; // We only care when player collider enters (not other NPCs for ex)
        playerCamera.enabled = false;
        planeCamera.enabled = true;
        player = other.transform;
        // Store a reference to the Transform component of the GameObject that entered the trigger (and is tagged "Player")
        currentPlayerMove = other.GetComponent<PlayerMovement>(); // script dynamically finds movement script for each player
        playerInRange = true; // we know the player is now in range
    }

    void OnTriggerExit(Collider other) 
        // If the player leaves the trigger zone - close the canopy (not exit key logic here)
    {
        if (!other.CompareTag("Player")) return;
        playerCamera.enabled = true;
        planeCamera.enabled = false;
        playerInRange = false;
        player = null;

        if (!inVehicle)
        {
            SnapCanopyClosed();
        }
    }

    void EnterVehicle()
    {
        // Guard: need a player reference and at least one seat
        if (player == null) return;
        if (frontSeatPoint == null && backSeatPoint == null) return;


        Transform chosenSeat = null;

        // FRONTSEAT = FILL FIRST --> BACKSEAT
        if (frontSeatPoint != null)
        {
            chosenSeat = frontSeatPoint;
        }
        else if (backSeatPoint != null)
        {
            chosenSeat = backSeatPoint;
        }

        // If both seats are taken, chosen seat is not assigned -- do nothing at this point
        if (chosenSeat == null) return;

        inVehicle = true;
        currentSeat = chosenSeat;

        // Disable player movement
        if (currentPlayerMove != null)
            currentPlayerMove.enabled = false;

        // Move player to seat + parent to seat so they follow the plane
        player.SetParent(chosenSeat, true);
        player.position = chosenSeat.position;
        player.rotation = chosenSeat.rotation;

        // mark occupant + assign pilot
        if (inVehicle)
        {
            print("Player is inVehicle!");

            if (flightControl != null)
            {
                print("Flight Control Enabled.");
                flightControl.enabled = true;
            }
        }

        // Optional: snap canopy open (otherwise proximity animation handles it)
        SnapCanopyOpen();
    }

    void ExitVehicle()
    {
        if (!inVehicle) return; // not currently in vehicle to exit iy

        inVehicle = false;

        // Disable vehicle controls
        if (flightControl != null)
            flightControl.enabled = false;

        // Unparent + move player out of the vehicle
        if (player != null)
        {
            player.SetParent(null, true);

            if (exitPoint != null)
            {
                player.position = exitPoint.position;
                player.rotation = exitPoint.rotation;
            }
        }

        // Re-enable player controls
        if (currentPlayerMove != null)
            currentPlayerMove.enabled = true;

        // Free the seat that was occupied
        if (currentSeat == frontSeatPoint)
        {

            if (flightControl != null)
            {
                flightControl.enabled = false;
            }

        }
        else if (currentSeat == backSeatPoint)
        {
        }
        currentSeat = null; // free the seat

        // Close canopy (snap). Animate later.
        CloseCanopy();
    }

    void OpenCanopy()
    {
        if (canopyBackHandle == null || canopyOpenPoint == null) return;

        canopyBackHandle.localPosition = Vector3.MoveTowards(
            canopyBackHandle.localPosition,
            canopyOpenPoint.localPosition,
            canopySlideSpeed * Time.deltaTime
        );
    }

    void CloseCanopy()
    {
        if (canopyBackHandle == null || canopyClosedPoint == null) return;

        canopyBackHandle.localPosition = Vector3.MoveTowards(
            canopyBackHandle.localPosition,
            canopyClosedPoint.localPosition,
            canopySlideSpeed * Time.deltaTime
        );
    }

    void SnapCanopyOpen()
    {
        if (canopyBackHandle == null || canopyOpenPoint == null) return;
        canopyBackHandle.localPosition = canopyOpenPoint.localPosition;
    }

    void SnapCanopyClosed()
    {
        if (canopyBackHandle == null || canopyClosedPoint == null) return;
        canopyBackHandle.localPosition = canopyClosedPoint.localPosition;
    }
}

    

