using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurgerMeat : MonoBehaviour
{
    private Color startColor;   // Original color of the burger meat
    public Color cookedColor = Color.black;  // Final darker color of the burger meat
    public float cookingSpeed = 0.5f;        // Speed of color change

    private Renderer meatRenderer;
    private bool isCooking = false;
    private float cookingProgress = 0f;

    void Start()
    {
        // Get the Renderer component of the burger meat to change its color
        meatRenderer = GetComponent<Renderer>();
        startColor = meatRenderer.material.color;  // Store the original color
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object has the "Heater" tag
        if (collision.gameObject.CompareTag("heater"))
        {
            isCooking = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Stop cooking when the burger meat is no longer colliding with the heater
        if (collision.gameObject.CompareTag("heater"))
        {
            isCooking = false;
        }
    }

    void Update()
    {
        // Gradually darken the color while the meat is cooking
        if (isCooking)
        {
            cookingProgress += cookingSpeed * Time.deltaTime;
            meatRenderer.material.color = Color.Lerp(startColor, cookedColor, cookingProgress);
            //Debug.Log("cooking");
            // Clamp the cooking progress so it doesn't exceed 1
            cookingProgress = Mathf.Clamp01(cookingProgress);
        }
    }
}
