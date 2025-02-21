using System;
using UnityEngine;

public class ItemGrabber : MonoBehaviour
{
    private Camera mainCamera;
    private HandController handController;
    [SerializeField] private LayerMask layerMask;
    void Start()
    {
        mainCamera = Camera.main;
        handController = FindFirstObjectByType<HandController>();   
        layerMask = gameObject.layer;
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
        int bitMask = 1 << layerMask; // Convert layer to bitmask
        if (Physics.Raycast(ray, out RaycastHit hit, 20f, bitMask) && hit.transform == transform)
        {
            EquipItem();
        }
    }

    public void EquipItem()
    {
        if(GetComponent<ItemThrower>())
        {
            GetComponent<ItemThrower>().enabled = true;
        }
        if(GetComponent<ItemCooker>() && GetComponent<ItemCooker>().tempVarHasBeenStarted == true)
        {
            GetComponent<ItemCooker>().EndCooking();
        }

        handController.EquipObject(gameObject);
        Destroy(gameObject);
    }
}
