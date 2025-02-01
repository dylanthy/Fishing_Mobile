using UnityEngine;

public class ActiveFishScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FishingBob")) // Checks if it's the fishing bob
        {
            FishingZoneScript fishingZone = GetComponentInParent<FishingZoneScript>();
            if (fishingZone != null)
            {
                fishingZone.OnFishCaught(gameObject);
            }
        }
    }
}
