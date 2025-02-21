using UnityEngine;
using System.Collections;
using UnityEngine.XR;

public class SwimmingFish : MonoBehaviour
{
    private Transform fishingZone;
    private float zoneWidth;
    private float zoneHeight;
    private Vector3 targetPosition;
    private float speed;
    private float turnSpeed;
    private bool isWaiting = false; // Fish should start moving immediately

    public float minSpeed = 1f;
    public float maxSpeed = 3f;
    public float minTurnSpeed = 2f;
    public float maxTurnSpeed = 5f;
    public GameObject myHoldableFish;

    // Distance multipliers
    public float closeDistanceFactor = 0.3f;  // 30% of the total zone size for close distances
    public float farDistanceFactor = 0.9f;    // 90% for rare long movements
    public float closePointChance = 0.8f;     // 80% chance to pick a close point

    void Start()
    {
        fishingZone = transform.parent;
        MeshRenderer renderer = fishingZone.GetComponent<MeshRenderer>();

        if (renderer == null)
        {
            Debug.LogError("Fishing Zone must have a MeshRenderer!");
            return;
        }

        // Get accurate boundary dimensions
        zoneWidth = renderer.bounds.extents.x * 0.9f;
        zoneHeight = renderer.bounds.extents.z * 0.9f;

        // Assign random speed and turn speed to each fish
        speed = Random.Range(minSpeed, maxSpeed);
        turnSpeed = Random.Range(minTurnSpeed, maxTurnSpeed);

        // Initialize fish at a valid position and start swimming immediately
        SetNewTarget(true);
    }

    void Update()
    {
        if (!isWaiting)
        {
            MoveTowardsTarget();
        }
    }

    void SetNewTarget(bool isFirstTime = false)
    {
        float distanceFactor = Random.value < closePointChance ? closeDistanceFactor : farDistanceFactor;

        // Generate a new target position within the specified distance
        targetPosition = new Vector3(
            transform.position.x + Random.Range(-zoneWidth * distanceFactor, zoneWidth * distanceFactor),
            fishingZone.position.y,
            transform.position.z + Random.Range(-zoneHeight * distanceFactor, zoneHeight * distanceFactor)
        );

        // Ensure the target remains inside the fishing zone
        targetPosition.x = Mathf.Clamp(targetPosition.x, fishingZone.position.x - zoneWidth, fishingZone.position.x + zoneWidth);
        targetPosition.z = Mathf.Clamp(targetPosition.z, fishingZone.position.z - zoneHeight, fishingZone.position.z + zoneHeight);

        // If first time, set the fish position directly
        if (isFirstTime)
        {
            transform.position = targetPosition;
            return; // Start swimming without waiting
        }
    }

    void MoveTowardsTarget()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Rotate towards the target, adjusting for the fish's default forward direction (-X)
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            targetRotation *= Quaternion.Euler(0, 90, 0); // Offset so fish faces forward correctly
            
            // Lock the X-axis to 90 degrees
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(90, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }

        // Move forward
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // If fish is close to the target, wait before moving again
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            StartCoroutine(WaitBeforeMoving());
        }
    }

    IEnumerator WaitBeforeMoving()
    {
        isWaiting = true;
        float waitTime = GetRandomWaitTime();
        yield return new WaitForSeconds(waitTime);
        isWaiting = false;
        SetNewTarget();
    }

    float GetRandomWaitTime()
    {
        float randomValue = Random.value;
        if (randomValue < 0.1f) return 0f;   // 10% chance for no wait
        if (randomValue < 0.4f) return 2f;   // 30% chance for 2 seconds
        if (randomValue < 0.7f) return 5f;   // 30% chance for 5 seconds
        return 7f;                           // 30% chance for 7 seconds
    }
}
