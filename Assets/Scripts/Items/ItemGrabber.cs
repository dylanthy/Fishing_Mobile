using System;
using UnityEngine;

public class ItemGrabber : MonoBehaviour
{
    private Camera mainCamera;
    private HandController handController;
    void Start()
    {
        mainCamera = Camera.main;
        handController = FindFirstObjectByType<HandController>();   
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryEquipItem();
        }
    }

    void TryEquipItem()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        int layerMask = 1 << gameObject.layer; // Convert layer to bitmask
        if (Physics.Raycast(ray, out RaycastHit hit, 20f, layerMask) && hit.transform == transform)
        {
            if(GetComponent<ItemThrower>())
            {
                GetComponent<ItemThrower>().enabled = true;
            }
            handController.EquipObject(gameObject);
            Destroy(gameObject);
        }
    }
}
