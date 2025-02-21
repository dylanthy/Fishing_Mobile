using System.Collections.Generic;
using UnityEngine;


public class OrderManager : MonoBehaviour
{
    public int currency = 25;
    public float timeBetweenCustomers = 15f;
    public float remainingTimeBetweenCustomers = 0f;

    public float totalHappiness = 100f;

    public float remainingHappiness;
    public float happinessLossPerSecond = .5f;
    public bool isOpen = false;
    public GameObject customerPrefab;
    public List<Order> myOrders = new List<Order>();

    [Header("Transforms")]
    public Transform customerSpawnPoint;
    public Transform orderPoint1;
    public bool p1Occupied;
    public Transform orderPoint2;
    public bool p2Occupied;
    public Transform orderPoint3;
    public bool p3Occupied;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        remainingHappiness = totalHappiness;
    }
    public void OpenToggle()
    {
        isOpen = !isOpen;
    }
    
    // Update is called once per frame
    void Update()
    {
        if(remainingHappiness > totalHappiness)
            remainingHappiness = totalHappiness;
        if(isOpen)
        {
            if(remainingTimeBetweenCustomers <= 0f)
            {
                if(!p1Occupied || !p2Occupied || !p3Occupied)
                {
                    CreateCustomer();
                    remainingTimeBetweenCustomers = timeBetweenCustomers;
                }
                else
                {
                    remainingHappiness -= happinessLossPerSecond * Time.deltaTime;
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
        if(!p1Occupied)
        {
            myOrderPoint = orderPoint1;
            p1Occupied = true;    
        }
        else
        {
            int mySpawn = Random.Range(0,2);
            if (mySpawn == 0 && !p2Occupied)
            {
                myOrderPoint = orderPoint2;
                p2Occupied = true;
            }          
            else if(!p3Occupied)
            {
                myOrderPoint = orderPoint3;
                p3Occupied = true;
            }       
        }
        GameObject myCustomer = Instantiate(customerPrefab, customerSpawnPoint);
        myCustomer.GetComponent<Order>().Init();
        myCustomer.GetComponent<CustomerMovement>().Init(myOrderPoint, gameObject);
        myOrders.Add(myCustomer.GetComponent<Order>());
    }


    public void ResetOrderPoint(Transform myOrderPoint)
    {
        if(myOrderPoint == orderPoint1)
        {
            p1Occupied = false;
            remainingHappiness += 10f;
        }
        else if(myOrderPoint == orderPoint2)
        {
            p2Occupied = false;
            remainingHappiness += 10f;

        }
        else if(myOrderPoint == orderPoint3)
        {
            p3Occupied = false;
            remainingHappiness += 10f;

        }
    }
}