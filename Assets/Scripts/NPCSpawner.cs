using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public List<GameObject> npcPrefabs;
    public Transform spawnPoint;
    public Transform counterPoint;
    public Transform exitPoint;
    public float spawnInterval = 10f;
    public int maxNpcs = 10;
    public int npcnum=0;

    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(SpawnNPCs());
    }
     
    private IEnumerator SpawnNPCs()
    {
        while(true)
        {
            if(npcnum<=maxNpcs)
            {
                
                SpawnNPC();
                
                yield return new WaitForSeconds(spawnInterval);
            }
            else
            {
                Debug.Log("TOO MANY NPC MAN STOP PLZ");
            }
            
        }
    }

    private void SpawnNPC()
    {
        if(npcPrefabs.Count==0)
        {
            Debug.Log("NO NPC IN LIST BOSS UNGA");
            return;
        }

        npcnum++;
        // select random npc from list
        int rnd = Random.Range(0, npcPrefabs.Count);
        GameObject selectedNPC = npcPrefabs[rnd];
        //instantiate random npc from list
        GameObject newNPC = Instantiate(selectedNPC, spawnPoint.position, Quaternion.identity);
        NPCController npcController = newNPC.GetComponent<NPCController>();

        //assing necessary poins to the npc controller
        npcController.spawnPoint = spawnPoint;
        npcController.counterPoint = counterPoint;
        npcController.exitPoint = exitPoint;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
