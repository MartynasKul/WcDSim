using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookBurgerEachSide : MonoBehaviour
{
    private Color startColor1;   // Original color of the first side of the burger
    private Color startColor2;   // Original color of the second side of the burger
    public Color cookedColor = Color.black;  // Final darker color of the burger meat
    public float cookingSpeed = 0.5f;        // Speed of color change

    private Renderer burgerRenderer;
    private bool isCooking = false; // Flag to indicate cooking status
    private float cookingProgress1 = 0f; // Progress of the cooking effect
    private float cookingProgress2 = 0f; // Progress of the cooking effect
    private bool isFacingUp = false; // To track which side is facing the heater

    void Start()
    {
        // Get the Renderer component of the burger to change its color
        burgerRenderer = GetComponent<Renderer>();

        // Store the original colors of both materials (assuming the mesh has two materials)
        if (burgerRenderer.materials.Length > 0)
        {
            startColor1 = burgerRenderer.materials[1].color; // First side
        }
        if (burgerRenderer.materials.Length > 1)
        {
            startColor2 = burgerRenderer.materials[0].color; // Second side
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object has the "Heater" tag
        if (collision.gameObject.CompareTag("heater"))
        {
            isCooking = true; // Start changing color
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Stop cooking when the burger is no longer colliding with the heater
        if (collision.gameObject.CompareTag("heater"))
        {
            isCooking = false; // Stop changing color
            //ResetColors(); // Reset colors back to original
        }
    }

    void Update()
    {
        // Check which way the burger is facing
        if (isCooking)
        {
            // Determine if the burger is facing up or down
            if (Vector3.Dot(transform.up, Vector3.up) > 0.5f) // Facing up
            {
                isFacingUp = true;
                Debug.Log("Burger is facing up.");
            }
            else if (Vector3.Dot(transform.up, Vector3.down) > 0.5f) // Facing down
            {
                isFacingUp = false;
                Debug.Log("Burger is facing down.");
            }

            

            // Change the color based on the facing side
            if (isFacingUp)
            {
                // Gradually darken the color while the burger is cooking
                cookingProgress1 += cookingSpeed * Time.deltaTime;

                // Clamp the cooking progress so it doesn't exceed 1
                cookingProgress1 = Mathf.Clamp01(cookingProgress1);
                burgerRenderer.materials[1].color = Color.Lerp(startColor1, cookedColor, cookingProgress1);
            }
            else
            {
                // Gradually darken the color while the burger is cooking
                cookingProgress2 += cookingSpeed * Time.deltaTime;

                // Clamp the cooking progress so it doesn't exceed 1
                cookingProgress2 = Mathf.Clamp01(cookingProgress2);
                burgerRenderer.materials[0].color = Color.Lerp(startColor2, cookedColor, cookingProgress2);
            }
        }
    }

    private void ResetColors()
    {
        // Reset the colors of both sides to their original colors
        //if (burgerRenderer.materials.Length > 0)
        //{
        //    burgerRenderer.materials[1].color = startColor1;
        //}
        //if (burgerRenderer.materials.Length > 1)
        //{
        //    burgerRenderer.materials[0].color = startColor2;
        //}
    }
}
