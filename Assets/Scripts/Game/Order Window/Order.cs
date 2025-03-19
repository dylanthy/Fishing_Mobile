using System.Collections.Generic;
using UnityEngine;

public class Order : MonoBehaviour
{
    public bool orderAnyFish;
    public int orderFishNumber;
    public float orderTime = 35f;
    public float remainingOrderTime;
    public bool ordered = false;
    public int myTicketNumber;

    public void Init(int ticketNum) // 75 % chance for a random fish order , 25% chance for a "specific" fish order
    {
        orderAnyFish = Random.value < .75f;
        if (!orderAnyFish)
        {
            orderFishNumber = Random.Range(0, 4);
        }
        else // random quantity of fish, 93% chance they want 1 fish, 7% chance they want 2
        {
            orderFishNumber = (Random.value < .93f) ? 1 : 2;
        }
        remainingOrderTime = orderTime;
        myTicketNumber = ticketNum;
    }

    void Update()
    {
        if (ordered)
        {
            OrdersUI.Instance.UpdateOrderTimer(GetComponent<CustomerMovement>().myOrderSpot, remainingOrderTime / orderTime);
            remainingOrderTime -= Time.deltaTime;
            if (remainingOrderTime <= 0)
            {
                FindFirstObjectByType<OrderManager>().ResetOrderPoint(GetComponent<CustomerMovement>().orderPoint);
                OrderFailed();
                Destroy(gameObject);
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
        OrdersUI.Instance.ResetOrderPanel(GetComponent<CustomerMovement>().myOrderSpot);
        Destroy(gameObject);
    }

    private void OrderFailed()
    {
        FindFirstObjectByType<OrderManager>().ResetOrderPoint(GetComponent<CustomerMovement>().orderPoint);
        OrdersUI.Instance.ResetOrderPanel(GetComponent<CustomerMovement>().myOrderSpot);
        Destroy(gameObject);
    }
}
