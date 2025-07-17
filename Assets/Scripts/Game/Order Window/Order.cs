using System.Collections.Generic;
using UnityEngine;

public class Order : MonoBehaviour
{
    public bool orderAnyFish;
    public int orderFishNumber;
    public float orderTime = 35f;
    public float positiveReceiveTime = 15f;
    public float remainingOrderTime;
    public bool isWithinPositiveReceiveTime = true;
    public bool ordered = false;
    public int myTicketNumber;
    public int baseOrderValue = 2;
    public int tipValue = 1;
    public int totalOrderValue;
    public int mySpotInNeededFishList = -1; // -1 means not in the list, otherwise it's the index in the neededFish list
    public List<int> currentFishPool = new List<int>();

    public void Init(int ticketNum) // 75 % chance for a random fish order , 25% chance for a "specific" fish order
    {
        orderAnyFish = Random.value < .01f;
        if (!orderAnyFish)
        {
            foreach (GameObject fish in DayPondManager.I.activeFish)
            {
                if (fish.GetComponent<ActiveFishScript>().myHoldableFish != null && !currentFishPool.Contains(fish.GetComponent<ActiveFishScript>().myHoldableFish.GetComponent<ItemCooker>().fishIdentifier))
                {
                    currentFishPool.Add(fish.GetComponent<ActiveFishScript>().myHoldableFish.GetComponent<ItemCooker>().fishIdentifier);
                }
            }

            orderFishNumber = currentFishPool[Random.Range(0, currentFishPool.Count)];
            DayPondManager.I.neededFish.Add(DayPondManager.I.dayFishPrefabs[orderFishNumber]);
            mySpotInNeededFishList = DayPondManager.I.neededFish.Count - 1;

            currentFishPool.Clear();
        }
        else // random quantity of fish, 93% chance they want 1 fish, 7% chance they want 2
        {
            orderFishNumber = (Random.value < .93f) ? 1 : 2;
        }
        remainingOrderTime = orderTime;
        myTicketNumber = ticketNum;
        totalOrderValue = baseOrderValue;
    }

    void Update()
    {
        if (ordered)
        {
            OrdersUI.Instance.UpdateOrderTimer(GetComponent<CustomerMovement>().myOrderSpot, remainingOrderTime / orderTime);
            remainingOrderTime -= Time.deltaTime * OrderManager.Instance.timeMultiplier;
            if(remainingOrderTime >= positiveReceiveTime)
            {
                isWithinPositiveReceiveTime = true;
            }
            else
                isWithinPositiveReceiveTime = false;
            if (remainingOrderTime <= 0)
            {
                FindFirstObjectByType<OrderManager>().ResetOrderPoint(GetComponent<CustomerMovement>().orderPoint);
                OrderFailed();
                Destroy(gameObject);
                if(!orderAnyFish)
                    DayPondManager.I.neededFish.RemoveAt(mySpotInNeededFishList);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Cookable" && other.GetComponent<ItemCooker>().isCooked && !other.GetComponent<ItemCooker>().isBurned)
        {
            if (orderAnyFish)
            {
                if (orderFishNumber >= 0)
                {
                    orderFishNumber--;
                    if (orderFishNumber <= 0)
                    {
                        OrderCompleted(other.gameObject);
                    }
                    else
                    {
                        GetComponent<CustomerMovement>().SayOrder();
                    }
                }
                else
                {
                    OrderCompleted(other.gameObject);
                }
            }
            else
            {
                if (orderFishNumber == other.GetComponent<ItemCooker>().fishIdentifier)
                {
                    OrderCompleted(other.gameObject);
                }
            }
        }
    }

    void OrderCompleted(GameObject dishThatCollided)
    {
        FindFirstObjectByType<OrderManager>().ResetOrderPoint(GetComponent<CustomerMovement>().orderPoint);
        Destroy(dishThatCollided);
        if (isWithinPositiveReceiveTime)
        {
            totalOrderValue += tipValue;
            OrderManager.Instance.AddToScore(GetComponent<CustomerMovement>().myOrderSpot);
        }
        OrderManager.Instance.currentBalance += totalOrderValue;
        OrdersUI.Instance.ResetOrderPanel(GetComponent<CustomerMovement>().myOrderSpot);
        Destroy(gameObject);
        if(!orderAnyFish)
            DayPondManager.I.neededFish.RemoveAt(mySpotInNeededFishList);
    }

    private void OrderFailed()
    {
        FindFirstObjectByType<OrderManager>().ResetOrderPoint(GetComponent<CustomerMovement>().orderPoint);
        OrdersUI.Instance.ResetOrderPanel(GetComponent<CustomerMovement>().myOrderSpot);
        OrderManager.Instance.SubtractFromScore(GetComponent<CustomerMovement>().myOrderSpot);
        Destroy(gameObject);
        if(!orderAnyFish)
            DayPondManager.I.neededFish.RemoveAt(mySpotInNeededFishList);
    }
}
