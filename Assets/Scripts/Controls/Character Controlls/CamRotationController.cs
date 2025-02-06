using System.Collections;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float rotationDuration = 0.5f; // Time in seconds for a full rotation
    public float rotationXForward = 10f;
    public float rotationXRight = 15f;
    public float rotationXBackward = 5f;
    public float rotationXLeft = 20f;

    private Quaternion targetRotation;
    private Coroutine rotationCoroutine;
    private bool isRotating = false;
    private float queuedRotation = 0;

    void Update()
    {
        if (!isRotating)
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKey(KeyCode.D))
            {
                RotateCamera(90);
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKey(KeyCode.A))
            {
                RotateCamera(-90);
            }
        }
        else
        {
            // Queue the next turn if the user is still holding a key
            if (Input.GetKey(KeyCode.D))
                queuedRotation = 90;
            else if (Input.GetKey(KeyCode.A))
                queuedRotation = -90;
            else
                queuedRotation = 0; // Reset if no key is held
        }
    }

    void RotateCamera(float angle)
    {
        if (isRotating) return;

        isRotating = true;
        float newYRotation = transform.eulerAngles.y + angle;
        targetRotation = Quaternion.Euler(GetXRotation(newYRotation), newYRotation, 0);

        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
        }

        rotationCoroutine = StartCoroutine(RotateOverTime());
    }

    IEnumerator RotateOverTime()
    {
        float elapsedTime = 0;
        Quaternion startRotation = transform.rotation;

        while (elapsedTime < rotationDuration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
        isRotating = false;

        // If there's a queued rotation, start it immediately
        if (queuedRotation != 0)
        {
            RotateCamera(queuedRotation);
        }
    }

    float GetXRotation(float yRotation)
    {
        yRotation = (yRotation + 360) % 360;

        if (Mathf.Approximately(yRotation, 0)) return rotationXForward;  // Facing forward
        if (Mathf.Approximately(yRotation, 90)) return rotationXRight;   // Facing right
        if (Mathf.Approximately(yRotation, 180)) return rotationXBackward; // Facing backward
        if (Mathf.Approximately(yRotation, 270)) return rotationXLeft;   // Facing left

        return transform.eulerAngles.x;
    }
}
