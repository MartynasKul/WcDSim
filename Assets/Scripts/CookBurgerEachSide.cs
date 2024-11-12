using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookBurgerEachSide : MonoBehaviour
{
    public ParticleSystem fireEffect; // Reference to the particle system

    private Color startColor1;   // Original color of the first side of the burger
    private Color startColor2;   // Original color of the second side of the burger
    public Color cookedColor = Color.black;  // Final darker color of the burger meat
    public float cookingSpeed = 0.5f;        // Speed of color change

    private Renderer burgerRenderer;
    private bool isCooking = false; // Flag to indicate cooking status
    private float cookingProgress1 = 0f; // Progress of the cooking effect for side 1
    private float cookingProgress2 = 0f; // Progress of the cooking effect for side 2
    private bool isFacingUp = false; // To track which side is facing the heater
    private bool fireEffectStarted = false; // Flag to prevent multiple fire effects

    public AudioSource audio;
    public AudioClip sizzle;
    public AudioClip burn;




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

        // Find the particle system if it hasn't been assigned in the inspector
        if (fireEffect == null)
        {
            fireEffect = GetComponentInChildren<ParticleSystem>();  // Automatically find it in child objects
        }

        // Ensure the fire effect is off at the start
        if (fireEffect != null)
        {
            fireEffect.Stop();
        }
        else
        {
            Debug.LogWarning("Fire effect particle system not found.");
        }

        audio.Stop();
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("heater"))
        {
            isCooking = true; // Start changing color
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("heater"))
        {
            isCooking = false; // Stop changing color
        }
    }

    void Update()
    {
        if (isCooking)
        {
            audio.clip = sizzle;
            audio.Play();
            if (Vector3.Dot(transform.up, Vector3.up) >= 0.5f)
            {
                isFacingUp = true;
            }
            else if (Vector3.Dot(transform.up, Vector3.down) > 0.5f)
            {
                isFacingUp = false;
            }

            if (isFacingUp)
            {
                cookingProgress1 += cookingSpeed * Time.deltaTime;
                cookingProgress1 = Mathf.Clamp01(cookingProgress1);
                burgerRenderer.materials[1].color = Color.Lerp(startColor1, cookedColor, cookingProgress1);
            }
            else
            {
                cookingProgress2 += cookingSpeed * Time.deltaTime;
                cookingProgress2 = Mathf.Clamp01(cookingProgress2);
                burgerRenderer.materials[0].color = Color.Lerp(startColor2, cookedColor, cookingProgress2);
            }

            if (fireEffectStarted)
            {
                
                float cookprogmax = cookingProgress1;
                if (cookingProgress2 > cookprogmax)
                {
                    cookprogmax = cookingProgress2;
                }
                //cookingProgress1 += cookingSpeed * Time.deltaTime;
                //cookingProgress1 = Mathf.Clamp01(cookingProgress1);
                burgerRenderer.materials[1].color = Color.Lerp(startColor1, cookedColor, cookprogmax);
                //cookingProgress2 += cookingSpeed * Time.deltaTime;
                //cookingProgress2 = Mathf.Clamp01(cookingProgress2);
                burgerRenderer.materials[0].color = Color.Lerp(startColor2, cookedColor, cookprogmax);
            }

            // Check if one side is fully cooked (i.e., fully black)
            if ((cookingProgress1 >= 1f || cookingProgress2 >= 1f) && !fireEffectStarted)
            {
                StartFireEffect();
            }
        }
        else{
            if(audio.isPlaying){
                audio.Stop();
            }
        }
    }

    private void StartFireEffect()
    {
        if (fireEffect != null && !fireEffect.isPlaying)
        {
            fireEffect.Play();  // Start the fire particle system
            fireEffectStarted = true;
            Debug.Log("Fire effect started.");

            audio.Stop();
            audio.clip=burn;
            audio.Play();
        }
    }
}

