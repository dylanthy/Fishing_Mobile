using UnityEngine;

public class PanCook : MonoBehaviour
{
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = Camera.main; // Optimized way to get the main camera
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 1f);

            int layerMask = 1 << gameObject.layer; // Convert layer to bitmask
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                if (hit.transform == transform)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
