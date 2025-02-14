using Unity.VisualScripting;
using UnityEngine;

public class HandController : MonoBehaviour
{
    
    public Transform leftHandPoint;
    public Transform rightHandPoint;
    private bool lHandFull = false;
    private bool rHandFull = false;
    public GameObject EquipObject(GameObject item)
    {
        if (!lHandFull)
        {
            GameObject throwable = Instantiate(item, leftHandPoint);
            ItemThrower itemThrower = throwable.GetComponent<ItemThrower>();

            if (itemThrower == null)
            {
                Debug.LogError("ItemThrower is missing on the instantiated object: " + throwable.name);
                return null;
            }

            itemThrower.Init(false, leftHandPoint, true);
            lHandFull = true;
            return throwable;
        }
        if (!rHandFull)
        {
            GameObject throwable = Instantiate(item, rightHandPoint);
            ItemThrower itemThrower = throwable.GetComponent<ItemThrower>();

            if (itemThrower == null)
            {
                Debug.LogError("ItemThrower is missing on the instantiated object: " + throwable.name);
                return null;
            }

            itemThrower.Init(true, rightHandPoint, true);
            rHandFull = true;
            return throwable;
        }
        return null;
    }

    public void ResetHand(bool hand) //LEFT = 0, RIGHT = 1
    {
        if(!hand)
            lHandFull = false;
        else
            rHandFull = false;
    }
}
