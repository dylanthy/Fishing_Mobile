using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR;

public class ItemThrower : MonoBehaviour
{

    public LayerMask targetLayer;
    public float lerpSpeed = 10f;
    public float throwThreshold = .03f;
    public float throwForceMultiplier = 5f;
    public float maxFrameTrackingTime = 0.2f;
    public float returnTime = 2f;
    public FishUI fishUI;
    public Vector2 throwAngle = new Vector2(20f, 50f);

    private Vector3 startingPosition;
    private Quaternion startingRotation;
    private bool isEquipped = false;
    private bool isHoldingBob = false;
    private bool isThrown = false;
    private List<Vector3> previousPositions = new List<Vector3>();
    private float timeTracking = 0f;
    private Rigidbody bobRigidbody;
    private int ballCounter = 3;

    private Camera mainCamera;
    private Transform targetArea;
    private HandController handController;
    public GameObject fish;

    private bool myHand; //LEFT = false, RIGHT = true

    private Transform  handLocation;
    public void Init(bool hand, Transform location, bool equipped) //LEFT = false, RIGHT = true
    {
        myHand = hand;
        handLocation = location;
        isEquipped = equipped;
        if(GetComponent<ItemGrabber>())
            GetComponent<ItemGrabber>().enabled = false;
        if(GetComponent<ItemCooker>())
            GetComponent<ItemCooker>().enabled = false;
    }

    void Start()
    {
        mainCamera = Camera.main;
        targetArea = mainCamera.GetComponentInChildren<BoxCollider>().transform;
        handController = FindFirstObjectByType<HandController>();

        fishUI = FindFirstObjectByType<FishUI>();
        startingPosition = transform.position;
        startingRotation = transform.rotation;
        bobRigidbody = GetComponent<Rigidbody>() ?? gameObject.AddComponent<Rigidbody>();
        bobRigidbody.useGravity = false;
        bobRigidbody.maxAngularVelocity = 100f;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isEquipped)
        {
            TryPickUpBob();
        }

        if (Input.GetMouseButtonUp(0) && isHoldingBob)
        {
            isHoldingBob = false;
            if (CalculateMomentum() > throwThreshold)
            {
                ThrowBob();
            }
            else
            {
                ResetBob();
            }
        }

        if (isHoldingBob)
        {
            MoveBobToCursor();
            TrackMovement();
        }

        if (!isThrown && isEquipped)
        {
            transform.position = Vector3.Lerp(transform.position, handLocation.position, Time.deltaTime * lerpSpeed);
        }
    }

    void TryPickUpBob()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
        {
            isHoldingBob = true;
            bobRigidbody.isKinematic = true;
            previousPositions.Clear();
            timeTracking = 0f;
        }
    }

    void MoveBobToCursor()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, targetLayer) && hit.transform == targetArea)
        {
            transform.position = hit.point;
        }
    }

    void ResetBob()
    {
        transform.position = startingPosition;
        transform.rotation = startingRotation;
        bobRigidbody.isKinematic = true;
        isThrown = false;
    }
    void DestroyThrowable()
    {
        Destroy(gameObject);
    }

    void TrackMovement()
    {
        if (previousPositions.Count > 10) previousPositions.RemoveAt(0);
        previousPositions.Add(transform.position);
        timeTracking += Time.deltaTime;
    }

    float CalculateMomentum()
    {
        return (previousPositions.Count < 2 || timeTracking == 0f) ? 0f : (previousPositions[^1] - previousPositions[0]).magnitude / timeTracking;
    }

    void ThrowBob()
    {
        transform.SetParent(null);
        fishUI.UpdateBall(--ballCounter);
        bobRigidbody.isKinematic = false;
        bobRigidbody.useGravity = true;

        if (previousPositions.Count < 2) return;

        Vector3 totalDisplacement = previousPositions[^1] - previousPositions[0];
        float velocityMagnitude = totalDisplacement.magnitude / timeTracking;
        Vector3 throwDirection = totalDisplacement.normalized;

        Vector3 rotationAxis = mainCamera.transform.right;
        float angle = GetThrowAngle(velocityMagnitude);
        throwDirection = Quaternion.AngleAxis(angle, rotationAxis) * throwDirection;

        bobRigidbody.linearVelocity = throwDirection * velocityMagnitude * throwForceMultiplier;
        isThrown = true;
        Debug.Log($"Resetting Hand {myHand}");
        handController.ResetHand(myHand);
        Invoke(nameof(DestroyThrowable), returnTime);
    }

    float GetThrowAngle(float velocityMagnitude)
    {
        return Mathf.Lerp(throwAngle.x, throwAngle.y, velocityMagnitude / throwForceMultiplier);
    }
}
