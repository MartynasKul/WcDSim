using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BurgerStacker : MonoBehaviour
{
    public GameObject completedBurgerPrefab; // Prefab of the final, single burger item
    private bool hasBottomBun = false;
    private bool hasPatty = false;
    private bool hasTopBun = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BottomBun") && !hasBottomBun)
        {
            hasBottomBun = true;
            HandlePart(other.gameObject);
        }
        else if (other.CompareTag("Patty") && hasBottomBun && !hasPatty)
        {
            hasPatty = true;
            HandlePart(other.gameObject);
        }
        else if (other.CompareTag("TopBun") && hasBottomBun && hasPatty && !hasTopBun)
        {
            hasTopBun = true;
            HandlePart(other.gameObject);
        }

        // Check if all parts are in place
        if (hasBottomBun && hasPatty && hasTopBun)
        {
            CompleteBurger();
        }
    }

    private void HandlePart(GameObject part)
    {
        // Deactivate the part to remove it visually but keep its reference
        part.SetActive(false);
    }

    private void CompleteBurger()
    {
        // Instantiate the completed burger at the position of the stacker
        Instantiate(completedBurgerPrefab, transform.position, transform.rotation);
        
        // Reset the parts flags for the next burger
        hasBottomBun = hasPatty = hasTopBun = false;
    }
}
