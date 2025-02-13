using UnityEngine;

public class Fryer : MonoBehaviour
{
    [SerializeField] private Transform[] panLocations;
    public GameObject[] cookableFish;
    public GameObject cookingPan;

    public void PanEntered(int grillNumber)
    {
        Instantiate(cookingPan, panLocations[grillNumber]);
    }
}
