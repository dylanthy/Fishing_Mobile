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
    public float timeBetweenCustomers = 15f;
    public float timeBetweenCustomersMin = 5f;
    public float remainingTimeBetweenCustomers = 0f;
    public bool isOpen = false;
    public GameObject customerPrefab;

    [Header("Transforms")]
    public Transform customerSpawnPoint;
    public Transform orderPoint1;
    public bool p1Occupied = false;
    public Transform orderPoint2;
    public bool p2Occupied = false;
    public Transform orderPoint3;
    public bool p3Occupied = false;
    public float openTime = 10f; // 10 seconds
    public float closeTime = 240f; // 4 minutes in seconds
    public float dayDuration = 300f; // 5 minutes in seconds
    public float currentTime;
    private int currentOrder = 1;
    public float storeScore = 5f;
    public float storeScoreMax = 5f;
    public float storeScoreMin = 0f;
    public float storeScoreDecay = -0.1f;
    public float storeScoreIncrease = 0.1f;
    public float timeMultiplier = 1f;
    public int currentBalance = 0;

    void Start()
    {
        storeScore = storeScoreMax;
        currentTime = 0f;
        if (closeTime > dayDuration)
        {
            closeTime = dayDuration;
            Debug.LogWarning("Close time must be less than or equal to day duration");
        }
    }

    void Update()
    {
        currentTime += Time.deltaTime * timeMultiplier;
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
                    remainingTimeBetweenCustomers = timeBetweenCustomersMin;
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
        int myOrderPointInt = 0;
        if (!p1Occupied)
        {
            myOrderPoint = orderPoint1;
            myOrderPointInt = 2;
            p1Occupied = true;
        }
        else
        {
            int mySpawn = Random.Range(0, 2);
            if (mySpawn == 0 && !p2Occupied)
            {
                myOrderPoint = orderPoint2;
                myOrderPointInt = 1;
                p2Occupied = true;
            }
            else if (!p3Occupied)
            {
                myOrderPoint = orderPoint3;
                myOrderPointInt = 3;
                p3Occupied = true;
            }
        }
        if(myOrderPoint == null)
        {
            return;
        }
        GameObject myCustomer = Instantiate(customerPrefab, customerSpawnPoint);
        myCustomer.GetComponent<Order>().Init(currentOrder);
        myCustomer.GetComponent<CustomerMovement>().Init(myOrderPoint, gameObject, myOrderPointInt);
        currentOrder++;
    }

    public void ResetOrderPoint(Transform myOrderPoint)
    {
        if (myOrderPoint == orderPoint1)
        {
            p1Occupied = false;
        }
        else if (myOrderPoint == orderPoint2)
        {
            p2Occupied = false;
        }
        else if (myOrderPoint == orderPoint3)
        {
            p3Occupied = false;
        }
    }

    public void AddToScore(int orderSpot)
    {
        storeScore += storeScoreIncrease;
        if (storeScore > storeScoreMax)
        {
            storeScore = storeScoreMax;
            return;
        }
        OrdersUI.Instance.SpawnScoreChangeText(storeScoreIncrease, orderSpot);
    }
    public void SubtractFromScore(int orderSpot)
    {
        storeScore += storeScoreDecay;
        if (storeScore < storeScoreMin)
        {
            storeScore = storeScoreMin;
            return;
        }
        OrdersUI.Instance.SpawnScoreChangeText(storeScoreDecay, orderSpot);
    }
}