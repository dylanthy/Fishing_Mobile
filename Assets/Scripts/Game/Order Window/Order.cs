using System.Collections.Generic;
using UnityEngine;

public class Order : MonoBehaviour
{
    public bool anyFish;
    public int fishNumber;

    public void Init() // 75 % chance for a random fish order , 25% chance for a "specific" fish order
    {
        anyFish = Random.value <.75f;
        if(!anyFish)
        {
            fishNumber = Random.Range(0,4);
        }
        else // random quantity of fish, 93% chance they want 1 fish, 7% chance they want 2
        {
            fishNumber = (Random.value < .93f) ? 1 : 2;
        }
    }
}
