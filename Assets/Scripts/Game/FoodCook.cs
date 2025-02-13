using System.Collections;
using UnityEngine;

public class FoodCook : MonoBehaviour
{
    public float cookTime;
    public float burnTime;
    public Material cookedMaterial;
    public Material burnedMaterial;

    public bool isCooked = false;
    public bool isBurned = false;
    public void Init()
    {
        StartCoroutine(CookOverTime(cookTime, false));
    }
    
    private IEnumerator CookOverTime(float time, bool isCookedYet)
    {
        yield return new WaitForSeconds(time);
        if(!isCookedYet)
        {
            Renderer renderer = GetComponent<Renderer>();
            renderer.material = cookedMaterial;
            StartCoroutine(CookOverTime(burnTime, true));
        }
        else
        {
            Renderer renderer = GetComponent<Renderer>();
            renderer.material = burnedMaterial;
        }
    }

}
