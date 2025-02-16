using UnityEngine;

public class ItemSpawners : MonoBehaviour
{
    public GameObject myPrefab;
    private HandController handController;
    private Camera mainCamera;
    void Start()
    {
        handController = FindFirstObjectByType<HandController>();
        mainCamera = Camera.main;
    }

    // Update is called once per frame
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
            handController.EquipObject(myPrefab);
        }
    }
}
