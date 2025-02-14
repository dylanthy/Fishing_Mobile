using UnityEngine;
using System.Linq; // Required for sorting

public class PanCook : MonoBehaviour
{
    private Camera mainCamera;
    public Transform foodLocation;
    private GameObject myFood;

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

                // Debug.Log($"First hit: {hits[0].transform.name}, Second hit: {secondHit.transform.name}");

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
            ThrowableController throwable = other.GetComponent<ThrowableController>();
            StartCook(throwable.fish);
            Destroy(other.gameObject);
        }
    }
}

    public void StartCook(GameObject food)
    {
        myFood = Instantiate(food, foodLocation);
        myFood.GetComponent<CookableItem>().Init();
    }

    public void TryPickUp()
    {
        if(!myFood)
        {
            Destroy(gameObject);
        }
        else
        {
            if(!myFood.GetComponent<CookableItem>().isCooked)
                return;
            else if(myFood.GetComponent<CookableItem>().isBurned)
            {
                
            }
        }        

    }
}
