using UnityEngine;

public class ActiveFishScript : MonoBehaviour
{
    public GameObject myHoldableFish;

    public void Init()
    {
        Destroy(gameObject, 10f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FishingBob")) // Checks if it's the fishing bob
        {
            FishingZoneScript fishingZone = GetComponentInParent<FishingZoneScript>();
            if (fishingZone != null)
            {
                fishingZone.OnFishCaught(gameObject, myHoldableFish);
                Destroy(other.gameObject);
            }
        }
    }
}
