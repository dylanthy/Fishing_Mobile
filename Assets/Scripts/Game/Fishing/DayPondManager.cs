using System.Collections.Generic;
using UnityEngine;

public class DayPondManager : MonoBehaviour
{
    private static DayPondManager instance;
    public static DayPondManager I
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }
    public List<GameObject> dayFishPrefabs;
    public List<GameObject> nightFishPrefabs;
    public List<GameObject> activeFish = new List<GameObject>();
    public Transform fishingZone;
    private float zoneWidth;
    private float zoneHeight;
    public float daySpawnInterval = 3f;
    public float nightSpawnInterval = 3f;
    public int maxFishDay = 7;
    public int maxFishNight = 3;

    public bool isDayMode = false;
    private float currentSpawnInterval;

    void Start()
    {
        MeshRenderer renderer = fishingZone.GetComponent<MeshRenderer>();
        zoneWidth = renderer.bounds.size.x / 2;
        zoneHeight = renderer.bounds.size.z / 2;
        currentSpawnInterval = daySpawnInterval;
    }

    void Update()
    {
        if (isDayMode != OrderManager.Instance.isOpen)
        {
            isDayMode = OrderManager.Instance.isOpen;
            ClearPond();
            currentSpawnInterval = isDayMode ? daySpawnInterval : nightSpawnInterval;
        }
        if(currentSpawnInterval >= 0)
        {
            currentSpawnInterval -= Time.deltaTime * OrderManager.Instance.timeMultiplier;
        }
        else
        {
            if (isDayMode) SpawnFishDay(); else SpawnFishNight();
            currentSpawnInterval = isDayMode ? daySpawnInterval : nightSpawnInterval;
        }
    }
    void ClearPond()
    {
        foreach (GameObject fish in activeFish.ToArray())
        {
            if (fish != null)
            {
                Destroy(fish);
            }
        }
        activeFish.Clear();
    }

    void SpawnFishDay()
    {
        if (fishingZone.childCount >= maxFishDay) return;

        Vector3 spawnPosition = new Vector3(
            Random.Range(fishingZone.position.x - zoneWidth, fishingZone.position.x + zoneWidth),
            fishingZone.position.y + 0.3f,
            Random.Range(fishingZone.position.z - zoneHeight, fishingZone.position.z + zoneHeight)
        );
        GameObject fishPrefab = dayFishPrefabs[Random.Range(0, dayFishPrefabs.Count)];
        GameObject newFish = Instantiate(fishPrefab, spawnPosition, Quaternion.identity, fishingZone);
        activeFish.Add(newFish);
        // Ensure the fish faces -X at spawn
        newFish.transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    private void SpawnFishNight()
    {
        if (fishingZone.childCount >= maxFishNight) return;

        Vector3 spawnPosition = new Vector3(
            Random.Range(fishingZone.position.x - zoneWidth, fishingZone.position.x + zoneWidth),
            fishingZone.position.y + 0.3f,
            Random.Range(fishingZone.position.z - zoneHeight, fishingZone.position.z + zoneHeight)
        );
        GameObject fishPrefab = nightFishPrefabs[Random.Range(0, nightFishPrefabs.Count)];
        GameObject newFish = Instantiate(fishPrefab, spawnPosition, Quaternion.identity, fishingZone);
        activeFish.Add(newFish);
        // Ensure the fish faces -X at spawn
        newFish.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void OnFishCaught(GameObject fish, GameObject spawnedFish)
    {
        Destroy(fish);
        if(activeFish.Contains(fish))
            activeFish.Remove(fish);
        FindFirstObjectByType<HandController>().EquipObject(spawnedFish);
    }
}
