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
    // Update is called once per frame
    void Update()
    {
        if(!orderPoint)
            Debug.LogWarning("NoOrderPoint");
        transform.position = Vector3.Lerp(transform.position, orderPoint.position, Time.deltaTime * walkSpeed);
        if(Vector3.Distance(transform.position, orderPoint.position) < 0.1f)
            SayOrder();
    }
    public void SayOrder()
    {

        if(GetComponent<Order>().orderAnyFish)
        {
            text.text = $"Any {GetComponent<Order>().orderFishNumber} fish";
            speechBubble.SetActive(true);
            text.gameObject.SetActive(true);
            hasOrderedOnce = true;
        }
        else
        {
            GameObject dish = fishSpeech[GetComponent<Order>().orderFishNumber];
            speechBubble.SetActive(true);
            text.gameObject.SetActive(false);
            dishParent.SetActive(true);
            if(!hasOrderedOnce)
            {
                GameObject display = Instantiate(dish, gameObject.transform);
                display.transform.SetParent(dishParent.transform, false);
                hasOrderedOnce = true;
            }
                  
        }
    }
}
