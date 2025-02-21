// using Unity.VisualScripting;
// using UnityEngine;

// public class Grill : MonoBehaviour
// {
//     private GameObject myStove;

//     void Start()
//     {
//         myStove = FindFirstObjectByType<StoveManager>().gameObject;
//     }

//     void OnTriggerEnter(Collider other)
//     {
//         if(!isBeingUsed)
//         {
//             if(other.tag == "Pan")
//             {
//                 myStove.GetComponent<StoveManager>().PanEntered(gameObject);
//                 Destroy(other.gameObject);
//                 isBeingUsed = true;
//             }
//         }
//     }

// }
