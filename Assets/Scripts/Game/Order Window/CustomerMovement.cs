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
    public GameObject dish;


    public void Init(Transform orderPoint, GameObject orderManager)
    {
        this.orderPoint = orderPoint;
        this.orderManager = orderManager;
    }
    // Update is called once per frame
    void Update()
    {
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
            dish.SetActive(false);
        }
        else
        {
            dish = fishSpeech[GetComponent<Order>().orderFishNumber];
            speechBubble.SetActive(true);
            text.gameObject.SetActive(false);
            dish.SetActive(true);           
        }
    }
}
