using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NPCController : MonoBehaviour
{
    public AudioSource entrySound;

    public Transform exitPoint; // Assigned by NPCSpawner
    public float moveSpeed = 3f;
    public Canvas orderCanvas; // Reference to the NPC's canvas
    public float waitTimeAtCounter = 15f;

    private List<Transform> queuePositions; // Assigned by NPCSpawner
    private int currentQueueIndex = -1; // NPC's current queue position index
    private Vector3 targetPosition; // Current movement target

    private enum NPCState { MovingToQueue, AtCounter, Exiting }
    private NPCState currentState = NPCState.MovingToQueue;

    private Dictionary<string, int> burgerOrder = new Dictionary<string, int>();
    private Dictionary<string, int> completedOrder = new Dictionary<string, int>();
    private static readonly Dictionary<string, decimal> burgerValues = new Dictionary<string, decimal>
    {
        { "Burger", 1.00m },
        { "CheeseBurger", 1.50m },
        { "TomatoBurger", 1.50m },
        { "CheeseAndTomatoBurger", 2.00m },
        { "PotatoFries", 1.00m}
    };

    private TextMeshProUGUI orderText;// Reference to the TextMeshPro component

    public void Initialize(List<Transform> queuePositions, Transform exitPoint)
    {
        this.queuePositions = queuePositions;
        this.exitPoint = exitPoint;

        // Assign the first available queue position (e.g., P1)
        AssignQueuePosition();
        MoveToPosition(targetPosition);
    }

    void Start()
    {
        if (orderCanvas != null)
        {
            // Find the TextMeshProUGUI component
            orderText = orderCanvas.GetComponent<TextMeshProUGUI>();
            if (orderText != null)  orderText.enabled = false;
            else Debug.LogError("TextMeshProUGUI component not found on the canvas!");
        }
        else Debug.LogError("Order canvas is not assigned!");

        entrySound?.Play();
        GenerateRandomOrder();
        Debug.Log($"{gameObject.name} is entering the store, heading to {targetPosition}.");
    }

    void Update()
    {
         switch (currentState)
        {
            case NPCState.MovingToQueue:
                MoveToPosition(targetPosition);

                if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                {
                    if (currentQueueIndex == queuePositions.Count - 1)
                    {
                        StartCoroutine(ProcessOrder());
                        currentState = NPCState.AtCounter;
                    }
                    else if (currentQueueIndex + 1 < queuePositions.Count && !IsPositionOccupied(currentQueueIndex + 1))
                    {
                        currentQueueIndex++;
                        targetPosition = queuePositions[currentQueueIndex].position;
                    }
                }
                break;

            case NPCState.Exiting:
                MoveToPosition(exitPoint.position);

                if (Vector3.Distance(transform.position, exitPoint.position) < 0.1f)
                {
                    Destroy(gameObject);
                }
                break;
        }
    }
    private void MoveToTarget(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    private void MoveToPosition(Vector3 targetPosition)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    private void AssignQueuePosition()
    {
        for (int i = 0; i < queuePositions.Count; i++)
        {
            if (!IsPositionOccupied(i))
            {
                currentQueueIndex = i;
                targetPosition = queuePositions[i].position;
                return;
            }
        }

        Debug.LogWarning($"{gameObject.name}: No available queue position!");
    }
    private void CheckQueueProgression()
    {
        // Check if close enough to the current position
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            // If at the counter, start processing
            if (currentQueueIndex == queuePositions.Count - 1) // At CT
            {
                StartCoroutine(ProcessOrder());
                currentState = NPCState.AtCounter;
                return;
            }

            // Check if the next position is empty
            if (currentQueueIndex + 1 < queuePositions.Count && !IsPositionOccupied(currentQueueIndex + 1))
            {
                currentQueueIndex++;
                targetPosition = queuePositions[currentQueueIndex].position;
            }
        }
    }

    private bool IsPositionOccupied(int index)
    {
        Collider[] colliders = Physics.OverlapSphere(queuePositions[index].position, 0.5f);
        foreach (Collider col in colliders)
        {
            if (col.gameObject != this.gameObject && col.CompareTag("NPC"))
                return true;
        }
        return false;
    }
    private void GenerateRandomOrder()
    {
        string[] possibleBurgerTypes = { "Burger", "CheeseBurger", "TomatoBurger", "CheeseAndTomatoBurger", "PotatoFries" };
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

    private IEnumerator ProcessOrder()
    {
        Debug.Log($"{gameObject.name}: Processing order at counter.");
        orderCanvas.enabled = true;
        orderText.enabled = true;
        UpdateOrderCanvas();

        yield return new WaitForSeconds(waitTimeAtCounter);

        Debug.Log($"{gameObject.name}: Order complete. Exiting.");
        currentState = NPCState.Exiting;
        targetPosition = exitPoint.position;
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

    private void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed*Time.deltaTime);
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
        targetPosition = exitPoint.position;

        if (orderCanvas != null)
            orderCanvas.enabled = false; // Hide the canvas when exiting
    }
}