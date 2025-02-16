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
    }

}
