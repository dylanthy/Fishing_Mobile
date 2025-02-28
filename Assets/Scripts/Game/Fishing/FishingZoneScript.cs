using System.Collections.Generic;
using UnityEngine;

public class FishingZoneScript : MonoBehaviour
{
    public List<GameObject> fishPrefabs;
    public Transform fishingZone;
    
    private float zoneWidth;
    private float zoneHeight;

    public float spawnInterval = 3f;
    public int maxFish = 7;

    void Start()
    {
        MeshRenderer renderer = fishingZone.GetComponent<MeshRenderer>();
        zoneWidth = renderer.bounds.size.x / 2;
        zoneHeight = renderer.bounds.size.z / 2;
        InvokeRepeating(nameof(SpawnFish), 0f, spawnInterval);
    }

    void SpawnFish()
    {
        if (fishingZone.childCount >= maxFish) return;

        Vector3 spawnPosition = new Vector3(
            Random.Range(fishingZone.position.x - zoneWidth, fishingZone.position.x + zoneWidth),
            fishingZone.position.y + 0.3f,
            Random.Range(fishingZone.position.z - zoneHeight, fishingZone.position.z + zoneHeight)
        );
        GameObject fishPrefab = fishPrefabs[Random.Range(0, fishPrefabs.Count)];
        GameObject newFish = Instantiate(fishPrefab, spawnPosition, Quaternion.identity, fishingZone);

        // Ensure the fish faces -X at spawn
        newFish.transform.rotation = Quaternion.Euler(0, 90, 0);
    }

    public void OnFishCaught(GameObject fish, GameObject spawnedFish)
    {
        Destroy(fish);
        FindFirstObjectByType<HandController>().EquipObject(spawnedFish);
    }
}
