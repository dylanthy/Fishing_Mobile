using UnityEngine;
using System.Collections.Generic;

public class FishingBobController : MonoBehaviour
{
    public Camera mainCamera;
    public Transform targetArea;
    public LayerMask targetLayer;
    public float lerpSpeed = 10f;
    public float throwThreshold = 2.0f;
    public float throwForceMultiplier = 5f;
    public float maxFrameTrackingTime = 0.2f;
    public float returnTime = 2f;

    private Vector3 bobStart;
    private bool isHoldable = true;
    private bool isHoldingBob = false;
    private bool isThrown = false;
    private Vector3 targetPosition;
    private List<Vector3> previousPositions = new List<Vector3>();
    private List<Vector3> previousRotations = new List<Vector3>(); // To track rotational movement
    private float timeTracking = 0f;
    private Rigidbody bobRigidbody;

    public FishUI fishUI;
    private int ballCounter = 3;

    public Vector2 throwAngleForward = new Vector2(-30f, -10f);
    public Vector2 throwAngleRight = new Vector2(-20f, 0f);
    public Vector2 throwAngleBackward = new Vector2(-40f, -20f);
    public Vector2 throwAngleLeft = new Vector2(-25f, -5f);

    void Start()
    {
        bobStart = transform.position;
        targetPosition = bobStart;
        bobRigidbody = GetComponent<Rigidbody>();

        if (bobRigidbody == null)
        {
            bobRigidbody = gameObject.AddComponent<Rigidbody>();
        }
        bobRigidbody.useGravity = false;
        bobRigidbody.maxAngularVelocity = 100f;
    }

    void Update()
    {
        if (Input.GetButtonDown("Reload"))
        {
            ballCounter++;
            fishUI.UpdateBall(ballCounter);
        }

        if (Input.GetMouseButtonDown(0) && isHoldable)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
            {
                isHoldingBob = true;
                bobRigidbody.isKinematic = true;
                previousPositions.Clear();
                previousRotations.Clear();
                timeTracking = 0f;
            }
        }

        if (Input.GetMouseButtonUp(0) && isHoldingBob)
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
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * lerpSpeed);
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
        transform.position = bobStart;
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
        previousPositions.Add(transform.position);
        previousRotations.Add(transform.eulerAngles);
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

    private void ThrowBob()
    {
        ballCounter--;
        fishUI.UpdateBall(ballCounter);
        bobRigidbody.isKinematic = false;
        bobRigidbody.useGravity = true;

        if (previousPositions.Count < 2) return;

        Vector3 totalDisplacement = previousPositions[previousPositions.Count - 1] - previousPositions[0];
        float velocityMagnitude = totalDisplacement.magnitude / timeTracking;
        Vector3 throwDirection = totalDisplacement.normalized;

        float playerYRotation = (mainCamera.transform.eulerAngles.y + 360) % 360;
        Vector3 rotationAxis = mainCamera.transform.right;

        float angle = 0f;
        if (playerYRotation < 45 || playerYRotation > 315)
        {
            angle = Mathf.Lerp(throwAngleForward.x, throwAngleForward.y, velocityMagnitude / throwForceMultiplier);
        }
        else if (playerYRotation >= 45 && playerYRotation < 135)
        {
            angle = Mathf.Lerp(throwAngleRight.x, throwAngleRight.y, velocityMagnitude / throwForceMultiplier);
        }
        else if (playerYRotation >= 135 && playerYRotation < 225)
        {
            angle = Mathf.Lerp(throwAngleBackward.x, throwAngleBackward.y, velocityMagnitude / throwForceMultiplier);
        }
        else if (playerYRotation >= 225 && playerYRotation < 315)
        {
            angle = Mathf.Lerp(throwAngleLeft.x, throwAngleLeft.y, velocityMagnitude / throwForceMultiplier);
        }

        throwDirection = Quaternion.AngleAxis(angle, rotationAxis) * throwDirection;
        bobRigidbody.linearVelocity = throwDirection * velocityMagnitude * throwForceMultiplier;


        Debug.Log($"Bob Thrown at angle: {angle}");
        Invoke(nameof(ReturnBob), returnTime);
    }
}