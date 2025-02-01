using UnityEngine;

public class FishingZoneScript : MonoBehaviour
{
    public GameObject activeFishPrefab;
    public Transform fishingZone;
    public float spawnInterval = 3f;
    public int maxFish = 5;

    private float zoneWidth;
    private float zoneHeight;
    public FishUI fishUI;

    void Start()
    {
        if (fishingZone == null)
        {
            Debug.LogError("FishingZone not assigned!");
            return;
        }

        MeshRenderer renderer = fishingZone.GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            Debug.LogError("FishingZone must have a MeshRenderer!");
            return;
        }

        zoneWidth = renderer.bounds.size.x / 2;
        zoneHeight = renderer.bounds.size.z / 2;

        InvokeRepeating(nameof(SpawnActiveFish), 0f, spawnInterval);
    }

    void SpawnActiveFish()
    {
        if (fishingZone.childCount >= maxFish) return; // Limits fish count

        Vector3 spawnPosition = new Vector3(
            Random.Range(fishingZone.position.x - zoneWidth, fishingZone.position.x + zoneWidth),
            fishingZone.position.y + 0.1f,
            Random.Range(fishingZone.position.z - zoneHeight, fishingZone.position.z + zoneHeight)
        );

        GameObject newFish = Instantiate(activeFishPrefab, spawnPosition, Quaternion.identity);
        newFish.transform.parent = fishingZone; // Set as child of FishingZone
    }

    public void OnFishCaught(GameObject fish)
    {
        Destroy(fish);
        fishUI.UpdateFish();
    }
}
