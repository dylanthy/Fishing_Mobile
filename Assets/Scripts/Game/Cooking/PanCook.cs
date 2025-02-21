using UnityEngine;
using System.Linq; // Required for sorting

public class PanCook : MonoBehaviour
{
    private Camera mainCamera;
    public Transform foodLocation;
    [SerializeField] private GameObject myFood;

    [SerializeField] private bool isFull = false;


    void Start()
    {
        mainCamera = Camera.main;
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
    if(!isFull)
    {
        if (other.CompareTag("Cookable"))
        {
            isFull = true;
            ItemThrower throwable = other.GetComponent<ItemThrower>();
            StartCook(throwable.gameObject);
            Destroy(other.gameObject);
        }
    }
}

    public void StartCook(GameObject food)
    {
        myFood = Instantiate(food, foodLocation);
        Rigidbody rb = myFood.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        
        myFood.transform.position = foodLocation.position;
        myFood.transform.rotation = foodLocation.rotation;
        
        myFood.GetComponent<ItemCooker>().Init(gameObject);
    }


    public void TryPickUp()
    {
        if(!myFood)
        {
            // Destroy(gameObject);
        }
        else
        {
            myFood.GetComponent<ItemGrabber>().EquipItem();
        }        

    }

    public void FoodRemoved()
    {
        myFood = null;
        isFull = false;
    }
}
