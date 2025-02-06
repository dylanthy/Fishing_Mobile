using UnityEngine;

public class ItemGrabber : MonoBehaviour
{
    private Camera mainCamera;
    private HandController handController;

    public GameObject myFish;

    void Start()
    {
        mainCamera = Camera.main;
        handController = FindFirstObjectByType<HandController>();    
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryGetItem();
        }
    }
    void TryGetItem()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        int layerMask = 1 << gameObject.layer; // Convert layer to bitmask
        if (Physics.Raycast(ray, out RaycastHit hit, 20f, layerMask) && hit.transform == transform)
        {
            handController.EquipObject(myFish);
            Destroy(gameObject);
        }
    }
}
