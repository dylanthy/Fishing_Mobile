using UnityEngine;
using System.Collections.Generic;

public class FishingBobController : MonoBehaviour
{
    public Camera mainCamera;
    public Transform fishingBob;
    public Transform targetArea;
    public LayerMask targetLayer;
    public float lerpSpeed = 10f;
    public float throwThreshold = 2.0f;
    public float throwForceMultiplier = 5f;
    public float maxFrameTrackingTime = 0.2f;
    public float returnTime = 2f;

    private Vector3 bobStart;
    private bool isHoldable = false;
    private bool isHoldingBob = false;
    private bool isThrown = false;
    private Vector3 targetPosition;
    private List<Vector3> previousPositions = new List<Vector3>();
    private List<Vector3> previousRotations = new List<Vector3>(); // To track rotational movement
    private float timeTracking = 0f;
    private Rigidbody bobRigidbody;

    void Start()
    {
        bobStart = fishingBob.position;
        targetPosition = bobStart;
        bobRigidbody = fishingBob.GetComponent<Rigidbody>();
        if (bobRigidbody == null)
        {
            bobRigidbody = fishingBob.gameObject.AddComponent<Rigidbody>();
        }
        bobRigidbody.useGravity = false;
        bobRigidbody.maxAngularVelocity = 100f; // Increase max angular velocity for proper spinning
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isHoldable)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == fishingBob)
            {
                isHoldingBob = true;
                bobRigidbody.isKinematic = true;
                previousPositions.Clear();
                previousRotations.Clear();
                timeTracking = 0f;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isHoldingBob = false;
            if (CalculateMomentum() > throwThreshold)
            {
                ThrowBob();
                isThrown = true;
                isHoldable = false;
            }
            else
            {
                ReturnBob();
            }
        }

        if (isHoldingBob)
        {
            MoveBobToCursor();
            TrackMovement();
        }

        if (!isThrown)
            fishingBob.position = Vector3.Lerp(fishingBob.position, targetPosition, Time.deltaTime * lerpSpeed);
    }

    void MoveBobToCursor()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, targetLayer))
        {
            if (hit.transform == targetArea)
            {
                targetPosition = hit.point;
            }
        }
    }

    private void ReturnBob()
    {
        fishingBob.position = bobStart;
        targetPosition = bobStart;
        bobRigidbody.isKinematic = true;
        isThrown = false;
        isHoldable = true;
    }

    private void TrackMovement()
    {
        if (previousPositions.Count > 10)
        {
            previousPositions.RemoveAt(0);
            previousRotations.RemoveAt(0);
        }
        previousPositions.Add(fishingBob.position);
        previousRotations.Add(fishingBob.eulerAngles); // Track rotational movement
        timeTracking += Time.deltaTime;
        if (timeTracking > maxFrameTrackingTime)
        {
            previousPositions.RemoveAt(0);
            previousRotations.RemoveAt(0);
        }
    }

    private float CalculateMomentum()
    {
        if (previousPositions.Count < 2 || timeTracking == 0f) return 0f;
        Vector3 totalDisplacement = previousPositions[previousPositions.Count - 1] - previousPositions[0];
        return totalDisplacement.magnitude / timeTracking;
    }

    private Vector3 CalculateAngularMomentum()
    {
        if (previousRotations.Count < 2) return Vector3.zero;
        Vector3 totalRotation = previousRotations[previousRotations.Count - 1] - previousRotations[0];

        // Convert from Euler angles to an approximate angular velocity
        return totalRotation / timeTracking;
    }

    private void ThrowBob()
    {
        bobRigidbody.isKinematic = false;
        bobRigidbody.useGravity = true;

        Vector3 throwDirection = previousPositions[previousPositions.Count - 1] - previousPositions[0];

        // Rotate throw direction by 45 degrees upward on the X-Z plane
        throwDirection = Quaternion.AngleAxis(-45, Vector3.right) * throwDirection;

        bobRigidbody.linearVelocity = throwDirection * throwForceMultiplier;

        // Apply saved angular momentum
        Vector3 angularVelocity = CalculateAngularMomentum();
        bobRigidbody.angularVelocity = angularVelocity;

        Invoke(nameof(ReturnBob), returnTime);
    }
}
