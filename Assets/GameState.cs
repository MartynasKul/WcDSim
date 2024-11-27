using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    public decimal playerMoney; // Player's money
    public TextMeshProUGUI moneyText; // Reference to the world space UI text

    private decimal previousMoney = 0; // Store the previous money value to show the old amount

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist GameState across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate instances
        }
    }

    private void Start()
    {
        moneyText.enabled = false; // Start with the money text hidden
        UpdateMoneyUI(); // Initialize the UI (initially hidden)
    }

    public void AddMoney(decimal amount)
    {
        previousMoney = playerMoney; // Store the previous money value
        playerMoney += amount; // Add the amount to the player's money

        Debug.Log($"Money added: {amount:C}. Total: {playerMoney:C}");

        // Start showing the old value with "+" and then show the new total
        StartCoroutine(DisplayMoneyWithAnimation(amount));
    }

    public bool DeductMoney(decimal amount)
    {
        if (playerMoney >= amount)
        {
            playerMoney -= amount;
            Debug.Log($"Money deducted: {amount:C}. Remaining: {playerMoney:C}");
            UpdateMoneyUI();
            return true;
        }
        else
        {
            Debug.LogWarning("Not enough money!");
            return false;
        }
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = $"Money: {playerMoney:C}"; // Update to show the new total money
        }
        else
        {
            Debug.LogWarning("MoneyText is not assigned in GameState!");
        }
    }

    private IEnumerator DisplayMoneyWithAnimation(decimal addedAmount)
    {
        // Show the old value with a "+" sign for 2 seconds
        if (moneyText != null)
        {
            moneyText.text = $"Money: {previousMoney:C} + {addedAmount:C}";
            moneyText.enabled = true; // Display the text immediately
        }

        // Wait for 2 seconds to show the old value
        yield return new WaitForSeconds(2f);

        // Now update the UI to show the new total money
        UpdateMoneyUI(); // Show the updated money after the addition

        // Hide the money text after the update
        yield return new WaitForSeconds(1f); // Wait for 1 more second before hiding it
        moneyText.enabled = false; // Hide the money text
    }
}



