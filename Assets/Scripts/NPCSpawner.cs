using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public List<GameObject> npcPrefabs;
    public Transform spawnPoint;
    public Transform exitPoint;
    public List<Transform> queuePositions;
    public List<NPCController> activeNPCs = new List<NPCController>();
    private float spawnInterval = 5f;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnNPCs());
    }
     
    private IEnumerator SpawnNPCs()
    {
        while(true)
        {
            SpawnNPC();
            spawnInterval=DifficultySelector.Instance.GetSpawnInterval();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnNPC()
    {
        if(npcPrefabs.Count==0)
        {
            Debug.Log("NO NPC IN LIST BOSS UNGA");
            return;
        }

        GameObject newNPC = Instantiate(npcPrefabs[Random.Range(0, npcPrefabs.Count)], spawnPoint.position, Quaternion.identity);
        NPCController npcController = newNPC.GetComponent<NPCController>();
        npcController.Initialize(queuePositions, exitPoint);
    }
}