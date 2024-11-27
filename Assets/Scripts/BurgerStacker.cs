using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BurgerStacker : MonoBehaviour
{
    public GameObject completedBurgerPrefab; // Prefab of the final, single burger item
    public GameObject failedBurgerPrefab;
    public GameObject completedCheeseBurgerPrefab;
    public GameObject completedTomatoBurgerPrefab;
    public GameObject completedTomatoAndCheeseBurgerPrefab;

    private bool hasBottomBun = false;
    private bool hasPatty = false;
    private bool hasTopBun = false;
    private bool hasCheese = false;
    private bool hasTomato = false;

    private List<GameObject> collectedParts = new List<GameObject>(); // Store parts for later destruction

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BottomBun") && !hasBottomBun)
        {
            hasBottomBun = true;
            CollectPart(other.gameObject);
        }
        else if (other.CompareTag("Patty") && hasBottomBun && !hasPatty)
        {
            hasPatty = true;
            CollectPart(other.gameObject);
        }
        else if (other.CompareTag("TopBun") && hasBottomBun && hasPatty && !hasTopBun)
        {
            hasTopBun = true;
            CollectPart(other.gameObject);
        }
        else if (other.CompareTag("Cheese") && hasBottomBun && hasPatty && !hasTopBun)
        {
            hasCheese = true;
            CollectPart(other.gameObject);
        }
        else if (other.CompareTag("Tomato") && hasBottomBun && hasPatty && !hasTopBun)
        {
            hasTomato = true;
            CollectPart(other.gameObject);
        }

        // Check if all parts are in place
        if (hasBottomBun && hasPatty && hasTopBun)
        {
            CompleteBurger();
        }
    }

    private void CollectPart(GameObject part)
    {
        // Add the part to the collected parts list
        collectedParts.Add(part);
    }

    private void CompleteBurger()
    {
        GameObject completedBurger = null;

        // Instantiate the correct completed burger prefab
        if (hasBottomBun && hasPatty && hasTomato && hasCheese && hasTopBun)
        {
            completedBurger = Instantiate(completedTomatoAndCheeseBurgerPrefab, transform.position, transform.rotation);
        }
        else if (hasBottomBun && hasPatty && hasTomato && hasTopBun)
        {
            completedBurger = Instantiate(completedTomatoBurgerPrefab, transform.position, transform.rotation);
        }
        else if (hasBottomBun && hasPatty && hasCheese && hasTopBun)
        {
            completedBurger = Instantiate(completedCheeseBurgerPrefab, transform.position, transform.rotation);
        }
        else if (hasBottomBun && hasPatty && hasTopBun)
        {
            completedBurger = Instantiate(completedBurgerPrefab, transform.position, transform.rotation);
        }

        if (completedBurger != null)
        {
            // Locate the patty in the prefab
            Transform pattyTransform = completedBurger.transform.Find("Patty"); // Adjust the name if necessary
            if (pattyTransform != null)
            {
                Renderer pattyRenderer = pattyTransform.GetComponent<Renderer>();
                if (pattyRenderer != null)
                {
                    // Get the cooking script from the original patty
                    CookBurgerEachSide cookScript = collectedParts.Find(p => p.CompareTag("Patty")).GetComponent<CookBurgerEachSide>();
                    if (cookScript != null)
                    {
                        // Get the cooked colors for both sides
                        var (side1Color, side2Color) = cookScript.GetCurrentColors();

                        // Apply the colors to the materials of the prefab's patty
                        Material[] materials = pattyRenderer.materials;
                        if (materials.Length > 0)
                        {
                            materials[0].color = side1Color; // First side
                        }
                        if (materials.Length > 1)
                        {
                            materials[1].color = side2Color; // Second side
                        }
                        pattyRenderer.materials = materials;
                    }
                }
            }
        }

        // Destroy all collected parts
        foreach (var part in collectedParts)
        {
            Destroy(part);
        }

        // Clear the collected parts list
        collectedParts.Clear();

        // Reset the part flags for the next burger
        hasBottomBun = hasPatty = hasTopBun = hasTomato = hasCheese = false;
    }

    // Example method to determine the cooked color
    //public Color GetCurrentColor()
    //{
    //    float maxCookingProgress = Mathf.Max(cookingProgress1, cookingProgress2);
    //    return Color.Lerp(startColor1, cookedColor, maxCookingProgress);
    //}

}
