using UnityEngine;

public class ActiveFishScript : MonoBehaviour
{
    public GameObject myHoldableFish;
    public bool isNeededForOrder = false; // If true, this fish is needed for an order
    public int myNumber;

    public void Start()
    {
        Invoke("DestroyFish", 10f); // Destroy the fish after 10 seconds if not caught
        Destroy(gameObject, 10f);
        myNumber = myHoldableFish.GetComponent<ItemCooker>().fishIdentifier;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FishingBob")) // Checks if it's the fishing bob
        {
            DayPondManager.I.OnFishCaught(gameObject, myHoldableFish);
            Destroy(other.gameObject);
        }
    }
    public void DestroyFish()
    {
        if (!isNeededForOrder)
            Destroy(gameObject);
        else
            Invoke("DestroyFish", 10f); // Re-invoke if still needed for an order
    }
    void OnDestroy()
    {
        if(DayPondManager.I.activeFish.Contains(gameObject))
            DayPondManager.I.activeFish.Remove(gameObject);
    }
}
