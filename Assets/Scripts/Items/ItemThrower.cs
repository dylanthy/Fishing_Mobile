using UnityEngine;
using System.Collections.Generic;

public class ItemThrower : MonoBehaviour
{
    [Header("Throwing Settings")]
    public float trackingTime = 5f;               // (Resets to 0 when picked up)
    public float maxThrowVelocity = 50f;
    public float minThrowVelocity = 1f;
    public float returnTime = 2f;                 // Time after throw before destruction
    public float targetAngle = 45f;               // Desired launch angle relative to horizontal

    [Header("References")]
    public LayerMask targetLayer;
    private Camera mainCamera;
    private Rigidbody itemRigidbody;
    [SerializeField] private HandController handController;

    // State variables
    private bool isEquipped = false;
    private bool isHoldingItem = false;
    private bool isThrown = false;
    
    // Movement tracking – we record the last 20 frames of “velocity”
    private List<Vector3> previousVelocities = new List<Vector3>();
    public Vector3 averageVelocity;             // Updated each frame while holding

    // Hand-related references
    private Transform handLocation;
    private bool myHand;                        // LEFT = false, RIGHT = true

    // For computing manual velocity while being held (kinematic)
    private Vector3 lastPosition;

    /// <summary>
    /// Initializes the throwable with the hand side and hand location.
    /// </summary>
    /// <param name="hand">true for right hand, false for left hand</param>
    /// <param name="location">The transform representing the hand’s location</param>
    public void Init(bool hand, Transform location)
    {
        myHand = hand;
        handLocation = location;
        isEquipped = true;
        DisableComponents();
    }

    void Start()
    {
        mainCamera = Camera.main;
        // Try to find the hand controller if not assigned via the inspector.
        if (handController == null)
            handController = FindFirstObjectByType<HandController>();

        itemRigidbody = GetComponent<Rigidbody>();
        if (itemRigidbody != null)
        {
            // Disable gravity while the item is held.
            itemRigidbody.useGravity = false;
        }
    }

    void Update()
    {
        // When equipped, check for mouse input to pick up or throw the item.
        if (isEquipped)
        {
            if (Input.GetMouseButtonDown(0))
            {
                TryPickUpItem();
            }
            if (Input.GetMouseButtonUp(0) && isHoldingItem)
            {
                isHoldingItem = false;
                ThrowItem();
            }
            else
            {
                ResetItem();
            }
        }

        // While holding, move the item to the cursor and track its movement.
        if (isHoldingItem)
        {
            MoveItemToCursor();
            TrackMovement();
        }

        // If the item is equipped but not yet thrown, smoothly move it to the hand position.
        if (!isThrown && isEquipped)
        {
            transform.position = Vector3.Lerp(transform.position, handLocation.position, 10f * Time.deltaTime);
        }
    }

    /// <summary>
    /// Moves the item along a raycast from the mouse cursor using the specified target layer.
    /// </summary>
    void MoveItemToCursor()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, targetLayer))
        {
            transform.position = hit.point;
        }
    }

    /// <summary>
    /// Tracks the movement of the item while being held by computing a manual velocity
    /// (using positional differences) and maintaining a rolling average of the last 20 frames.
    /// </summary>
    void TrackMovement()
    {
        // Calculate current “velocity” based on positional change
        Vector3 currentVelocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;

        // Add to our list – keep only the most recent 20 frames
        previousVelocities.Add(currentVelocity);
        if (previousVelocities.Count > 20)
        {
            previousVelocities.RemoveAt(0);
        }

        // Compute the average velocity from the stored values.
        Vector3 sum = Vector3.zero;
        foreach (Vector3 vel in previousVelocities)
        {
            sum += vel;
        }
        averageVelocity = sum / previousVelocities.Count;
    }

    /// <summary>
    /// Attempts to pick up the item if the mouse cursor is over it.
    /// </summary>
    void TryPickUpItem()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
        {
            isHoldingItem = true;
            itemRigidbody.isKinematic = true;
            previousVelocities.Clear();
            trackingTime = 0f;
            lastPosition = transform.position;
        }
    }

    /// <summary>
    /// Throws the item by detaching it from the hand, enabling physics, and applying a launch velocity.
    /// The launch velocity uses the tracked average speed (clamped between min and max) and adjusts the upward angle.
    /// It now factors in the ball's left/right movement (horizontal component) relative to the camera.
    /// </summary>
    void ThrowItem()
    {
        // Detach the item from its parent (the hand)
        transform.parent = null;
        // Enable physics simulation.
        itemRigidbody.isKinematic = false;
        itemRigidbody.useGravity = true;

        // Use the tracked average velocity magnitude as our speed (clamped between min and max).
        float speed = Mathf.Clamp(averageVelocity.magnitude, minThrowVelocity, maxThrowVelocity);

        // Determine horizontal direction from the tracked average velocity.
        Vector3 horizontalDir = averageVelocity;
        horizontalDir.y = 0;
        if (horizontalDir.magnitude < 0.1f)
        {
            // Fallback to the camera's forward if the movement isn't significant.
            horizontalDir = mainCamera.transform.forward;
            horizontalDir.y = 0;
            horizontalDir.Normalize();
        }
        else
        {
            horizontalDir.Normalize();
        }

        // Calculate the throw velocity.
        // The horizontal component is along the determined horizontalDir,
        // and the vertical component is adjusted so that the launch angle equals targetAngle.
        Vector3 throwVelocity = horizontalDir * speed;
        throwVelocity.y = speed * Mathf.Tan(targetAngle * Mathf.Deg2Rad);

        // Apply the computed velocity.
        itemRigidbody.linearVelocity = throwVelocity;

        // Calculate the corrected launch angle from the velocity vector.
        float horizontalSpeed = new Vector2(throwVelocity.x, throwVelocity.z).magnitude;
        float correctedLaunchAngle = Mathf.Atan2(throwVelocity.y, horizontalSpeed) * Mathf.Rad2Deg;

        // Debug output: Log the launch velocity magnitude and corrected launch angle.
        Debug.Log("Launch Velocity Magnitude: " + throwVelocity.magnitude + ", Corrected Launch Angle: " + correctedLaunchAngle);

        isThrown = true;
        // Inform the hand controller that the hand is free.
        handController.ResetHand(myHand);
        // Schedule destruction of the item after returnTime seconds.
        Invoke("DestroyThrowable", returnTime);
        // Re-enable any components that were disabled.
        EnableComponents();
    }

    /// <summary>
    /// Resets the item’s position to the hand’s location when not held or thrown.
    /// </summary>
    void ResetItem()
    {
        if (!isHoldingItem && !isThrown)
        {
            transform.position = handLocation.position;
        }
    }

    /// <summary>
    /// Destroys the throwable item.
    /// </summary>
    void DestroyThrowable()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Disables additional components (if present) such as ItemGrabber or ItemCooker.
    /// </summary>
    void DisableComponents()
    {
        ItemGrabber grabber = GetComponent<ItemGrabber>();
        if (grabber != null)
            grabber.enabled = false;
        ItemCooker cooker = GetComponent<ItemCooker>();
        if (cooker != null)
            cooker.enabled = false;
    }

    /// <summary>
    /// Enables additional components that were disabled.
    /// </summary>
    void EnableComponents()
    {
        ItemGrabber grabber = GetComponent<ItemGrabber>();
        if (grabber != null)
            grabber.enabled = true;
        ItemCooker cooker = GetComponent<ItemCooker>();
        if (cooker != null)
            cooker.enabled = true;
    }
}
