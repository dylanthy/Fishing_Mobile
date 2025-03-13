using TMPro;
using UnityEngine;

public class CustomerMovement : MonoBehaviour
{
    public float walkSpeed;
    public Transform orderPoint;
    private GameObject orderManager;
    public GameObject[] fishSpeech;

    public GameObject speechBubble;
    public TextMeshPro text;
    public GameObject dishParent;

    private bool hasOrderedOnce = false;

    public void Init(Transform orderPoint, GameObject orderManager)
    {
        this.orderPoint = orderPoint;
        this.orderManager = orderManager;
    }

    void Update()
    {
        if (!orderPoint)
            Debug.LogWarning("NoOrderPoint");
        transform.position = Vector3.Lerp(transform.position, orderPoint.position, Time.deltaTime * walkSpeed);
        if (Vector3.Distance(transform.position, orderPoint.position) < 0.1f)
            SayOrder();
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

        // Update the timer display
        float remainingTime = order.remainingOrderTime;
        float totalTime = order.orderTime;
        float percentage = remainingTime / totalTime;
        if (percentage < 0.33f)
        {
            // Set health bar to red
        }
        else if (percentage < 0.66f)
        {
            // Set health bar to yellow
        }
        else
        {
            // Set health bar to green
        }
    }
}
