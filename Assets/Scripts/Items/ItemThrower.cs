using UnityEngine;
using System.Collections.Generic;

public class ItemThrower : MonoBehaviour
{
    [Header("Throwing Settings")]
    public float throwForceMultiplier = 5f;
    public float maxThrowVelocity = 50f;
    public float minThrowVelocity = 1f;
    public float returnTime = 2f;
    public float targetAngle = 45f; // Base throw angle set in the inspector

    [Header("References")]
    public LayerMask targetLayer;
    public FishUI fishUI;
    
    private Camera mainCamera;
    private Rigidbody bobRigidbody;
    private HandController handController;

    private bool isEquipped = false;
    private bool isHoldingBob = false;
    private bool isThrown = false;
    private List<Vector3> previousPositions = new List<Vector3>();
    private float timeTracking = 0f;
    private int ballCounter = 3;
    private Transform handLocation;
    private bool myHand; // LEFT = false, RIGHT = true

    public void Init(bool hand, Transform location, bool equipped)
    {
        handLocation = location;
        isEquipped = equipped;
        DisableComponents();
    }

    void Start()
    {
        mainCamera = Camera.main;
        handController = FindFirstObjectByType<HandController>();
        fishUI = FindFirstObjectByType<FishUI>();

        bobRigidbody = GetComponent<Rigidbody>() ?? gameObject.AddComponent<Rigidbody>();
        bobRigidbody.useGravity = false;
        bobRigidbody.maxAngularVelocity = 100f;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isEquipped)
            TryPickUpBob();

        if (Input.GetMouseButtonUp(0) && isHoldingBob)
        {
            isHoldingBob = false;
            if (CalculateVelocity().magnitude >= minThrowVelocity)
                ThrowBob();
            else
                ResetBob();
        }

        if (isHoldingBob)
        {
            MoveBobToCursor();
            TrackMovement();
        }

        if (!isThrown && isEquipped)
            transform.position = Vector3.Lerp(transform.position, handLocation.position, Time.deltaTime * 10f);
    }

    void TryPickUpBob()
    {
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && hit.transform == transform)
        {
            isHoldingBob = true;
            bobRigidbody.isKinematic = true;
            previousPositions.Clear();
            timeTracking = 0f;
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
        timeTracking += Time.deltaTime;
    }

    Vector3 CalculateVelocity()
    {
        if (previousPositions.Count < 2 || timeTracking == 0f)
            return Vector3.zero;
        return (previousPositions[^1] - previousPositions[0]) / timeTracking;
    }

    void ThrowBob()
    {
        transform.SetParent(null);
        fishUI.UpdateBall(--ballCounter);
        bobRigidbody.isKinematic = false;
        bobRigidbody.useGravity = true;

        Vector3 velocity = CalculateVelocity();
        velocity = Vector3.ClampMagnitude(velocity, maxThrowVelocity);

        float cameraTilt = mainCamera.transform.eulerAngles.x;
        float finalThrowAngle = targetAngle - cameraTilt;
        Quaternion throwRotation = Quaternion.AngleAxis(finalThrowAngle, mainCamera.transform.right);
        Vector3 adjustedThrowDirection = throwRotation * mainCamera.transform.forward;

        bobRigidbody.linearVelocity = adjustedThrowDirection * velocity.magnitude;

        isThrown = true;
        handController.ResetHand(myHand);
        Invoke(nameof(DestroyThrowable), returnTime);
    }

    void ResetBob()
    {
        transform.position = handLocation.position;
        bobRigidbody.isKinematic = true;
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
