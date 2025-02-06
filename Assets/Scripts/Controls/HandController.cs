using Unity.VisualScripting;
using UnityEngine;

public class HandController : MonoBehaviour
{
    
    public Transform leftHandPoint;
    public Transform rightHandPoint;
    private bool lHandFull = false;
    private bool rHandFull = false;
    public void EquipObject(GameObject item)
    {
        if(!lHandFull)
        {
            GameObject throwable = Instantiate(item, leftHandPoint);
            throwable.GetComponent<ThrowableController>().Init(false, leftHandPoint);
            lHandFull = true;
            Debug.Log($"Current Bools, L:{lHandFull}, R:{rHandFull}");
            return;
        }
        if(!rHandFull)
        {
            GameObject throwable = Instantiate(item, rightHandPoint);
            throwable.GetComponent<ThrowableController>().Init(true, rightHandPoint);
            rHandFull = true;
            Debug.Log($"Current Bools, L:{lHandFull}, R:{rHandFull}");
        }
    }

    public void ResetHand(bool hand) //LEFT = 0, RIGHT = 1
    {
        if(!hand)
            lHandFull = false;
        else
            rHandFull = false;
    }
}
