using UnityEngine;

public class NightFishScript : MonoBehaviour
{
    public GameObject myHoldableFish;

    public void Start()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FishingBob")) // Checks if it's the fishing bob
        {
            DayPondManager.I.OnFishCaught(gameObject, myHoldableFish);
            Destroy(other.gameObject);
        }
    }
    void OnDestroy()
    {
        if(DayPondManager.I.activeFish.Contains(gameObject))
            DayPondManager.I.activeFish.Remove(gameObject);
    }
}
