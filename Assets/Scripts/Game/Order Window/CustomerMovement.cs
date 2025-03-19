using TMPro;
using UnityEngine;

public class CustomerMovement : MonoBehaviour
{
    public float walkSpeed;
    public Transform orderPoint;
    public GameObject[] fishSpeech;
    public GameObject speechBubble;
    public TextMeshPro text;
    public GameObject dishParent;
    private Order myOrder;
    private bool hasOrderedOnce = false;
    public int myOrderSpot;

    public void Init(Transform orderPoint, GameObject orderManager, int orderSpot)
    {
        this.orderPoint = orderPoint;
        myOrder = GetComponent<Order>();
        myOrderSpot = orderSpot;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, orderPoint.position, Time.deltaTime * walkSpeed);
        if (Vector3.Distance(transform.position, orderPoint.position) < 0.1f)
        if(!hasOrderedOnce)
        {
            SayOrder();
            GetComponent<Order>().ordered = true;
        }
    }

    public void SayOrder()
    {
        Order order = GetComponent<Order>();
        if (order.orderAnyFish)
        {
            text.text = $"Any {order.orderFishNumber} fish";
            speechBubble.SetActive(true);
            text.gameObject.SetActive(true);
            hasOrderedOnce = true;
        }
        else
        {
            GameObject dish = fishSpeech[order.orderFishNumber];
            speechBubble.SetActive(true);
            text.gameObject.SetActive(false);
            dishParent.SetActive(true);
            if (!hasOrderedOnce)
            {
                GameObject display = Instantiate(dish, gameObject.transform);
                display.transform.SetParent(dishParent.transform, false);
                hasOrderedOnce = true;
            }
        }
        OrdersUI.Instance.DisplayOrder(myOrder, myOrderSpot);
    }
}
