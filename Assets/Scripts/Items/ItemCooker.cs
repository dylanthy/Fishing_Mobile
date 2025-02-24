using System.Collections;
using UnityEngine;

public class ItemCooker : MonoBehaviour
{
    public bool isBeingUsed;
    public int fishIdentifier;
    public float cookTime;
    public float burnTime;
    public Material cookedMaterial;
    public Material burnedMaterial;
    public bool isCooked = false;
    public bool isBurned = false;
    private GameObject parentPan;
    public void Init(GameObject pan)
    {  
        parentPan = pan;
        StartCoroutine(CookOverTime(cookTime));
        isBeingUsed = true;
        if(GetComponent<ItemThrower>())
        {
           GetComponent<ItemThrower>().enabled = false; 
        }
    }
    
    private IEnumerator CookOverTime(float time)
    {
        yield return new WaitForSeconds(time);
        if(!isCooked)
        {
            Renderer renderer = GetComponent<Renderer>();
            renderer.material = cookedMaterial;
            isCooked = true;
            StartCoroutine(CookOverTime(burnTime));
        }
        else
        {
            isBurned = true;
            Renderer renderer = GetComponent<Renderer>();
            renderer.material = burnedMaterial;
        }
    }

    public void EndCooking()
    {
        isBeingUsed = false;
        // fish.GetComponent<Renderer>().material = GetComponent<Renderer>().material;
        parentPan.GetComponent<PanCook>().FoodRemoved();
    }

}
