using System.Collections;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float rotationSpeed = 2f; // Speed of rotation
    public float cooldownTime = 2f; // Cooldown after rotation
    public float rotationXForward = 10f; // X rotation when facing forward
    public float rotationXRight = 15f; // X rotation when facing right
    public float rotationXBackward = 5f; // X rotation when facing backward
    public float rotationXLeft = 20f; // X rotation when facing left

    private bool isRotating = false;
    private Quaternion targetRotation;
    private float targetXRotation;

    void Update()
    {
        if (!isRotating)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                RotateCamera(90);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                RotateCamera(-90);
            }
        }
    }

    void RotateCamera(float angle)
    {
        if (isRotating) return;

        isRotating = true;
        float newYRotation = transform.eulerAngles.y + angle;
        targetRotation = Quaternion.Euler(GetXRotation(newYRotation), newYRotation, 0);
        StartCoroutine(RotateOverTime());
    }

    IEnumerator RotateOverTime()
    {
        float elapsedTime = 0;
        Quaternion startRotation = transform.rotation;

        while (elapsedTime < 1f)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime * rotationSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
        yield return new WaitForSeconds(cooldownTime); // Start cooldown only after rotation finishes

        isRotating = false;
    }

    float GetXRotation(float yRotation)
    {
        // Normalize rotation between 0-360 degrees
        yRotation = (yRotation + 360) % 360;

        if (Mathf.Approximately(yRotation, 0)) return rotationXForward;  // Facing forward
        if (Mathf.Approximately(yRotation, 90)) return rotationXRight;   // Facing right
        if (Mathf.Approximately(yRotation, 180)) return rotationXBackward; // Facing backward
        if (Mathf.Approximately(yRotation, 270)) return rotationXLeft;   // Facing left

        return transform.eulerAngles.x; // Default (shouldn't happen)
    }
}
