using UnityEngine;

public class HandController : MonoBehaviour
{
    
    public Transform leftHandPoint;
    public Transform rightHandPoint;
    private bool lHandFull;
    private bool rHandFull;
    public void EquipObject(GameObject item)
    {
        if(!lHandFull)
        {
            Instantiate(item, leftHandPoint);
            lHandFull = true;
        }
        else if(!rHandFull)
        {
            Instantiate(item, rightHandPoint);
            rHandFull = true;
        }
    }

    public void ResetHand(bool hand) //LEFT = 0, RIGHT = 1
    {
        if(hand)
            lHandFull = false;
        else
            rHandFull = false;
    }
}
