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
    public FishUI fishUI;
    public Vector2 throwAngleForward = new Vector2(-30f, -10f);
    public Vector2 throwAngleRight = new Vector2(-20f, 0f);
    public Vector2 throwAngleBackward = new Vector2(-40f, -20f);
    public Vector2 throwAngleLeft = new Vector2(-25f, -5f);

    private Vector3 startingPosition;
    private Quaternion startingRotation;
    private bool isHoldingBob = false;
    private bool isThrown = false;
    private List<Vector3> previousPositions = new List<Vector3>();
    private float timeTracking = 0f;
    private Rigidbody bobRigidbody;
    private int ballCounter = 3;

    void Start()
    {
        startingPosition = transform.position;
        startingRotation = transform.rotation;
        bobRigidbody = GetComponent<Rigidbody>() ?? gameObject.AddComponent<Rigidbody>();
        bobRigidbody.useGravity = false;
        bobRigidbody.maxAngularVelocity = 100f;
    }

    void Update()
    {
        if (Input.GetButtonDown("Reload"))
        {
            fishUI.UpdateBall(++ballCounter);
        }

        if (Input.GetMouseButtonDown(0))
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

        if (!isThrown)
        {
            transform.position = Vector3.Lerp(transform.position, startingPosition, Time.deltaTime * lerpSpeed);
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
        fishUI.UpdateBall(--ballCounter);
        bobRigidbody.isKinematic = false;
        bobRigidbody.useGravity = true;

        if (previousPositions.Count < 2) return;

        Vector3 totalDisplacement = previousPositions[^1] - previousPositions[0];
        float velocityMagnitude = totalDisplacement.magnitude / timeTracking;
        Vector3 throwDirection = totalDisplacement.normalized;

        float playerYRotation = (mainCamera.transform.eulerAngles.y + 360) % 360;
        Vector3 rotationAxis = mainCamera.transform.right;
        float angle = GetThrowAngle(playerYRotation, velocityMagnitude);
        throwDirection = Quaternion.AngleAxis(angle, rotationAxis) * throwDirection;

        bobRigidbody.linearVelocity = throwDirection * velocityMagnitude * throwForceMultiplier;
        isThrown = true;
        Invoke(nameof(ResetBob), returnTime);
    }

    float GetThrowAngle(float yRotation, float velocityMagnitude)
    {
        if (yRotation < 45 || yRotation > 315) return Mathf.Lerp(throwAngleForward.x, throwAngleForward.y, velocityMagnitude / throwForceMultiplier);
        if (yRotation >= 45 && yRotation < 135) return Mathf.Lerp(throwAngleRight.x, throwAngleRight.y, velocityMagnitude / throwForceMultiplier);
        if (yRotation >= 135 && yRotation < 225) return Mathf.Lerp(throwAngleBackward.x, throwAngleBackward.y, velocityMagnitude / throwForceMultiplier);
        return Mathf.Lerp(throwAngleLeft.x, throwAngleLeft.y, velocityMagnitude / throwForceMultiplier);
    }
}
