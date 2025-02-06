using Unity.VisualScripting;
using UnityEngine;

public class Grill : MonoBehaviour
{
    public int grillNum;
    public GameObject myFryer;

    void OnTriggerEnter()
    {
        myFryer.GetComponent<Fryer>().PanEntered(grillNum);
    }
}
