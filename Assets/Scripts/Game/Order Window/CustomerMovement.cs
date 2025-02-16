using TMPro;
using UnityEngine;

public class CustomerMovement : MonoBehaviour
{
    public float walkSpeed;
    private Transform orderPoint;
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
        if(transform.position == orderPoint.position)
            Order();
    }
    void Order()
    {
        if(GetComponent<Order>().anyFish)
        {
            text.text = $"Any {GetComponent<Order>().fishNumber} fish";
            speechBubble.SetActive(true);
            text.gameObject.SetActive(true);
            dish.SetActive(false);
        }
        else
        {
            dish = fishSpeech[GetComponent<Order>().fishNumber];
            speechBubble.SetActive(true);
            text.gameObject.SetActive(false);
            dish.SetActive(true);           
        }
    }

    
}
