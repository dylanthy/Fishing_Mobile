using UnityEngine;
using System.Linq; // Required for sorting
using System.Collections.Generic;
using Unity.VisualScripting; //used for Coroutine

public class PanCook : MonoBehaviour
{
    private Camera mainCamera;
    public Transform foodLocation;
    private GameObject myFood;
    private GameObject myFryer;

    private bool isFull = false;


    void Start()
    {
        mainCamera = Camera.main;
        myFryer = FindFirstObjectByType<Fryer>().gameObject;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 1f);

            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

            if (hits.Length > 1)
            {
                hits = hits.OrderBy(hit => hit.distance).ToArray();
                RaycastHit secondHit = hits[1];

                Debug.Log($"First hit: {hits[0].transform.name}, Second hit: {secondHit.transform.name}");

                if (secondHit.transform == transform)
                {
                    TryPickUp();
                }
            }
        }
    }
void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Cookable") && !isFull)
    {
        ThrowableController throwable = other.GetComponent<ThrowableController>();
        StartCook(throwable.fishC);
        Destroy(other.gameObject);
        isFull = true;
    }
}

    public void StartCook(GameObject food)
    {
        myFood = Instantiate(food, foodLocation);
        myFood.GetComponent<FoodCook>().Init();
    }

    public void TryPickUp()
    {
        if(!myFood)
        {
            Destroy(gameObject);
        }
        else
        {
            if(myFood)
        }        

    }
}
