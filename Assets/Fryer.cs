using UnityEngine;

public class Fryer : MonoBehaviour
{
    [SerializeField] private Transform[] panLocations;


    public GameObject pan;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PanEntered(int grill)
    {
        Instantiate(pan, panLocations[grill]);
    }
}
