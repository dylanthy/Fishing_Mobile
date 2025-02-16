using System.Collections.Generic;
using UnityEngine;


public class OrderManager : MonoBehaviour
{
    public float timeBetweenCustomers = 15f;
    private float remainingTimeBetweenCustomers;
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
    
    public void OpenToggle()
    {
        isOpen = !isOpen;
    }
    
    // Update is called once per frame
    void Update()
    {
        if(isOpen)
        {
            if(remainingTimeBetweenCustomers <= 0f)
            {
                CreateCustomer();
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
        int mySpawn = Random.Range(0,3);
        if(mySpawn == 0)
        {
            myOrderPoint = orderPoint1;
            p1Occupied = true;
        }
        else if (mySpawn == 1)
        {
            myOrderPoint = orderPoint2;
            p2Occupied = true;
        }          
        else if (mySpawn == 2)
        {
            myOrderPoint = orderPoint3;
            p3Occupied = true;
        }       
        GameObject myCustomer = Instantiate(customerPrefab, customerSpawnPoint);
        myCustomer.GetComponent<Order>().Init();
        myCustomer.GetComponent<CustomerMovement>().Init(myOrderPoint, gameObject);
        myOrders.Add(myCustomer.GetComponent<Order>());
    }
}