using System;
using UnityEngine;

public class FishDisplays : MonoBehaviour
{
    public Transform fishDisplayLocation;
    private bool isFull = false;
    public GameObject myFishOnDisplay;
    void OnTriggerEnter(Collider other)
    {
        if(!isFull)
        {
            if (other.CompareTag("RareFish"))
            {
                isFull = true;
                SetFishToDisplay(other.gameObject);
            }
        }
    }

    public void ResetDisplay()
    {
        myFishOnDisplay.GetComponent<Rigidbody>().isKinematic = false; // Enable physics
        myFishOnDisplay = null;
        isFull = false;
    }

    private void SetFishToDisplay(GameObject gameObject)
    {
        myFishOnDisplay = gameObject;
        gameObject.GetComponent<RareFishManager>().myDisplay = gameObject;
        gameObject.transform.position = fishDisplayLocation.position;
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true; // Disable physics
        }
        gameObject.GetComponent<ItemGrabber>().enabled = true;
        gameObject.GetComponent<ItemThrower>().enabled = false;
    }
}
