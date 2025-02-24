using UnityEngine;
using System.Collections.Generic;

public class ItemThrower : MonoBehaviour
{
    [Header("Throwing Settings")]
    public float trackingTime = 5f;
    public float maxThrowVelocity = 50f;
    public float minThrowVelocity = 1f;
    public float returnTime = 2f;
    public float targetAngle = 45f; // Desired throw angle

    [Header("References")]
    public LayerMask targetLayer;
    [SerializeField] private FishUI fishUI;
    
    private Camera mainCamera;
    private Rigidbody itemRigidbody;
    [SerializeField] private HandController handController;

    private bool isEquipped = false;
    private bool isHoldingItem = false;
    private bool isThrown = false;
    private List<Vector3> previousPositions = new List<Vector3>();
    private int ballCounter = 3;
    private Transform handLocation;
    private bool myHand; // LEFT = false, RIGHT = true

    // Fixed the assignment: assign the parameter value to myHand.
    public void Init(bool hand, Transform location, bool equipped)
    {
        myHand = hand;
        handLocation = location;
        isEquipped = equipped;
        DisableComponents();
    }

    void Start()
    {
        mainCamera = Camera.main;
        if (handController == null)
            handController = FindFirstObjectByType<HandController>();
        if (fishUI == null)
            fishUI = FindFirstObjectByType<FishUI>();

        itemRigidbody = GetComponent<Rigidbody>() ?? gameObject.AddComponent<Rigidbody>();
        itemRigidbody.useGravity = false;
        itemRigidbody.maxAngularVelocity = 100f;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isEquipped)
            TryPickUpBob();

        if (Input.GetMouseButtonUp(0) && isHoldingItem)
        {
            isHoldingItem = false;
            if (CalculateVelocity().magnitude >= minThrowVelocity)
                ThrowBob();
            else
                ResetBob();
        }

        if (isHoldingItem)
        {
            MoveBobToCursor();
            TrackMovement();
        }
            transform.position = Vector3.MoveTowards(transform.position, handLocation.position, Time.deltaTime * 10f);
        if (!isThrown && isEquipped)
            transform.position = Vector3.Lerp(transform.position, handLocation.position, Time.deltaTime * 10f);
    }

    void TryPickUpBob()
    {
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && hit.transform == transform)
        {
            isHoldingItem = true;
            itemRigidbody.isKinematic = true;
            previousPositions.Clear();
            trackingTime = 0f;
        }
    }

    void MoveBobToCursor()
    {
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, targetLayer))
            transform.position = hit.point;
    }

    void TrackMovement()
    {
        if (previousPositions.Count > 10) previousPositions.RemoveAt(0);
        previousPositions.Add(transform.position);
        trackingTime += Time.deltaTime;
    }

    Vector3 CalculateVelocity()
    {
        if (previousPositions.Count < 2 || trackingTime == 0f)
            return Vector3.zero;
        return (previousPositions[^1] - previousPositions[0]) / trackingTime;
    }

    void ThrowBob()
    {
        transform.SetParent(null);
        fishUI.UpdateBall(--ballCounter);
        itemRigidbody.isKinematic = false;
        itemRigidbody.useGravity = true;

        // Get the tracked average speed.
        Vector3 trackedVelocity = CalculateVelocity();
        float throwSpeed = Mathf.Clamp(trackedVelocity.magnitude, minThrowVelocity, maxThrowVelocity);

        // Use the camera's forward vector, but only its horizontal component.
        Vector3 horizontalDir = mainCamera.transform.forward;
        horizontalDir.y = 0;
        horizontalDir.Normalize();

        // Calculate the throw velocity based on the target angle.
        float angleInRad = targetAngle * Mathf.Deg2Rad;
        Vector3 throwVelocity = (horizontalDir * Mathf.Cos(angleInRad) + Vector3.up * Mathf.Sin(angleInRad)) * throwSpeed;

        itemRigidbody.linearVelocity = throwVelocity;

        isThrown = true;
        handController.ResetHand(myHand);
        Invoke(nameof(DestroyThrowable), returnTime);
    }

    void ResetBob()
    {
        transform.position = handLocation.position;
        itemRigidbody.isKinematic = true;
        itemRigidbody.useGravity = false;
        isThrown = false;
    }

    void DestroyThrowable()
    {
        Destroy(gameObject);
    }

    void DisableComponents()
    {
        if (GetComponent<ItemGrabber>()) GetComponent<ItemGrabber>().enabled = false;
        if (GetComponent<ItemCooker>()) GetComponent<ItemCooker>().enabled = false;
    }
}
