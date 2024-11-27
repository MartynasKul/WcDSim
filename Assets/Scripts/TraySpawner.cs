using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TraySpawner : MonoBehaviour
{
    public GameObject itemPrefab;
    public Transform spawnPoint;
    public float spawnCooldown = 3f;
    public int maxItems = 99;
    private float lastSpawnTime = -Mathf.Infinity;
    private List<GameObject> spawnedItems = new List<GameObject>(); // List to keep track of spawned items

    void OnTriggerEnter(Collider other)
    {
        if (Time.time >= lastSpawnTime + spawnCooldown && spawnedItems.Count < maxItems)
        {
            // Debug.Log($"{gameObject.name} is attempting to spawn an item.");
            SpawnItem();
            lastSpawnTime = Time.time;
        }
        else
        {
           // Debug.Log($"{gameObject.name} cannot spawn. Either cooldown not met, max items reached, or tag mismatch.");
        }   
    }

    private void SpawnItem()
    {
        // Instantiate the item at the spawn point and add it to the list
        GameObject newItem = Instantiate(itemPrefab, spawnPoint.position, spawnPoint.rotation);
        spawnedItems.Add(newItem);
        
        // Remove any destroyed items from the list (cleanup in case items are removed)
        CleanupDestroyedItems();
    }

    private void CleanupDestroyedItems()
    {
        spawnedItems.RemoveAll(item => item == null);
    }

    public void Update(){
        CleanupDestroyedItems();
    }

}
