using Unity.VisualScripting;
using UnityEngine;

public class Grill : MonoBehaviour
{
    public int grillNum;
    public GameObject myFryer;

    private bool isBeingUsed = false;

    void OnTriggerEnter(Collider other)
    {
        if(!isBeingUsed)
        {
            if(other.tag == "Pan")
            {
                myFryer.GetComponent<Fryer>().PanEntered(grillNum);
                Destroy(other.gameObject);
                isBeingUsed = true;
            }
        }
        if(isBeingUsed && other.tag == "Cookable")
        {
            GameObject fishToCook = other.GetComponent<ThrowableController>().fishC;
            GetComponentInChildren<PanCook>().StartCook(fishToCook);
        }
    }

}
