using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    private static OrderManager instance;
    public static OrderManager Instance
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

    public int currency = 25;
    public float timeBetweenCustomers = 15f;
    public float remainingTimeBetweenCustomers = 0f;

    public float totalHappiness = 100f;

    public float remainingHappiness;
    public float happinessLossPerSecond = .5f;
    public bool isOpen = false;
    public GameObject customerPrefab;

    [Header("Transforms")]
    public Transform customerSpawnPoint;
    public Transform orderPoint1;
    public bool p1Occupied;
    public Transform orderPoint2;
    public bool p2Occupied;
    public Transform orderPoint3;
    public bool p3Occupied;
    public float openTime = 10f; // 10 seconds
    public float closeTime = 240f; // 4 minutes in seconds
    public float dayDuration = 300f; // 5 minutes in seconds
    public float currentTime;

    void Start()
    {
        remainingHappiness = totalHappiness;
        currentTime = 0f;
        if (closeTime > dayDuration)
        {
            closeTime = dayDuration;
            Debug.LogWarning("Close time must be less than or equal to day duration");
        }
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= dayDuration)
        {
            currentTime = 0f;
            OrdersUI.Instance.day++;
        }
        if (currentTime >= closeTime)
        {
            isOpen = false;
        }
        else if (currentTime >= openTime)
        {
            isOpen = true;
        }

        if (isOpen)
        {
            if (remainingTimeBetweenCustomers <= 0f)
            {
                if (!p1Occupied || !p2Occupied || !p3Occupied)
                {
                    CreateCustomer();
                    remainingTimeBetweenCustomers = timeBetweenCustomers;
                }
                else
                {
                    // No room for customers
                }
            }
            else
            {
                remainingTimeBetweenCustomers -= Time.deltaTime;
            }
        }
    }

    private void CreateCustomer()
    {
        Transform myOrderPoint = null;
        if (!p1Occupied)
        {
            myOrderPoint = orderPoint1;
            p1Occupied = true;
        }
        else
        {
            int mySpawn = Random.Range(0, 2);
            if (mySpawn == 0 && !p2Occupied)
            {
                myOrderPoint = orderPoint2;
                p2Occupied = true;
            }
            else if (!p3Occupied)
            {
                myOrderPoint = orderPoint3;
                p3Occupied = true;
            }
        }
        GameObject myCustomer = Instantiate(customerPrefab, customerSpawnPoint);
        myCustomer.GetComponent<Order>().Init();
        myCustomer.GetComponent<CustomerMovement>().Init(myOrderPoint, gameObject);
    }

    public void ResetOrderPoint(Transform myOrderPoint)
    {
        if (myOrderPoint == orderPoint1)
        {
            p1Occupied = false;
            remainingHappiness += 10f;
        }
        else if (myOrderPoint == orderPoint2)
        {
            p2Occupied = false;
            remainingHappiness += 10f;
        }
        else if (myOrderPoint == orderPoint3)
        {
            p3Occupied = false;
            remainingHappiness += 10f;
        }
    }
}