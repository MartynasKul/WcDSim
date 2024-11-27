using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NPCController : MonoBehaviour
{
    public Transform spawnPoint;
    public Transform counterPoint;
    public Transform exitPoint;
    public AudioSource entrySound;
    public float waitTimeAtCounter = 15f;
    public Canvas orderCanvas; // Reference to the NPC's canvas
    public float spacing = 1.5f; // Distance between NPCs at the counter
    private NavMeshAgent agent;

    private static List<Vector3> occupiedCounterPositions = new List<Vector3>(); // Shared by all NPCs

    private enum NPCState { Entering, Waiting, Exiting }
    private NPCState currentState;

    private Vector3 assignedCounterPosition;

    private Dictionary<string, int> burgerOrder = new Dictionary<string, int>();
    private Dictionary<string, int> completedOrder = new Dictionary<string, int>();
    private static readonly Dictionary<string, decimal> burgerValues = new Dictionary<string, decimal>
    {
        { "Burger", 1.00m },
        { "CheeseBurger", 1.50m },
        { "TomatoBurger", 1.50m },
        { "CheeseAndTomatoBurger", 2.00m }
    };

    private TextMeshProUGUI orderText;// Reference to the TextMeshPro component

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        assignedCounterPosition = GetAvailableCounterPosition(counterPoint.position);
        currentState = NPCState.Entering;
        agent.SetDestination(assignedCounterPosition);
        entrySound?.Play();

        GenerateRandomOrder();

        // Ensure the canvas is initially hidden
        if (orderCanvas != null)
        {
            // Find the TextMeshProUGUI component
            orderText = orderCanvas.GetComponent<TextMeshProUGUI>();
            if (orderText != null)
            {
                orderText.enabled = false; // Hide the text initially
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found on the canvas!");
            }
        }
        else
        {
            Debug.LogError("Order canvas is not assigned!");
        }

        Debug.Log($"{gameObject.name} is entering the store, heading to {assignedCounterPosition}.");
    }

    private void GenerateRandomOrder()
    {
        string[] possibleBurgerTypes = { "Burger", "CheeseBurger", "TomatoBurger", "CheeseAndTomatoBurger" };
        int burgerCount = Random.Range(1, 4); // Randomly choose between 1 and 3 burgers

        for (int i = 0; i < burgerCount; i++)
        {
            string burgerType = possibleBurgerTypes[Random.Range(0, possibleBurgerTypes.Length)];
            if (burgerOrder.ContainsKey(burgerType))
            {
                burgerOrder[burgerType]++;
            }
            else
            {
                burgerOrder[burgerType] = 1;
                completedOrder[burgerType] = 0; // Initialize completed count
            }
        }
    }

    private Vector3 GetAvailableCounterPosition(Vector3 basePosition)
    {
        Vector3 offset = Vector3.zero;
        for (int i = 0; i < occupiedCounterPositions.Count + 1; i++)
        {
            offset = new Vector3((i % 3) * spacing, 0, (i / 3) * spacing); // Calculate grid-like offset
            Vector3 potentialPosition = basePosition + offset;
            if (!occupiedCounterPositions.Contains(potentialPosition)) // Ensure it's not already taken
            {
                occupiedCounterPositions.Add(potentialPosition);
                return potentialPosition;
            }
        }
        return basePosition;
    }

    private void ReleaseCounterPosition()
    {
        if (occupiedCounterPositions.Contains(assignedCounterPosition))
        {
            occupiedCounterPositions.Remove(assignedCounterPosition);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        string burgerType = other.tag; // Use the tag as the burger type

        if (burgerOrder.ContainsKey(burgerType) && completedOrder[burgerType] < burgerOrder[burgerType])
        {
            ProcessBurger(burgerType);
            Destroy(other.gameObject); // Remove the burger from the scene
        }
        else
        {
            Debug.Log($"{gameObject.name} did not order {burgerType}!");
        }
    }

    private void ProcessBurger(string burgerType)
    {
        if (burgerOrder.ContainsKey(burgerType) && completedOrder[burgerType] < burgerOrder[burgerType])
        {
            completedOrder[burgerType]++;
            UpdateOrderCanvas();

            // Add money based on the burger type
            if (burgerValues.TryGetValue(burgerType, out decimal value))
            {
                GameState.Instance.AddMoney(value); // Add the specific value of the burger
                Debug.Log($"{gameObject.name} received {burgerType} worth {value:C}. Total money: {GameState.Instance.playerMoney:C}");
            }
            else
            {
                Debug.LogWarning($"Burger type {burgerType} not found in values dictionary!");
            }

            // Check if all orders are complete
            if (IsOrderComplete())
            {
                Debug.Log($"{gameObject.name} has received their complete order!");
                ExitStore();
            }
        }
        else
        {
            Debug.Log($"{gameObject.name} did not order {burgerType}!");
        }
    }

    private bool IsOrderComplete()
    {
        foreach (var item in burgerOrder)
        {
            if (completedOrder[item.Key] < item.Value)
            {
                return false;
            }
        }
        return true;
    }

    private void UpdateOrderCanvas()
    {
        if (orderCanvas != null && orderText != null)
        {
            string displayText = "";
            foreach (var item in burgerOrder)
            {
                displayText += $"{item.Key} {completedOrder[item.Key]}/{item.Value}\n";
            }
            Debug.Log($"Updating order text: {displayText}");
            orderText.text = displayText;
        }
        else
        {
            Debug.LogWarning("OrderCanvas or OrderText is null!");
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case NPCState.Entering:
                if (Vector3.Distance(transform.position, assignedCounterPosition) < 2f)
                {
                    currentState = NPCState.Waiting;
                    StartCoroutine(WaitForBurger());

                    if (orderCanvas != null)
                    {
                        orderCanvas.enabled = true;
                        orderText.enabled = true;
                        UpdateOrderCanvas(); // Show the initial order
                    }
                }
                break;

            case NPCState.Waiting:
                // Waiting state logic
                break;

            case NPCState.Exiting:
                if (Vector3.Distance(transform.position, exitPoint.position) < 2f)
                {
                    ReleaseCounterPosition();
                    Destroy(gameObject); // Remove NPC once they reach the exit
                }
                break;
        }
    }

    IEnumerator WaitForBurger()
    {
        Debug.Log($"{gameObject.name}: My order is:\n{GetOrderSummary()}");
        float elapsedTime = 0f;
        while (!IsOrderComplete() && elapsedTime < waitTimeAtCounter)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (IsOrderComplete())
        {
            Debug.Log("Yay! I got my order!");
        }
        else
        {
            Debug.Log($"{gameObject.name} is leaving because they didn't get their order.");
        }

        ExitStore();
    }

    private string GetOrderSummary()
    {
        string summary = "";
        foreach (var item in burgerOrder)
        {
            summary += $"{item.Key}: {completedOrder[item.Key]}/{item.Value}\n";
        }
        return summary;
    }

    private void ExitStore()
    {
        currentState = NPCState.Exiting;
        agent.SetDestination(exitPoint.position);

        if (orderCanvas != null)
            orderCanvas.enabled = false; // Hide the canvas when exiting
    }
}


